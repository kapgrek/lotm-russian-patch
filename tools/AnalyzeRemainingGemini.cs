using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class AnalyzeRemainingGemini
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var existingRu = new HashSet<string>(StringComparer.Ordinal);
        if (File.Exists(RuPath))
        {
            foreach (var line in File.ReadAllLines(RuPath, Encoding.UTF8))
            {
                string t = line.Trim();
                if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 0)
                    {
                        existingRu.Add(t.Substring(2, delim - 2));
                    }
                }
            }
        }
        Console.WriteLine("Existing entries in Ru dictionary: " + existingRu.Count);

        int totalLines = 0;
        int alreadyRu = 0;
        int untranslated = 0;
        int pureNumbers = 0;
        int debugOrConfig = 0;
        int englishText = 0;
        int containsChinese = 0;

        var samplesNonDebug = new List<Tuple<string, string>>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        using (var reader = new StreamReader(GeminiPath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string t = line.Trim();
                if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
                {
                    totalLines++;
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 0)
                    {
                        string cnKey = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string enVal = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        if (existingRu.Contains(cnKey) || existingRu.Contains(enVal) || seen.Contains(cnKey) || seen.Contains(enVal))
                        {
                            alreadyRu++;
                            continue;
                        }

                        untranslated++;
                        seen.Add(cnKey);
                        seen.Add(enVal);

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                        {
                            pureNumbers++;
                            continue;
                        }

                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi"))
                        {
                            debugOrConfig++;
                            continue;
                        }

                        if (Regex.IsMatch(enVal, @"[\u4e00-\u9fff]"))
                        {
                            containsChinese++;
                        }
                        else
                        {
                            englishText++;
                            if (samplesNonDebug.Count < 50)
                            {
                                samplesNonDebug.Add(Tuple.Create(cnKey, enVal));
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Total in Gemini: {0}", totalLines));
        Console.WriteLine(string.Format("Already in Ru: {0}", alreadyRu));
        Console.WriteLine(string.Format("Untranslated: {0}", untranslated));
        Console.WriteLine(string.Format("  - Pure Numbers: {0}", pureNumbers));
        Console.WriteLine(string.Format("  - Debug/Config IDs: {0}", debugOrConfig));
        Console.WriteLine(string.Format("  - Contains Chinese in value: {0}", containsChinese));
        Console.WriteLine(string.Format("  - English non-debug text: {0}", englishText));

        Console.WriteLine("\n--- Sample English non-debug untranslated (First 30) ---");
        for (int i = 0; i < Math.Min(30, samplesNonDebug.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] EN: {1} | CN: {2}", i, samplesNonDebug[i].Item2, samplesNonDebug[i].Item1));
        }
    }
}
