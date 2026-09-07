using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class InspectCyrillicGlyphs
{
    static ushort ReadUShort(byte[] d, int o) { return (ushort)((d[o] << 8) | d[o + 1]); }
    static uint ReadUInt(byte[] d, int o) { return (uint)((d[o] << 24) | (d[o + 1] << 16) | (d[o + 2] << 8) | d[o + 3]); }
    static short ReadShort(byte[] d, int o) { return (short)((d[o] << 8) | d[o + 1]); }

    static void Main()
    {
        string path = @"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64\allin_data\font\Aleo_TitleNew.ttf";
        byte[] data = File.ReadAllBytes(path);
        int numTables = ReadUShort(data, 4);
        int cmapOffset = 0, hmtxOffset = 0, hheaOffset = 0, glyfOffset = 0, locaOffset = 0, headOffset = 0;
        for (int i = 0; i < numTables; i++)
        {
            int o = 12 + i * 16;
            string tag = Encoding.ASCII.GetString(data, o, 4);
            int to = (int)ReadUInt(data, o + 8);
            if (tag == "cmap") cmapOffset = to;
            if (tag == "hmtx") hmtxOffset = to;
            if (tag == "hhea") hheaOffset = to;
            if (tag == "glyf") glyfOffset = to;
            if (tag == "loca") locaOffset = to;
            if (tag == "head") headOffset = to;
        }

        short indexToLocFormat = ReadShort(data, headOffset + 50);
        int numOfLongHorMetrics = ReadUShort(data, hheaOffset + 34);

        // Parse cmap format 4
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
                        int glyphId = (idRange == 0) ? ((c + idDelta) & 0xFFFF) : ReadUShort(data, idRangeOffset + s * 2 + idRange + (c - startCode) * 2);
                        if (idRange != 0 && glyphId != 0) glyphId = (glyphId + idDelta) & 0xFFFF;
                        if (glyphId != 0 && !charToGlyph.ContainsKey(c)) charToGlyph[c] = glyphId;
                    }
                }
            }
        }

        Console.WriteLine("Cyrillic Glyphs in Aleo_TitleNew.ttf:");
        for (int c = 0x0410; c <= 0x044F; c++)
        {
            if (charToGlyph.ContainsKey(c))
            {
                int gid = charToGlyph[c];
                int adv = ReadUShort(data, hmtxOffset + gid * 4);
                short lsb = ReadShort(data, hmtxOffset + gid * 4 + 2);
                
                // Get glyph bounding box from glyf table
                int glyphOffset = 0;
                if (indexToLocFormat == 0) glyphOffset = ReadUShort(data, locaOffset + gid * 2) * 2;
                else glyphOffset = (int)ReadUInt(data, locaOffset + gid * 4);
                
                short xMin = ReadShort(data, glyfOffset + glyphOffset + 2);
                short yMin = ReadShort(data, glyfOffset + glyphOffset + 4);
                short xMax = ReadShort(data, glyfOffset + glyphOffset + 6);
                short yMax = ReadShort(data, glyfOffset + glyphOffset + 8);
                int glyphWidth = xMax - xMin;

                Console.WriteLine(string.Format("U+{0:X4} (gid {1,4}): adv={2,4}, lsb={3,4}, bbox=[{4,4}..{5,4}], glyphWidth={6,4}",
                    c, gid, adv, lsb, xMin, xMax, glyphWidth));
            }
        }
    }
}
