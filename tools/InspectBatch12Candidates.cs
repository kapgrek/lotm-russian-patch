using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectBatch12Candidates
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

        var oldTalk = new List<Tuple<string, string>>();
        var talkOther = new List<Tuple<string, string>>();
        var tingen = new List<Tuple<string, string>>();
        var otherTalk = new List<Tuple<string, string>>();
        var asideTalk = new List<Tuple<string, string>>();
        var gossip = new List<Tuple<string, string>>();
        var others = new List<Tuple<string, string>>();

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

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching"))
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
                            }

                            if (tags.Contains("oldtalk")) oldTalk.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("talkother")) talkOther.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("tingentalk") || tags.Contains("tingen")) tingen.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("othertalk")) otherTalk.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("asidetalk")) asideTalk.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("gossip")) gossip.Add(Tuple.Create(cnKey, enVal));
                            else others.Add(Tuple.Create(cnKey, enVal));

                            seen.Add(cnKey);
                            seen.Add(enVal);
                        }
                        else
                        {
                            untagged++;
                        }
                    }
                }
            }
        }

        Console.WriteLine("\n=== Tagged Candidate Breakdown for Batch 12 ===");
        Console.WriteLine("  oldtalk: " + oldTalk.Count);
        Console.WriteLine("  talkother: " + talkOther.Count);
        Console.WriteLine("  tingentalk / tingen: " + tingen.Count);
        Console.WriteLine("  othertalk: " + otherTalk.Count);
        Console.WriteLine("  asidetalk: " + asideTalk.Count);
        Console.WriteLine("  gossip: " + gossip.Count);
        Console.WriteLine("  other tagged categories: " + others.Count);
        Console.WriteLine("  untagged entries: " + untagged);

        Console.WriteLine("\nTop Tags among remaining candidates:");
        var sortedTags = new List<KeyValuePair<string, int>>(tagCounters);
        sortedTags.Sort((a, b) => b.Value.CompareTo(a.Value));
        for (int i = 0; i < Math.Min(25, sortedTags.Count); i++)
        {
            Console.WriteLine(string.Format("  {0, -25} : {1}", sortedTags[i].Key, sortedTags[i].Value));
        }
    }
}
