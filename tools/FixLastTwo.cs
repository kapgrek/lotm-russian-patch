using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class FixLastTwo
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        var lines = File.ReadAllLines(ruPath, Encoding.UTF8);
        var newLines = new List<string>(lines.Length);

        for (int i = 0; i < lines.Length; i++)
        {
            string l = lines[i];
            if (l.Contains("Give\\nyou\\nagain") || l.Contains("Give\nyou\nagain"))
            {
                l = l.Replace("Give\\nyou\\nagain", "Сно\\nва\\nте\\nбе").Replace("Give\nyou\nagain", "Сно\nва\nте\nбе");
            }
            if (l.Contains("W\\nh\\ny\\n?") || l.Contains("W\nh\ny\n?"))
            {
                l = l.Replace("W\\nh\\ny\\n?", "По\\nче\\nму\\n?").Replace("W\nh\ny\n?", "По\nче\nму\n?");
            }
            newLines.Add(l);
        }

        File.WriteAllLines(ruPath, newLines, Encoding.UTF8);
        Console.WriteLine("Done updating last 2 lines.");
    }
}
