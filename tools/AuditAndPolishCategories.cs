using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class AuditAndPolishCategories
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string catPath = @"d:\gameDev\translate lotm\tools\patch_2.6_untranslated_categories.txt";
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        using (var reader = new StreamReader(ruPath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string t = line.Trim();
                if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 0)
                    {
                        string k = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string v = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";
                        ruDict[Unescape(k)] = Unescape(v);
                    }
                }
            }
        }
        Console.WriteLine("Loaded " + ruDict.Count + " entries from RuntimeTextRussian.lua.");

        // Read categories
        var catLines = File.ReadAllLines(catPath, Encoding.UTF8);
        string currentCategory = "";
        string curCn = "", curEn = "";
        int checkedEntries = 0;
        int untranslatedInCat = 0;
        int canonIssues = 0;

        Action check = () =>
        {
            if (string.IsNullOrEmpty(curCn)) return;
            checkedEntries++;
            string ruVal = null;
            if (ruDict.TryGetValue(curCn, out ruVal) || ruDict.TryGetValue(curEn, out ruVal))
            {
                if (!Regex.IsMatch(ruVal, @"[\u0400-\u04FF]") && !Regex.IsMatch(curEn.Trim(), @"^[\d\s\.,:%/\\~_\-\+\*\(\)\#\[\]\|]+$"))
                {
                    untranslatedInCat++;
                }
                if (ruVal.Contains("Кляйн") || ruVal.Contains("солей") || ruVal.Contains("Блэкторн") || ruVal.Contains("Старый Нил") || ruVal.Contains("Потусторонняя характеристика"))
                {
                    canonIssues++;
                }
            }
            else
            {
                untranslatedInCat++;
            }
        };

        foreach (var l in catLines)
        {
            if (l.StartsWith("=== "))
            {
                currentCategory = l;
                continue;
            }
            var mIdx = Regex.Match(l, @"^\[\d+\]\s*CN:\s*(.*)");
            if (mIdx.Success)
            {
                check();
                curCn = Unescape(mIdx.Groups[1].Value.Trim());
                curEn = "";
                continue;
            }
            var mEn = Regex.Match(l, @"^\s*EN:\s*(.*)");
            if (mEn.Success)
            {
                curEn = Unescape(mEn.Groups[1].Value.Trim());
                continue;
            }
        }
        check();

        Console.WriteLine(string.Format("Total categorized entries checked: {0}", checkedEntries));
        Console.WriteLine(string.Format("Untranslated (no Cyrillic): {0}", untranslatedInCat));
        Console.WriteLine(string.Format("Canon glossary issues: {0}", canonIssues));
    }

    static string Unescape(string s)
    {
        return s.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\\", "\\");
    }
}
