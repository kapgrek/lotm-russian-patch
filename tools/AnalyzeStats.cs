using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class AnalyzeStats
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string geminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

        var ruKeys = new HashSet<string>(StringComparer.Ordinal);
        if (File.Exists(ruPath))
        {
            using (var reader = new StreamReader(ruPath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string t = line.Trim();
                    if (t.StartsWith("[\""))
                    {
                        int delim = t.IndexOf("\"] = \"");
                        if (delim > 2)
                        {
                            ruKeys.Add(t.Substring(2, delim - 2));
                        }
                    }
                }
            }
        }

        int totalGemini = 0;
        int translatedFromGemini = 0;
        int untranslatedGemini = 0;

        using (var reader = new StreamReader(geminiPath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string t = line.Trim();
                if (t.StartsWith("[\""))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 2)
                    {
                        totalGemini++;
                        string cnKey = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string enVal = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        if (ruKeys.Contains(cnKey) || ruKeys.Contains(enVal))
                        {
                            translatedFromGemini++;
                        }
                        else
                        {
                            untranslatedGemini++;
                        }
                    }
                }
            }
        }

        Console.WriteLine("Total unique entries in Gemini source: " + totalGemini);
        Console.WriteLine("Real translated unique strings from Gemini: " + translatedFromGemini);
        Console.WriteLine("Remaining untranslated in Gemini: " + untranslatedGemini);
        Console.WriteLine("Total entries in RuntimeTextRussian.lua: " + ruKeys.Count);
        double pct = totalGemini > 0 ? (100.0 * translatedFromGemini / totalGemini) : 0;
        Console.WriteLine("Percentage: " + pct.ToString("F2") + "%");
    }
}
