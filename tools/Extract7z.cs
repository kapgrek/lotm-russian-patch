using System;
using System.IO;

class Program
{
    static void Main()
    {
        using (var fs = File.OpenRead("Lord-of-Mysteries-English-Patch-2.6.exe"))
        {
            byte[] header = new byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C };
            byte[] buf = new byte[65536];
            long pos = 0;
            int read;
            while ((read = fs.Read(buf, 0, buf.Length)) > 0)
            {
                for (int i = 0; i <= read - 6; i++)
                {
                    bool match = true;
                    for (int j = 0; j < 6; j++)
                    {
                        if (buf[i + j] != header[j]) { match = false; break; }
                    }
                    if (match)
                    {
                        Console.WriteLine("Found 7z header at offset: " + (pos + i));
                        // extract 7z stream to file
                        long offset = pos + i;
                        fs.Seek(offset, SeekOrigin.Begin);
                        using (var outFs = File.Create("english_2.6.7z"))
                        {
                            fs.CopyTo(outFs);
                        }
                        Console.WriteLine("Extracted 7z archive to english_2.6.7z (size: " + (fs.Length - offset) + " bytes)");
                        return;
                    }
                }
                pos += read - 5;
                fs.Seek(pos, SeekOrigin.Begin);
            }
        }
        Console.WriteLine("7z header not found");
    }
}
