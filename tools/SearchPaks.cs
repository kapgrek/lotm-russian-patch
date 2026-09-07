using System;
using System.IO;
using System.Text;

class SearchPaks
{
    static void Main()
    {
        string paksDir = @"D:\Games\GMZZLauncher\Game\C7\Content\Paks";
        string[] terms = new string[] { ".ttf", ".otf", "Font", "Aleo", "ZhuZi", "Mincho", "QiHei", "Songti" };
        
        foreach (var file in Directory.GetFiles(paksDir, "*.pak"))
        {
            SearchFile(file, terms);
        }
        foreach (var file in Directory.GetFiles(paksDir, "*.upak"))
        {
            SearchFile(file, terms);
        }
    }

    static void SearchFile(string path, string[] terms)
    {
        long len = new FileInfo(path).Length;
        if (len > 500 * 1024 * 1024) return; // skip huge files for now

        Console.WriteLine("Scanning " + Path.GetFileName(path) + " (" + (len / 1024 / 1024) + " MB)...");
        using (var fs = File.OpenRead(path))
        {
            byte[] buf = new byte[1024 * 1024 * 8];
            long filePos = 0;
            while (filePos < len)
            {
                int read = fs.Read(buf, 0, buf.Length);
                if (read <= 0) break;
                string s = Encoding.ASCII.GetString(buf, 0, read);
                foreach (var term in terms)
                {
                    int idx = 0;
                    while ((idx = s.IndexOf(term, idx, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        int start = Math.Max(0, idx - 40);
                        int l = Math.Min(s.Length - start, 90);
                        string snippet = s.Substring(start, l).Replace("\0", " ").Replace("\r", " ").Replace("\n", " ");
                        Console.WriteLine("  [" + term + "]: " + snippet);
                        idx += term.Length + 50; // skip ahead
                    }
                }
                filePos += read - 1024;
                if (read < buf.Length) break;
            }
        }
    }
}
