using System;
using System.IO;
using System.Text;

class FastSearch
{
    static void Main()
    {
        string exe = @"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64\C7-Win64-Shipping.exe";
        long len = new FileInfo(exe).Length;
        Console.WriteLine("Exe size: " + (len / 1024 / 1024) + " MB");

        string[] terms = new string[] { "allin_data", "Aleo_TitleNew", "FZFW", "ZhuZi", "HYQiHei", "SourceHanSans" };
        byte[][] patterns = new byte[terms.Length][];
        for (int i = 0; i < terms.Length; i++) patterns[i] = Encoding.ASCII.GetBytes(terms[i]);

        using (var fs = File.OpenRead(exe))
        {
            byte[] buf = new byte[1024 * 1024 * 16]; // 16MB buffer
            long filePos = 0;
            while (filePos < len)
            {
                fs.Seek(filePos, SeekOrigin.Begin);
                int read = fs.Read(buf, 0, buf.Length);
                if (read <= 0) break;

                for (int t = 0; t < terms.Length; t++)
                {
                    byte[] pat = patterns[t];
                    for (int i = 0; i <= read - pat.Length; i++)
                    {
                        bool match = true;
                        for (int j = 0; j < pat.Length; j++)
                        {
                            if (buf[i + j] != pat[j]) { match = false; break; }
                        }
                        if (match)
                        {
                            Console.WriteLine(string.Format("Found '{0}' at offset 0x{1:X}", terms[t], filePos + i));
                        }
                    }
                }

                filePos += read - 1024; // advance with overlap
                if (read < buf.Length) break;
            }
        }
        Console.WriteLine("Search complete.");
    }
}
