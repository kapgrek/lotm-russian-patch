using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class AnalyzeItemStrings
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

        var lsiTagMap = new Dictionary<string, string>();
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
                            lsiTagMap[hashKey] = tag;
                        }
                    }
                }
            }
        }
        Console.WriteLine("LSI mappings loaded: " + lsiTagMap.Count);

        var itemTags = new Dictionary<string, int>();
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
                        string tag = lsiTagMap.ContainsKey(sk) ? lsiTagMap[sk] : "";

                        bool isItemTag = tag.Contains("item") || tag.Contains("equip") || tag.Contains("potion") ||
                                         tag.Contains("prop") || tag.Contains("formula") || tag.Contains("artifact") ||
                                         tag.Contains("weapon") || tag.Contains("material") || tag.Contains("drop");

                        bool hasItemKeyword = enVal.Contains("Potion") || enVal.Contains("Formula") ||
                                              enVal.Contains("Characteristic") || enVal.Contains("Artifact") ||
                                              enVal.Contains("Sequence ") || enVal.Contains("Recipe") ||
                                              enVal.Contains("Remains") || enVal.Contains("Blood") ||
                                              enVal.Contains("Eye") || enVal.Contains("Crystal") ||
                                              enVal.Contains("Ring") || enVal.Contains("Amulet") ||
                                              enVal.Contains("Charm") || enVal.Contains("Dagger") ||
                                              enVal.Contains("Sword") || enVal.Contains("Staff") ||
                                              enVal.Contains("Robe") || enVal.Contains("Boots") ||
                                              enVal.Contains("Gloves");

                        if (isItemTag || (hasItemKeyword && (tag == "" || tag.Contains("other") || tag.Contains("system"))))
                        {
                            string prefix = tag;
                            int col = tag.IndexOf(':');
                            if (col > 0)
                            {
                                int eq = tag.IndexOf('=');
                                prefix = tag.Substring(eq + 1, col - eq - 1).Trim().Trim('"');
                            }
                            else if (string.IsNullOrEmpty(tag))
                            {
                                prefix = "(no-tag)";
                            }

                            if (!itemTags.ContainsKey(prefix)) itemTags[prefix] = 0;
                            itemTags[prefix]++;

                            itemCandidates.Add(Tuple.Create(cnKey, enVal, tag));
                        }
                    }
                }
            }
        }

        Console.WriteLine("\nTotal item/equip/potion candidates found: " + itemCandidates.Count);
        Console.WriteLine("Tags breakdown:");
        foreach (var kvp in itemTags)
        {
            Console.WriteLine(string.Format("  - {0}: {1}", kvp.Key, kvp.Value));
        }

        Console.WriteLine("\nFirst 15 sample candidates:");
        int show = Math.Min(15, itemCandidates.Count);
        for (int i = 0; i < show; i++)
        {
            var item = itemCandidates[i];
            Console.WriteLine(string.Format("[{0}] Tag: {1}\n  EN: {2}\n  CN: {3}\n", i + 1, item.Item3, item.Item2, item.Item1));
        }
    }
}
