using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectBatch16Candidates
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string LsiDir = @"d:\gameDev\translate lotm\mod_base\Saved\Mods\lua\mods\cpdd_runtime_fixes";

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
        Console.WriteLine("Existing entries in Ru dictionary: " + existingRu.Count);

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
        Console.WriteLine("LSI tags loaded: " + lsiTagMap.Count);

        var tagCounters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        int untagged = 0;
        var seen = new HashSet<string>(StringComparer.Ordinal);

        var samplePerTag = new Dictionary<string, List<Tuple<string, string>>>(StringComparer.OrdinalIgnoreCase);
        var untaggedSamples = new List<Tuple<string, string>>();

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

                        // Исключаем системный мусор
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") ||
                            enVal.StartsWith(".p4config") || enVal.Contains("-----") || enVal.Contains(".cpp") || enVal.Contains(".h"))
                            continue;

                        string sk = SourceKey(cnKey);
                        List<string> tags;
                        if (lsiTagMap.TryGetValue(sk, out tags))
                        {
                            foreach (var tag in tags)
                            {
                                int c;
                                tagCounters.TryGetValue(tag, out c);
                                tagCounters[tag] = c + 1;

                                List<Tuple<string, string>> list;
                                if (!samplePerTag.TryGetValue(tag, out list))
                                {
                                    list = new List<Tuple<string, string>>();
                                    samplePerTag[tag] = list;
                                }
                                if (list.Count < 5)
                                {
                                    list.Add(Tuple.Create(cnKey, enVal));
                                }
                            }
                        }
                        else
                        {
                            untagged++;
                            if (untaggedSamples.Count < 30)
                            {
                                untaggedSamples.Add(Tuple.Create(cnKey, enVal));
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("\n=== Tag Counters (Untranslated Non-Debug) ===");
        var sortedTags = new List<KeyValuePair<string, int>>(tagCounters);
        sortedTags.Sort((a, b) => b.Value.CompareTo(a.Value));
        foreach (var kvp in sortedTags)
        {
            Console.WriteLine(string.Format("{0,-25} : {1,6}", kvp.Key, kvp.Value));
        }
        Console.WriteLine(string.Format("{0,-25} : {1,6}", "Untagged", untagged));

        Console.WriteLine("\n=== Top Tags Samples ===");
        for (int i = 0; i < Math.Min(15, sortedTags.Count); i++)
        {
            var tag = sortedTags[i].Key;
            Console.WriteLine(string.Format("\n--- Tag: {0} ({1} entries) ---", tag, sortedTags[i].Value));
            foreach (var s in samplePerTag[tag])
            {
                Console.WriteLine(string.Format("  EN: {0} | CN: {1}", s.Item2, s.Item1));
            }
        }

        Console.WriteLine("\n=== Untagged Samples (First 20) ===");
        for (int i = 0; i < Math.Min(20, untaggedSamples.Count); i++)
        {
            Console.WriteLine(string.Format("  EN: {0} | CN: {1}", untaggedSamples[i].Item2, untaggedSamples[i].Item1));
        }
    }
}
