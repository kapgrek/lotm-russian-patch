using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class CheckFont
{
    static ushort ReadUShort(byte[] d, int o) { return (ushort)((d[o] << 8) | d[o + 1]); }
    static uint ReadUInt(byte[] d, int o) { return (uint)((d[o] << 24) | (d[o + 1] << 16) | (d[o + 2] << 8) | d[o + 3]); }
    static short ReadShort(byte[] d, int o) { return (short)((d[o] << 8) | d[o + 1]); }

    static void Check(string path)
    {
        Console.WriteLine("\n=== Checking " + Path.GetFileName(path) + " ===");
        byte[] data = File.ReadAllBytes(path);
        int numTables = ReadUShort(data, 4);
        int cmapOffset = 0, hmtxOffset = 0, hheaOffset = 0;
        for (int i = 0; i < numTables; i++)
        {
            int o = 12 + i * 16;
            string tag = Encoding.ASCII.GetString(data, o, 4);
            int to = (int)ReadUInt(data, o + 8);
            if (tag == "cmap") cmapOffset = to;
            if (tag == "hmtx") hmtxOffset = to;
            if (tag == "hhea") hheaOffset = to;
        }

        if (cmapOffset == 0 || hmtxOffset == 0 || hheaOffset == 0)
        {
            Console.WriteLine("CFF or missing TTF tables (e.g. otf CFF)");
            return;
        }

        int numOfLongHorMetrics = ReadUShort(data, hheaOffset + 34);
        ushort numSubtables = ReadUShort(data, cmapOffset + 2);
        Dictionary<int, int> charToGlyph = new Dictionary<int, int>();

        for (int i = 0; i < numSubtables; i++)
        {
            int subOffset = cmapOffset + (int)ReadUInt(data, cmapOffset + 4 + i * 8 + 4);
            ushort format = ReadUShort(data, subOffset);
            if (format == 4)
            {
                ushort segCountX2 = ReadUShort(data, subOffset + 6);
                int segCount = segCountX2 / 2;
                int endCodeOffset = subOffset + 14;
                int startCodeOffset = endCodeOffset + 2 + segCountX2;
                int idDeltaOffset = startCodeOffset + segCountX2;
                int idRangeOffset = idDeltaOffset + segCountX2;

                for (int s = 0; s < segCount; s++)
                {
                    ushort endCode = ReadUShort(data, endCodeOffset + s * 2);
                    ushort startCode = ReadUShort(data, startCodeOffset + s * 2);
                    short idDelta = ReadShort(data, idDeltaOffset + s * 2);
                    ushort idRange = ReadUShort(data, idRangeOffset + s * 2);
                    if (startCode == 0xFFFF) break;

                    for (int c = startCode; c <= endCode; c++)
                    {
                        int glyphId = 0;
                        if (idRange == 0) glyphId = (c + idDelta) & 0xFFFF;
                        else
                        {
                            int glyphOffset = idRangeOffset + s * 2 + idRange + (c - startCode) * 2;
                            glyphId = ReadUShort(data, glyphOffset);
                            if (glyphId != 0) glyphId = (glyphId + idDelta) & 0xFFFF;
                        }
                        if (glyphId != 0 && !charToGlyph.ContainsKey(c)) charToGlyph[c] = glyphId;
                    }
                }
            }
        }

        int[] testChars = new int[] { 'a', 0x0430, 0x043E };
        foreach (int c in testChars)
        {
            if (charToGlyph.ContainsKey(c))
            {
                int gid = charToGlyph[c];
                int advanceWidth = (gid < numOfLongHorMetrics) ? ReadUShort(data, hmtxOffset + gid * 4) : ReadUShort(data, hmtxOffset + (numOfLongHorMetrics - 1) * 4);
                Console.WriteLine(string.Format("Char U+{0:X4}: GlyphID={1}, AdvanceWidth={2}", c, gid, advanceWidth));
            }
        }
    }

    static void Main(string[] args)
    {
        string dir = @"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64\allin_data\font";
        foreach (var file in Directory.GetFiles(dir))
        {
            try { Check(file); } catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
    }
}
