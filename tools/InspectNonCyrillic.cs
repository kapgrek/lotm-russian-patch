using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class InspectNonCyrillic
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
        var nonCyrillic = new List<Tuple<string, string, string>>();

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

                        if (ru != null && !cyrillicRegex.IsMatch(ru))
                        {
                            nonCyrillic.Add(Tuple.Create(cn, en, ru));
                        }
                    }
                }
            }
        }

        Console.WriteLine("Total Non-Cyrillic entries: " + nonCyrillic.Count);
        for (int i = 0; i < Math.Min(30, nonCyrillic.Count); i++)
        {
            var item = nonCyrillic[i];
            Console.WriteLine(string.Format("{0}: CN=[{1}] | EN=[{2}] | RU=[{3}]", i + 1, item.Item1, item.Item2, item.Item3));
        }
    }
}
