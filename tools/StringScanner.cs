using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        byte[] bytes = File.ReadAllBytes("Lord-of-Mysteries-English-Patch-2.6.exe");
        string s = Encoding.ASCII.GetString(bytes);

        // Find interesting strings
        string[] patterns = new string[] {
            @"github\.com[^\x00\r\n\t ]+",
            @"Lani[^\x00\r\n\t ]+",
            @"v2\.[0-9]+[^\x00\r\n\t ]*",
            @"2\.6[^\x00\r\n\t ]*",
            @"[a-zA-Z0-9_\-\\]+\.lua",
            @"[a-zA-Z0-9_\-\\]+\.pak",
            @"[a-zA-Z0-9_\-\\]+\.zip",
            @"[a-zA-Z0-9_\-\\]+\.tar",
            @"cpdd[^\x00\r\n\t ]*",
            @"RuntimeText[^\x00\r\n\t ]*"
        };

        foreach (var p in patterns)
        {
            var matches = Regex.Matches(s, p, RegexOptions.IgnoreCase);
            Console.WriteLine("Pattern " + p + ": " + matches.Count + " matches");
            int shown = 0;
            foreach (Match m in matches)
            {
                if (shown++ < 5) Console.WriteLine("   " + m.Value);
            }
        }
    }
}
