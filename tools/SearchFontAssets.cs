using System;
using System.IO;
using System.Text;

class SearchFontAssets
{
    static void Main()
    {
        string paksDir = @"D:\Games\GMZZLauncher\Game\C7\Content\Paks";
        foreach (var file in Directory.GetFiles(paksDir, "*.*"))
        {
            if (file.EndsWith(".txt") || file.EndsWith(".utoc")) continue;
            long len = new FileInfo(file).Length;
            if (len > 700 * 1024 * 1024) continue; // check files <= 700MB

            using (var fs = File.OpenRead(file))
            {
                byte[] buf = new byte[1024 * 1024 * 4];
                long pos = 0;
                while (pos < len)
                {
                    int r = fs.Read(buf, 0, buf.Length);
                    if (r <= 0) break;
                    string s = Encoding.ASCII.GetString(buf, 0, r);
                    foreach (string term in new string[] { ".ttf", ".otf", "Aleo", "ZhuZi", "Mincho", "HYQiHei" })
                    {
                        int idx = 0;
                        while ((idx = s.IndexOf(term, idx, StringComparison.OrdinalIgnoreCase)) != -1)
                        {
                            int start = Math.Max(0, idx - 40);
                            int l = Math.Min(s.Length - start, 80);
                            string snip = s.Substring(start, l).Replace("\0", " ").Replace("\r", " ").Replace("\n", " ");
                            Console.WriteLine(Path.GetFileName(file) + " [" + term + "]: " + snip);
                            idx += term.Length + 100;
                        }
                    }
                    pos += r - 1024;
                    if (r < buf.Length) break;
                }
            }
        }
    }
}
