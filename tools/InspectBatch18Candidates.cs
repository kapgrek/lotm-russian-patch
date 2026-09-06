using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectBatch18Candidates
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

    static Dictionary<string, int> categories = new Dictionary<string, int>();
    static Dictionary<string, List<Tuple<string, string>>> categorySamples = new Dictionary<string, List<Tuple<string, string>>>();

    static void AddCandidate(string cat, string cn, string en)
    {
        if (!categories.ContainsKey(cat))
        {
            categories[cat] = 0;
            categorySamples[cat] = new List<Tuple<string, string>>();
        }
        categories[cat]++;
        if (categorySamples[cat].Count < 10)
        {
            categorySamples[cat].Add(Tuple.Create(cn, en));
        }
    }

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
                        string k = t.Substring(2, delim - 2);
                        existingRu.Add(k);
                    }
                }
            }
        }
        Console.WriteLine("Existing entries in Ru: " + existingRu.Count);

        var seen = new HashSet<string>(StringComparer.Ordinal);

        using (var reader = new StreamReader(GeminiPath, Encoding.UTF8))
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
                        string cnKey = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string enVal = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        if (existingRu.Contains(cnKey) || existingRu.Contains(enVal) || seen.Contains(cnKey) || seen.Contains(enVal))
                            continue;

                        seen.Add(cnKey);
                        seen.Add(enVal);

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        // Check categories
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") ||
                            enVal.StartsWith(".p4config") || enVal.Contains("-----") || enVal.Contains(".cpp") || enVal.Contains(".h") ||
                            enVal.StartsWith("523") || enVal.StartsWith("520") || enVal.StartsWith("GTA-") || enVal.StartsWith("GTA ") ||
                            enVal.StartsWith("Camera_") || enVal.StartsWith("Mat_") || enVal.StartsWith("SK_") || enVal.StartsWith("SM_") ||
                            enVal.StartsWith("SkillID: ") || enVal.Contains("InterruptMode") || enVal.Contains("BindSkillID") ||
                            Regex.IsMatch(enVal, @"^\d+:\s*(Disable|Enable|Hide)"))
                        {
                            AddCandidate("EngineDebugTechnical", cnKey, enVal);
                        }
                        else if (enVal.Length > 25 && (enVal.Contains(" ") || enVal.Contains("\"") || enVal.Contains("!") || enVal.Contains("?") || enVal.Contains("...") || enVal.Contains("—")))
                        {
                            AddCandidate("NarrativeAndSentences", cnKey, enVal);
                        }
                        else if (enVal.Contains("HyperLink") || enVal.Contains("damage") || enVal.Contains("Damage") || enVal.Contains("Cooldown") || enVal.Contains("Slash"))
                        {
                            AddCandidate("SkillsAndCombat", cnKey, enVal);
                        }
                        else if (enVal.Length >= 3 && enVal.Length <= 40 && !enVal.Contains("_") && !enVal.Contains("/"))
                        {
                            AddCandidate("UIAndShortLabels", cnKey, enVal);
                        }
                        else
                        {
                            AddCandidate("OtherRemaining", cnKey, enVal);
                        }
                    }
                }
            }
        }

        Console.WriteLine("\n=== Summary of Categories ===");
        foreach (var kvp in categories)
        {
            Console.WriteLine(string.Format("{0}: {1}", kvp.Key, kvp.Value));
        }

        Console.WriteLine("\n=== Samples ===");
        foreach (var kvp in categorySamples)
        {
            Console.WriteLine("\nCategory: " + kvp.Key);
            foreach (var s in kvp.Value)
            {
                Console.WriteLine("  EN: " + s.Item2 + " | CN: " + s.Item1);
            }
        }
    }
}
