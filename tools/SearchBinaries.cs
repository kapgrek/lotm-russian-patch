using System;
using System.IO;
using System.Text;

class SearchBinaries
{
    static void Main()
    {
        string dir = @"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64";
        byte[] p1 = Encoding.ASCII.GetBytes("allin_data");
        byte[] p2 = Encoding.Unicode.GetBytes("allin_data");
        byte[] p3 = Encoding.ASCII.GetBytes("Aleo");
        byte[] p4 = Encoding.Unicode.GetBytes("Aleo");

        foreach (var f in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
        {
            if (f.EndsWith(".ttf") || f.EndsWith(".otf") || f.EndsWith(".pak")) continue;
            try
            {
                byte[] d = File.ReadAllBytes(f);
                bool found = IndexOf(d, p1) != -1 || IndexOf(d, p2) != -1 || IndexOf(d, p3) != -1 || IndexOf(d, p4) != -1;
                if (found) Console.WriteLine("Found in: " + f);
            }
            catch {}
        }
    }

    static int IndexOf(byte[] src, byte[] pat)
    {
        for (int i = 0; i <= src.Length - pat.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < pat.Length; j++)
            {
                if (src[i + j] != pat[j]) { match = false; break; }
            }
            if (match) return i;
        }
        return -1;
    }
}
