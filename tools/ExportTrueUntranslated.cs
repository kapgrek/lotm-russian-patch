using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class ExportTrueUntranslated
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string geminiPath = @"source_en\RuntimeTextGemini.lua";
        string ruPath = @"RuntimeTextRussian.lua";

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
                        ruDict[k] = v;
                    }
                }
            }
        }

        var cyrillicRegex = new Regex(@"[\u0400-\u04FF]");
        var trueUntranslated = new List<Tuple<string, string, string>>();

        using (var reader = new StreamReader(geminiPath, Encoding.UTF8))
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
                        string cn = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string en = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        string ru = null;
                        if (ruDict.ContainsKey(cn)) ru = ruDict[cn];
                        else if (ruDict.ContainsKey(en)) ru = ruDict[en];

                        // If RU is missing or RU has no Cyrillic and EN has letters
                        bool hasEnLetters = Regex.IsMatch(en, @"[a-zA-Z]{2,}");
                        bool hasRuCyrillic = ru != null && cyrillicRegex.IsMatch(ru);

                        if (ru == null || (hasEnLetters && !hasRuCyrillic))
                        {
                            trueUntranslated.Add(Tuple.Create(cn, en, ru ?? ""));
                        }
                    }
                }
            }
        }

        Console.WriteLine("TRUE untranslated English strings remaining: " + trueUntranslated.Count);
        var sb = new StringBuilder();
        sb.AppendLine("ID\tCN\tEN\tCURRENT_RU");
        for (int i = 0; i < trueUntranslated.Count; i++)
        {
            var item = trueUntranslated[i];
            sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}", i + 1, item.Item1, item.Item2, item.Item3));
            if (i < 30)
            {
                Console.WriteLine(string.Format("#{0}: EN=[{1}] | RU=[{2}]", i + 1, item.Item2.Length > 40 ? item.Item2.Substring(0, 40) + "..." : item.Item2, item.Item3.Length > 40 ? item.Item3.Substring(0, 40) + "..." : item.Item3));
            }
        }

        File.WriteAllText(@"tools\true_untranslated_strings.tsv", sb.ToString(), Encoding.UTF8);
        Console.WriteLine("Exported to tools\\true_untranslated_strings.tsv");
    }
}
