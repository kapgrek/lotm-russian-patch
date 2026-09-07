using System;
using System.IO;
using System.Text;

class CheckFontName
{
    static ushort ReadUShort(byte[] d, int o) { return (ushort)((d[o] << 8) | d[o + 1]); }
    static uint ReadUInt(byte[] d, int o) { return (uint)((d[o] << 24) | (d[o + 1] << 16) | (d[o + 2] << 8) | d[o + 3]); }

    static void Main()
    {
        string path = @"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64\allin_data\font\Aleo_TitleNew.ttf";
        byte[] data = File.ReadAllBytes(path);
        int numTables = ReadUShort(data, 4);
        int nameOffset = 0;
        for (int i = 0; i < numTables; i++)
        {
            int o = 12 + i * 16;
            string tag = Encoding.ASCII.GetString(data, o, 4);
            if (tag == "name") nameOffset = (int)ReadUInt(data, o + 8);
        }

        if (nameOffset == 0) return;
        ushort count = ReadUShort(data, nameOffset + 2);
        ushort stringOffset = ReadUShort(data, nameOffset + 4);
        int storageOffset = nameOffset + stringOffset;

        for (int i = 0; i < count; i++)
        {
            int r = nameOffset + 6 + i * 12;
            ushort platformId = ReadUShort(data, r);
            ushort encodingId = ReadUShort(data, r + 2);
            ushort languageId = ReadUShort(data, r + 4);
            ushort nameId = ReadUShort(data, r + 6);
            ushort length = ReadUShort(data, r + 8);
            ushort offset = ReadUShort(data, r + 10);

            if (nameId == 1 || nameId == 4 || nameId == 6) // Family, Full name, PostScript name
            {
                string val = "";
                if (platformId == 3 || platformId == 0) // Unicode / Windows
                {
                    val = Encoding.BigEndianUnicode.GetString(data, storageOffset + offset, length);
                }
                else
                {
                    val = Encoding.ASCII.GetString(data, storageOffset + offset, length);
                }
                Console.WriteLine(string.Format("NameID {0} (plat={1}, lang={2}): {3}", nameId, platformId, languageId, val));
            }
        }
    }
}
