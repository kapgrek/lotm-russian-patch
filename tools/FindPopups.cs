using System;
using System.IO;
using System.Text;

class FindPopups
{
    static void Main()
    {
        string pak = @"D:\Games\GMZZLauncher\Game\C7\Content\Paks\pakchunk0-Windows.pak";
        using (var fs = File.OpenRead(pak))
        {
            byte[] buf = new byte[1024 * 1024 * 8];
            long pos = 0;
            long len = fs.Length;
            while (pos < len)
            {
                int r = fs.Read(buf, 0, buf.Length);
                if (r <= 0) break;
                string s = Encoding.ASCII.GetString(buf, 0, r);
                foreach (string term in new string[] { "PopUp", "Notice", "Tips", "Dialog" })
                {
                    int idx = 0;
                    while ((idx = s.IndexOf(term, idx, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        int start = Math.Max(0, idx - 40);
                        int l = Math.Min(s.Length - start, 90);
                        string snip = s.Substring(start, l).Replace("\0", " ").Replace("\r", " ").Replace("\n", " ");
                        if (snip.Contains("Gameplay.LogicSystem") || snip.Contains("LogicSystem") || snip.Contains("_Panel.lua"))
                        {
                            Console.WriteLine("Found: " + snip);
                        }
                        idx += term.Length + 50;
                    }
                }
                pos += r - 1024;
                if (r < buf.Length) break;
            }
        }
    }
}
