using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class DeepTranslationResearch
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string geminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        string reportPath = @"d:\gameDev\translate lotm\tools\translation_audit_report.txt";
        string untranslatedDataPath = @"d:\gameDev\translate lotm\tools\untranslated_strings.tsv";

        Console.WriteLine("=== DEEP TRANSLATION AUDIT ===");

        // 1. Load Russian Dictionary
        Console.WriteLine("Loading RuntimeTextRussian.lua...");
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
        Console.WriteLine("Loaded Russian entries: " + ruDict.Count);

        // 2. Scan source_en/RuntimeTextGemini.lua
        Console.WriteLine("Scanning source_en/RuntimeTextGemini.lua...");
        int totalGemini = 0;
        int fullyFoundCn = 0;
        int fullyFoundEn = 0;
        int completelyMissing = 0;
        int identicalToEn = 0;
        int emptyTranslation = 0;
        int containsLatinOnly = 0;
        int containsCyrillic = 0;

        var missingList = new List<Tuple<string, string>>();
        var identicalList = new List<Tuple<string, string, string>>();

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
                        totalGemini++;
                        string cn = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string en = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        bool hasCn = ruDict.ContainsKey(cn);
                        bool hasEn = ruDict.ContainsKey(en);

                        if (hasCn) fullyFoundCn++;
                        if (hasEn) fullyFoundEn++;

                        string ru = null;
                        if (hasCn) ru = ruDict[cn];
                        else if (hasEn) ru = ruDict[en];

                        if (ru == null)
                        {
                            completelyMissing++;
                            missingList.Add(Tuple.Create(cn, en));
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(ru))
                            {
                                emptyTranslation++;
                            }
                            else if (ru.Trim() == en.Trim())
                            {
                                identicalToEn++;
                                if (identicalList.Count < 500)
                                    identicalList.Add(Tuple.Create(cn, en, ru));
                            }
                            else
                            {
                                bool hasRuLetters = Regex.IsMatch(ru, @"[\p{IsCyrillic}]");
                                if (hasRuLetters) containsCyrillic++;
                                else containsLatinOnly++;
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("\n--- AUDIT RESULTS ---");
        Console.WriteLine(string.Format("Total strings in Gemini base: {0}", totalGemini));
        Console.WriteLine(string.Format("Found by CN key in Russian dict: {0} ({1:F2}%)", fullyFoundCn, (fullyFoundCn * 100.0 / totalGemini)));
        Console.WriteLine(string.Format("Found by EN key in Russian dict: {0} ({1:F2}%)", fullyFoundEn, (fullyFoundEn * 100.0 / totalGemini)));
        Console.WriteLine(string.Format("Completely MISSING from Russian dict: {0}", completelyMissing));
        Console.WriteLine(string.Format("Identical to English (untranslated copies): {0}", identicalToEn));
        Console.WriteLine(string.Format("Empty or whitespace translations: {0}", emptyTranslation));
        Console.WriteLine(string.Format("Translations with Cyrillic letters: {0}", containsCyrillic));
        Console.WriteLine(string.Format("Translations without Cyrillic (numbers/symbols/latin only): {0}", containsLatinOnly));

        // Save report
        using (var sw = new StreamWriter(reportPath, false, Encoding.UTF8))
        {
            sw.WriteLine("=== LOTM TRANSLATION AUDIT REPORT ===");
            sw.WriteLine("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sw.WriteLine(string.Format("Total strings in Gemini base: {0}", totalGemini));
            sw.WriteLine(string.Format("Found by CN key in Russian dict: {0}", fullyFoundCn));
            sw.WriteLine(string.Format("Found by EN key in Russian dict: {0}", fullyFoundEn));
            sw.WriteLine(string.Format("Completely MISSING from Russian dict: {0}", completelyMissing));
            sw.WriteLine(string.Format("Identical to English (RU == EN): {0}", identicalToEn));
            sw.WriteLine(string.Format("Empty translations: {0}", emptyTranslation));
            sw.WriteLine(string.Format("Valid Cyrillic translations: {0}", containsCyrillic));
            sw.WriteLine(string.Format("Non-Cyrillic translations: {0}", containsLatinOnly));
            sw.WriteLine("\n=== SAMPLE IDENTICAL TO ENGLISH (UNTRANSLATED) ===");
            for (int i = 0; i < Math.Min(100, identicalList.Count); i++)
            {
                sw.WriteLine(string.Format("[CN] {0}  ==>  [EN] {1}", identicalList[i].Item1, identicalList[i].Item2));
            }
        }

        // Save all untranslated strings to TSV
        using (var sw = new StreamWriter(untranslatedDataPath, false, Encoding.UTF8))
        {
            sw.WriteLine("CN\tEN");
            foreach (var m in missingList)
            {
                sw.WriteLine(m.Item1 + "\t" + m.Item2);
            }
        }

        Console.WriteLine("Report written to: " + reportPath);
        Console.WriteLine("Missing strings saved to: " + untranslatedDataPath);
    }
}
