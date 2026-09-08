using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LotmFontPatcher
{
    public class PatchFontAdvance
    {
        private static ushort ReadU16(byte[] data, int pos)
        {
            return (ushort)((data[pos] << 8) | data[pos + 1]);
        }

        private static short ReadS16(byte[] data, int pos)
        {
            return (short)((data[pos] << 8) | data[pos + 1]);
        }

        private static uint ReadU32(byte[] data, int pos)
        {
            return ((uint)data[pos] << 24) | ((uint)data[pos + 1] << 16) | ((uint)data[pos + 2] << 8) | (uint)data[pos + 3];
        }

        private static void WriteU16(byte[] data, int pos, ushort val)
        {
            data[pos] = (byte)((val >> 8) & 0xFF);
            data[pos + 1] = (byte)(val & 0xFF);
        }

        private static void WriteU32(byte[] data, int pos, uint val)
        {
            data[pos] = (byte)((val >> 24) & 0xFF);
            data[pos + 1] = (byte)((val >> 16) & 0xFF);
            data[pos + 2] = (byte)((val >> 8) & 0xFF);
            data[pos + 3] = (byte)(val & 0xFF);
        }

        private static uint CalcTableChecksum(byte[] data, uint offset, uint length)
        {
            uint sum = 0;
            uint nLongs = (length + 3) / 4;
            for (uint i = 0; i < nLongs; i++)
            {
                uint pos = offset + i * 4;
                uint b0 = pos < data.Length ? data[pos] : (uint)0;
                uint b1 = pos + 1 < data.Length ? data[pos + 1] : (uint)0;
                uint b2 = pos + 2 < data.Length ? data[pos + 2] : (uint)0;
                uint b3 = pos + 3 < data.Length ? data[pos + 3] : (uint)0;
                sum += (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
            }
            return sum;
        }

        public struct TableRecord
        {
            public string Tag;
            public int DirOffset;
            public uint Checksum;
            public uint Offset;
            public uint Length;
        }

        // Proportional advance width map matching Aleo design
        // Standard lowercase: ~540–560
        // Standard uppercase: ~620–680
        // Wide letters proportional to width
        public static ushort GetProportionalAdvance(char c)
        {
            switch (c)
            {
                // Uppercase Cyrillic (0x0401, 0x0410 - 0x042F)
                case 'Г': return 620;
                case 'Р': return 630;
                case 'Б': case 'В': case 'Ь': return 640;
                case 'З': return 650;
                case 'Е': case 'Ё': return 660;
                case 'А': case 'К': case 'С': case 'Т': case 'У': case 'Э': case 'Я': return 680;
                case 'И': case 'Й': case 'Л': case 'Н': case 'П': case 'Ч': return 700;
                case 'О': case 'Х': case 'Ц': case 'Д': case 'Ъ': return 720;
                case 'Ф': return 760;
                case 'Ы': return 820;
                case 'Ю': return 860;
                case 'Ж': case 'М': case 'Ш': return 880;
                case 'Щ': return 900;

                // Lowercase Cyrillic (0x0451, 0x0430 - 0x044F)
                case 'г': case 'з': return 520;
                case 'с': case 'э': return 530;
                case 'в': case 'е': case 'т': case 'ь': case 'ё': return 540;
                case 'а': case 'б': case 'к': case 'у': return 550;
                case 'о': return 554;
                case 'и': case 'й': case 'н': case 'п': case 'р': case 'х': case 'ч': case 'я': return 560;
                case 'л': return 570;
                case 'д': case 'ц': case 'ъ': return 580;
                case 'м': case 'ы': case 'ю': return 700;
                case 'ж': case 'ш': case 'ф': return 780;
                case 'щ': return 800;

                default:
                    if (char.IsUpper(c)) return 660;
                    if (char.IsLower(c)) return 550;
                    return 550;
            }
        }

        public static int Main(string[] args)
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("  Lord of the Mysteries — Утилита нормализации TTF шрифта  ");
            Console.WriteLine("==========================================================");

            string defaultInput = @"d:\gameDev\NewBild\source_font\Aleo_TitleNew.ttf";
            if (!File.Exists(defaultInput))
            {
                defaultInput = @"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64\allin_data\font\Aleo_TitleNew.ttf";
            }
            string defaultOutput = @"d:\gameDev\NewBild\data\font\Aleo_TitleNew.ttf";

            string inputPath = args.Length > 0 ? args[0] : defaultInput;
            string outputPath = args.Length > 1 ? args[1] : defaultOutput;

            if (!File.Exists(inputPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ОШИБКА: Исходный файл шрифта не найден: " + inputPath);
                Console.ResetColor();
                return 1;
            }

            Console.WriteLine("Входной файл:   " + inputPath);
            Console.WriteLine("Выходной файл:  " + outputPath);

            byte[] fontData = File.ReadAllBytes(inputPath);
            int dataLen = fontData.Length;
            Console.WriteLine(string.Format("Размер шрифта:  {0:N0} байт", dataLen));

            ushort numTables = ReadU16(fontData, 4);
            var tables = new Dictionary<string, TableRecord>();

            for (int i = 0; i < numTables; i++)
            {
                int dirPos = 12 + i * 16;
                string tag = Encoding.ASCII.GetString(fontData, dirPos, 4);
                uint csum = ReadU32(fontData, dirPos + 4);
                uint offset = ReadU32(fontData, dirPos + 8);
                uint length = ReadU32(fontData, dirPos + 12);
                tables[tag] = new TableRecord
                {
                    Tag = tag,
                    DirOffset = dirPos,
                    Checksum = csum,
                    Offset = offset,
                    Length = length
                };
            }

            if (!tables.ContainsKey("cmap") || !tables.ContainsKey("hmtx") || !tables.ContainsKey("head") || !tables.ContainsKey("hhea"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ОШИБКА: В шрифте отсутствуют обязательные таблицы (cmap, hmtx, head, hhea).");
                Console.ResetColor();
                return 2;
            }

            TableRecord cmapRec = tables["cmap"];
            TableRecord hmtxRec = tables["hmtx"];
            TableRecord headRec = tables["head"];
            TableRecord hheaRec = tables["hhea"];

            ushort numberOfHMetrics = ReadU16(fontData, (int)(hheaRec.Offset + 34));
            Console.WriteLine("hhea.numberOfHMetrics: " + numberOfHMetrics);

            // Read cmap subtables to map Cyrillic characters (0x0400 - 0x04FF) to glyph IDs
            int cmapOffset = (int)cmapRec.Offset;
            ushort numSubtables = ReadU16(fontData, cmapOffset + 2);

            uint chosenSubtableOffset = 0;
            int chosenFormat = -1;

            for (int i = 0; i < numSubtables; i++)
            {
                int stDir = cmapOffset + 4 + i * 8;
                ushort platformID = ReadU16(fontData, stDir);
                ushort encodingID = ReadU16(fontData, stDir + 2);
                uint subOffset = ReadU32(fontData, stDir + 4);

                int stPos = (int)(cmapOffset + subOffset);
                ushort format = ReadU16(fontData, stPos);

                // Prefer Unicode format 12 or 4 (Platform 3: Windows, Platform 0: Unicode)
                if (format == 12 && (platformID == 3 || platformID == 0))
                {
                    chosenSubtableOffset = (uint)stPos;
                    chosenFormat = 12;
                    break;
                }
                if (format == 4 && (platformID == 3 || platformID == 0) && chosenFormat == -1)
                {
                    chosenSubtableOffset = (uint)stPos;
                    chosenFormat = 4;
                }
            }

            if (chosenSubtableOffset == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ОШИБКА: Не найдена подходящая Unicode подтаблица cmap.");
                Console.ResetColor();
                return 3;
            }

            Console.WriteLine(string.Format("Выбрана подтаблица cmap формата {0} по смещению {1}", chosenFormat, chosenSubtableOffset));

            var charToGlyph = new Dictionary<int, ushort>();

            if (chosenFormat == 12)
            {
                int pos = (int)chosenSubtableOffset + 12;
                uint numGroups = ReadU32(fontData, pos);
                pos += 4;
                for (uint g = 0; g < numGroups; g++)
                {
                    uint startCharCode = ReadU32(fontData, pos);
                    uint endCharCode = ReadU32(fontData, pos + 4);
                    uint startGlyphID = ReadU32(fontData, pos + 8);
                    pos += 12;

                    for (uint c = startCharCode; c <= endCharCode; c++)
                    {
                        if (c >= 0x0400 && c <= 0x04FF)
                        {
                            charToGlyph[(int)c] = (ushort)(startGlyphID + (c - startCharCode));
                        }
                    }
                }
            }
            else if (chosenFormat == 4)
            {
                int pos = (int)chosenSubtableOffset;
                ushort segCountX2 = ReadU16(fontData, pos + 6);
                ushort segCount = (ushort)(segCountX2 / 2);

                int endCodePos = pos + 14;
                int startCodePos = endCodePos + segCount * 2 + 2;
                int idDeltaPos = startCodePos + segCount * 2;
                int idRangeOffsetPos = idDeltaPos + segCount * 2;

                for (int s = 0; s < segCount; s++)
                {
                    ushort endCode = ReadU16(fontData, endCodePos + s * 2);
                    ushort startCode = ReadU16(fontData, startCodePos + s * 2);
                    short idDelta = ReadS16(fontData, idDeltaPos + s * 2);
                    ushort idRangeOffset = ReadU16(fontData, idRangeOffsetPos + s * 2);

                    for (int c = startCode; c <= endCode; c++)
                    {
                        if (c == 0xFFFF) break;
                        if (c >= 0x0400 && c <= 0x04FF)
                        {
                            ushort glyphIndex = 0;
                            if (idRangeOffset == 0)
                            {
                                glyphIndex = (ushort)((c + idDelta) & 0xFFFF);
                            }
                            else
                            {
                                int glyphOffset = idRangeOffsetPos + s * 2 + idRangeOffset + (c - startCode) * 2;
                                ushort val = ReadU16(fontData, glyphOffset);
                                if (val != 0) glyphIndex = (ushort)((val + idDelta) & 0xFFFF);
                            }
                            charToGlyph[c] = glyphIndex;
                        }
                    }
                }
            }

            Console.WriteLine(string.Format("Найдено символов кириллицы в cmap: {0}", charToGlyph.Count));
            if (charToGlyph.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ОШИБКА: В шрифте не найдены символы кириллицы диапазона 0x0400-0x04FF.");
                Console.ResetColor();
                return 4;
            }

            // Normalizing Cyrillic glyphs in hmtx
            int patchedCount = 0;
            var processedGids = new HashSet<ushort>();

            Console.WriteLine("\nНормализация метрик hmtx (замена advance width 1000 -> пропорциональные значения):");

            foreach (var kvp in charToGlyph)
            {
                char ch = (char)kvp.Key;
                ushort gid = kvp.Value;

                if (gid >= numberOfHMetrics) continue;
                if (processedGids.Contains(gid)) continue;
                processedGids.Add(gid);

                int metricOffset = (int)(hmtxRec.Offset + gid * 4);
                ushort oldAdv = ReadU16(fontData, metricOffset);
                short lsb = ReadS16(fontData, metricOffset + 2);

                ushort newAdv = GetProportionalAdvance(ch);

                WriteU16(fontData, metricOffset, newAdv);
                patchedCount++;

                if (patchedCount <= 10 || patchedCount >= charToGlyph.Count - 5)
                {
                    Console.WriteLine(string.Format("  '{0}' (U+{1:X4}): GID={2,-4} adv: {3} -> {4} (lsb={5} сохранено)",
                        ch, (int)ch, gid, oldAdv, newAdv, lsb));
                }
                else if (patchedCount == 11)
                {
                    Console.WriteLine("  ... [остальные символы алфавита] ...");
                }
            }

            Console.WriteLine(string.Format("\nУспешно нормализовано {0} глифов кириллицы!", patchedCount));

            // Recalculate checksum of hmtx table
            uint newHmtxChecksum = CalcTableChecksum(fontData, hmtxRec.Offset, hmtxRec.Length);
            WriteU32(fontData, hmtxRec.DirOffset + 4, newHmtxChecksum);
            Console.WriteLine(string.Format("Новая контрольная сумма таблицы hmtx: 0x{0:X8} (было 0x{1:X8})", newHmtxChecksum, hmtxRec.Checksum));

            // Recalculate whole-font checkSumAdjustment in 'head' table
            int headAdjPos = (int)headRec.Offset + 8;
            WriteU32(fontData, headAdjPos, 0); // Clear adjustment

            uint wholeFontSum = 0;
            uint totalLongs = ((uint)dataLen + 3) / 4;
            for (uint i = 0; i < totalLongs; i++)
            {
                uint pos = i * 4;
                uint b0 = pos < fontData.Length ? fontData[pos] : (uint)0;
                uint b1 = pos + 1 < fontData.Length ? fontData[pos + 1] : (uint)0;
                uint b2 = pos + 2 < fontData.Length ? fontData[pos + 2] : (uint)0;
                uint b3 = pos + 3 < fontData.Length ? fontData[pos + 3] : (uint)0;
                wholeFontSum += (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
            }

            uint checkSumAdjustment = 0xB1B0AFBA - wholeFontSum;
            WriteU32(fontData, headAdjPos, checkSumAdjustment);
            Console.WriteLine(string.Format("Новый head.checkSumAdjustment: 0x{0:X8}", checkSumAdjustment));

            // Verify checksum integrity
            uint verifySum = 0;
            for (uint i = 0; i < totalLongs; i++)
            {
                uint pos = i * 4;
                uint b0 = pos < fontData.Length ? fontData[pos] : (uint)0;
                uint b1 = pos + 1 < fontData.Length ? fontData[pos + 1] : (uint)0;
                uint b2 = pos + 2 < fontData.Length ? fontData[pos + 2] : (uint)0;
                uint b3 = pos + 3 < fontData.Length ? fontData[pos + 3] : (uint)0;
                verifySum += (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
            }

            if (verifySum != 0xB1B0AFBA)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("ОШИБКА: Верификация контрольной суммы шрифта провалена (0x{0:X8} != 0xB1B0AFBA)", verifySum));
                Console.ResetColor();
                return 5;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ Верификация контрольной суммы шрифта: 0xB1B0AFBA (PASSED)");
            Console.ResetColor();

            // Save patched font
            string outDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            File.WriteAllBytes(outputPath, fontData);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("✔ Пропатченный шрифт успешно сохранён: {0} ({1:N0} байт)", outputPath, fontData.Length));
            Console.ResetColor();

            return 0;
        }
    }
}
