using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class AuditNonCyrillicResults
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string tsvPath = @"d:\gameDev\translate lotm\tools\non_cyrillic_translated.tsv";
        var lines = File.ReadAllLines(tsvPath, Encoding.UTF8);

        Console.WriteLine("Auditing translated entries...");
        var issues = new List<string>();

        for (int i = 1; i < lines.Length; i++)
        {
            var p = lines[i].Split('\t');
            if (p.Length < 5) continue;
            string id = p[0];
            string cn = p[1];
            string en = p[2];
            string ruNew = p[4];

            bool hasIssue = false;
            string reason = "";

            if (ruNew.Contains("abb") || ruNew.Contains(":bb") || ruNew.Contains(",b"))
            {
                hasIssue = true;
                reason = "corrupted number/placeholder (abb/:bb/,b)";
            }
            else if (!Regex.IsMatch(ruNew, @"[\u0400-\u04FF]") && !Regex.IsMatch(en.Trim(), @"^[\d\s\.,:%/\\~_\-\+\*\(\)\#\[\]\|]+$"))
            {
                hasIssue = true;
                reason = "no Cyrillic letters";
            }
            else if (ruNew.Contains("XTAG"))
            {
                hasIssue = true;
                reason = "unexpanded XTAG";
            }
            else if (ruNew.Contains("Кляйн") || ruNew.Contains("солей") || ruNew.Contains("Блэкторн") || ruNew.Contains("Старый Нил"))
            {
                hasIssue = true;
                reason = "canon violation";
            }

            if (hasIssue)
            {
                issues.Add(string.Format("[{0}] Reason: {1}\n  CN: {2}\n  EN: {3}\n  RU: {4}\n", id, reason, cn, en, ruNew));
            }
        }

        Console.WriteLine(string.Format("Total issues found: {0}", issues.Count));
        foreach (var iss in issues)
        {
            Console.WriteLine(iss);
        }
    }
}
