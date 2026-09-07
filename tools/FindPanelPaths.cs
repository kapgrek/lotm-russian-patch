using System;
using System.IO;
using System.Text;

class FindPanelPaths
{
    static void Main()
    {
        string paksDir = @D:\Games\GMZZLauncher\Game\C7\Content\Paks;
        string[] terms = new string[] { TaskBoard, Arcane, Offering, Strengthen, Enhance, EquipStrengthen, EquipEnhance, Task_Board };
        
        if (!Directory.Exists(paksDir))
        {
            Console.WriteLine(Paks dir not found:  + paksDir);
            return;
        }

        foreach (var file in Directory.GetFiles(paksDir, *.pak))
        {
            SearchFile(file, terms);
        }
        foreach (var file in Directory.GetFiles(paksDir, *.upak))
        {
            SearchFile(file, terms);
        }
    }

    static void SearchFile(string path, string[] terms)
    {
        long len = new FileInfo(path).Length;
        if (len > 300 * 1024 * 1024) return;

        using (var fs = File.OpenRead(path))
        {
            byte[] buf = new byte[1024 * 1024 * 4];
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
                        int start = Math.Max(0, idx - 60);
                        int l = Math.Min(s.Length - start, 140);
                        string snippet = s.Substring(start, l).Replace(\0,  ).Replace(\r,  ).Replace(\n,  );
                        if (snippet.Contains(Gameplay) || snippet.Contains(Panel) || snippet.Contains(LogicSystem) || snippet.Contains(WBP_))
                        {
                            Console.WriteLine([ + term + ]  + Path.GetFileName(path) +  @ 0x + (filePos + idx).ToString(X) + :  + snippet);
                        }
                        idx += term.Length + 50;
                    }
                }
                filePos += read - 1024;
                if (read < buf.Length) break;
            }
        }
    }
}
