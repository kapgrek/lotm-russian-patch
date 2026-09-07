using System;
using System.IO;
using System.Text;

class FindFontRefs
{
    static void Main()
    {
        string dir = @"D:\Games\GMZZLauncher\Game\C7\Binaries\Win64";
        byte[] p = Encoding.ASCII.GetBytes("allin_data/font");
        byte[] pBack = Encoding.ASCII.GetBytes("allin_data\\font");
        byte[] pAleo = Encoding.ASCII.GetBytes("Aleo_TitleNew");

        foreach (var f in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
        {
            if (f.EndsWith(".ttf") || f.EndsWith(".otf")) continue;
            try
            {
                byte[] d = File.ReadAllBytes(f);
                if (Contains(d, p) || Contains(d, pBack) || Contains(d, pAleo))
                {
                    Console.WriteLine("Found reference in: " + f);
                }
            }
            catch {}
        }
    }

    static bool Contains(byte[] s, byte[] p)
    {
        for (int i = 0; i <= s.Length - p.Length; i++)
        {
            bool m = true;
            for (int j = 0; j < p.Length; j++) if (s[i + j] != p[j]) { m = false; break; }
            if (m) return true;
        }
        return false;
    }
}
