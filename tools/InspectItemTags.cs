using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectItemTags
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
        Console.WriteLine("Existing RU entries: " + existingRu.Count);

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
                            string rest = line.Substring(end + 2).Trim();
                            var tags = new List<string>();
                            var matches = Regex.Matches(rest, @"""([a-zA-Z0-9_]+):(\d+)""");
                            foreach (Match m in matches)
                            {
                                tags.Add(m.Groups[1].Value);
                            }
                            if (tags.Count > 0)
                            {
                                lsiTagMap[hashKey] = tags;
                            }
                        }
                    }
                }
            }
        }
        Console.WriteLine("LSI loaded: " + lsiTagMap.Count);

        var tagStats = new Dictionary<string, int>();
        var itemCandidates = new List<Tuple<string, string, string>>();

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

                        if (existingRu.Contains(cnKey) || existingRu.Contains(enVal))
                            continue;

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        string sk = SourceKey(cnKey);
                        if (lsiTagMap.ContainsKey(sk))
                        {
                            foreach (var tag in lsiTagMap[sk])
                            {
                                if (!tagStats.ContainsKey(tag)) tagStats[tag] = 0;
                                tagStats[tag]++;

                                if (tag.StartsWith("item") || tag.StartsWith("equip") || tag.StartsWith("potion") ||
                                    tag.StartsWith("formula") || tag.StartsWith("artifact") || tag.StartsWith("prop"))
                                {
                                    itemCandidates.Add(Tuple.Create(cnKey, enVal, tag));
                                }
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("\nUntranslated Tag Counts across Gemini:");
        foreach (var kvp in tagStats)
        {
            if (kvp.Value > 20)
                Console.WriteLine(string.Format("  {0}: {1}", kvp.Key, kvp.Value));
        }

        Console.WriteLine("\nTotal item candidate records: " + itemCandidates.Count);
        Console.WriteLine("\nSamples:");
        int show = Math.Min(20, itemCandidates.Count);
        for (int i = 0; i < show; i++)
        {
            Console.WriteLine(string.Format("[{0}] Tag: {1}\n  EN: {2}\n  CN: {3}\n", i + 1, itemCandidates[i].Item3, itemCandidates[i].Item2, itemCandidates[i].Item1));
        }
    }
}
