using System;
using System.IO;
using System.Text;

class CheckExe
{
    static void Main()
    {
        string exe = @"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64\C7-Win64-Shipping.exe";
        using (var fs = File.OpenRead(exe))
        {
            byte[] buf = new byte[1024 * 1024 * 8]; // 8MB buffer
            long pos = 0;
            int bytesRead;
            while ((bytesRead = fs.Read(buf, 0, buf.Length)) > 0)
            {
                string sAscii = Encoding.ASCII.GetString(buf, 0, bytesRead);
                foreach (string term in new string[] { "allin_data", "Aleo_Title", "HYQiHei", "SourceHanSans" })
                {
                    int idx = 0;
                    while ((idx = sAscii.IndexOf(term, idx, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        int start = Math.Max(0, idx - 40);
                        int len = Math.Min(sAscii.Length - start, 100);
                        Console.WriteLine("Found " + term + " at file offset " + (pos + idx) + ": " + sAscii.Substring(start, len).Replace("\0", " "));
                        idx += term.Length;
                    }
                }
                pos += bytesRead - 200; // overlap
                fs.Seek(pos, SeekOrigin.Begin);
            }
        }
    }
}
