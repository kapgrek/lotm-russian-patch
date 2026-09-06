using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class AnalyzeDialogueStrings
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string LsiDir = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes";

    static string SourceKey(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        uint hash = 2166136261u;
        for (int i = 0; i < bytes.Length; i++)
        {
            hash ^= bytes[i];
            hash = unchecked(hash + (hash << 1) + (hash << 4) + (hash << 7) + (hash << 8) + (hash << 24));
        }
        return bytes.Length + ":" + hash.ToString("x8");
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 1. Load existing RU keys
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
        Console.WriteLine("Existing RU keys: " + existingRu.Count);

        // 2. Load LSI mappings
        var lsiTagMap = new Dictionary<string, List<string>>();
        if (Directory.Exists(LsiDir))
        {
            foreach (var file in Directory.GetFiles(LsiDir, "LanguageSourceIndex_*.lua"))
            {
                foreach (var line in File.ReadAllLines(file, Encoding.UTF8))
                {
                    int start = line.IndexOf("[\"");
                    if (start >= 0)
                    {
                        int end = line.IndexOf("\"]", start + 2);
                        if (end > start)
                        {
                            string hashKey = line.Substring(start + 2, end - start - 2);
                            string tag = line.Substring(end + 2).Trim();
                            if (!lsiTagMap.ContainsKey(hashKey))
                                lsiTagMap[hashKey] = new List<string>();
                            lsiTagMap[hashKey].Add(tag);
                        }
                    }
                }
            }
        }
        Console.WriteLine("LSI hash mappings loaded: " + lsiTagMap.Count);

        // 3. Scan Gemini entries and categorize
        int totalGemini = 0;
        int alreadyRu = 0;
        var categoryCounts = new Dictionary<string, int>();
        var untranslatedTingen = new List<Tuple<string, string, string>>();

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
                        totalGemini++;
                        string cnKey = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string enVal = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        bool isRu = existingRu.Contains(cnKey) || existingRu.Contains(enVal);
                        if (isRu)
                        {
                            alreadyRu++;
                            continue;
                        }

                        string sk = SourceKey(cnKey);
                        string tagSummary = "unknown";
                        if (lsiTagMap.ContainsKey(sk))
                        {
                            tagSummary = string.Join(";", lsiTagMap[sk].ToArray());
                        }

                        // Categorize
                        string cat = "other";
                        if (tagSummary.Contains("tingentalk") || tagSummary.Contains("tingen")) cat = "tingen";
                        else if (tagSummary.Contains("oldtalk")) cat = "oldtalk";
                        else if (tagSummary.Contains("talkother") || tagSummary.Contains("othertalk")) cat = "othertalk";
                        else if (tagSummary.Contains("talk")) cat = "talk";
                        else if (tagSummary.Contains("item")) cat = "item";
                        else if (tagSummary.Contains("quest") || tagSummary.Contains("task")) cat = "quest";
                        else if (tagSummary.Contains("book") || tagSummary.Contains("diary") || tagSummary.Contains("letter")) cat = "book/lore";

                        if (!categoryCounts.ContainsKey(cat)) categoryCounts[cat] = 0;
                        categoryCounts[cat]++;

                        if (cat == "tingen" || cat == "oldtalk" || cat == "talk" || cat == "othertalk" || enVal.Contains("Klein") || enVal.Contains("Melissa") || enVal.Contains("Benson") || enVal.Contains("Dunn") || enVal.Contains("Nighthawk"))
                        {
                            untranslatedTingen.Add(Tuple.Create(cnKey, enVal, tagSummary));
                        }
                    }
                }
            }
        }

        Console.WriteLine("Total Gemini entries: " + totalGemini);
        Console.WriteLine("Already translated in RU: " + alreadyRu);
        Console.WriteLine("Remaining untranslated: " + (totalGemini - alreadyRu));
        Console.WriteLine("\nUntranslated by category in LSI:");
        foreach (var kvp in categoryCounts)
        {
            Console.WriteLine(string.Format("  - {0}: {1}", kvp.Key, kvp.Value));
        }

        Console.WriteLine(string.Format("\nTingen / Story / Characters Candidates: {0}", untranslatedTingen.Count));

        // Output first 10 candidates
        int show = Math.Min(10, untranslatedTingen.Count);
        Console.WriteLine("\nSample candidates:");
        for (int i = 0; i < show; i++)
        {
            var item = untranslatedTingen[i];
            Console.WriteLine(string.Format("[{0}] Tag: {1}\n  EN: {2}\n  CN: {3}\n", i + 1, item.Item3, 
                item.Item2.Length > 100 ? item.Item2.Substring(0, 100) + "..." : item.Item2,
                item.Item1.Length > 100 ? item.Item1.Substring(0, 100) + "..." : item.Item1));
        }
    }
}
