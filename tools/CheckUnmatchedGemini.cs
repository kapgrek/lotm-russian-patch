using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class CheckUnmatchedGemini
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string ruPath = @"RuntimeTextRussian.lua";
        string geminiPath = @"source_en\RuntimeTextGemini.lua";

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

                        try {
                            ruDict[Regex.Unescape(k)] = Regex.Unescape(v);
                        } catch {
                            ruDict[k] = v;
                        }
                    }
                }
            }
        }
        Console.WriteLine("Loaded ruDict: " + ruDict.Count);

        var unmatchedList = new List<KeyValuePair<string, string>>();
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
                        string cnKeyEscaped = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string enValEscaped = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        string cnRaw = cnKeyEscaped;
                        string enRaw = enValEscaped;
                        try { cnRaw = Regex.Unescape(cnKeyEscaped); } catch {}
                        try { enRaw = Regex.Unescape(enValEscaped); } catch {}

                        string ruVal;
                        if (!ruDict.TryGetValue(cnRaw, out ruVal) || string.IsNullOrWhiteSpace(ruVal))
                        {
                            if (!ruDict.TryGetValue(enRaw, out ruVal) || string.IsNullOrWhiteSpace(ruVal))
                            {
                                unmatchedList.Add(new KeyValuePair<string, string>(cnRaw, enRaw));
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("Total unmatched in Gemini: " + unmatchedList.Count);

        using (var writer = new StreamWriter(@"tools\unmatched_gemini_sample.txt", false, Encoding.UTF8))
        {
            for (int i = 0; i < Math.Min(100, unmatchedList.Count); i++)
            {
                writer.WriteLine(string.Format("[{0}] CN: {1}", i + 1, unmatchedList[i].Key));
                writer.WriteLine(string.Format("    EN: {0}\n", unmatchedList[i].Value));
            }
        }
    }
}
