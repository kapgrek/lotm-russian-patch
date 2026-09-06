using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class CheckItemCoverage
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

        int itemTagCount = 0;
        int potionFormulaCount = 0;
        int artifactCount = 0;
        int materialCount = 0;
        int equipmentCount = 0;

        var candidates = new Dictionary<string, Tuple<string, string, string>>(); // key -> (cn, en, reason)

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
                        bool hasItemTag = false;
                        string matchedTag = "";
                        if (lsiTagMap.ContainsKey(sk))
                        {
                            foreach (var tag in lsiTagMap[sk])
                            {
                                if (tag.StartsWith("item") || tag.StartsWith("equip") || tag.StartsWith("potion") ||
                                    tag.StartsWith("formula") || tag.StartsWith("artifact") || tag.StartsWith("prop"))
                                {
                                    hasItemTag = true;
                                    matchedTag = tag;
                                    break;
                                }
                            }
                        }

                        bool isPotionOrFormula = cnKey.Contains("魔药") || cnKey.Contains("配方") || enVal.Contains("Potion") || enVal.Contains("Formula") || enVal.Contains("Recipe");
                        bool isArtifact = cnKey.Contains("封印物") || enVal.Contains("Sealed Artifact") || enVal.Contains("Artifact 2-") || enVal.Contains("Artifact 3-") || enVal.Contains("Artifact 1-") || enVal.Contains("Artifact 0-");
                        bool isMaterial = cnKey.Contains("主材料") || cnKey.Contains("辅助材料") || cnKey.Contains("非凡特性") || enVal.Contains("Main Ingredient") || enVal.Contains("Supplementary Ingredient") || enVal.Contains("Beyonder Characteristic");
                        bool isEquip = cnKey.Contains("装备") || cnKey.Contains("长袍") || cnKey.Contains("手套") || cnKey.Contains("靴") || enVal.Contains("Equipment") || enVal.Contains("Robe") || enVal.Contains("Boots") || enVal.Contains("Gloves") || enVal.Contains("Ring") || enVal.Contains("Necklace");

                        if (hasItemTag)
                        {
                            itemTagCount++;
                            candidates[cnKey] = Tuple.Create(cnKey, enVal, "tag:" + matchedTag);
                        }
                        else if (isPotionOrFormula)
                        {
                            potionFormulaCount++;
                            candidates[cnKey] = Tuple.Create(cnKey, enVal, "potion/formula");
                        }
                        else if (isArtifact)
                        {
                            artifactCount++;
                            candidates[cnKey] = Tuple.Create(cnKey, enVal, "artifact");
                        }
                        else if (isMaterial)
                        {
                            materialCount++;
                            candidates[cnKey] = Tuple.Create(cnKey, enVal, "material");
                        }
                        else if (isEquip && (cnKey.Length < 30 || enVal.Length < 60))
                        {
                            equipmentCount++;
                            candidates[cnKey] = Tuple.Create(cnKey, enVal, "equip");
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Item Tags: {0}", itemTagCount));
        Console.WriteLine(string.Format("Potion / Formula: {0}", potionFormulaCount));
        Console.WriteLine(string.Format("Artifacts: {0}", artifactCount));
        Console.WriteLine(string.Format("Materials / Characteristics: {0}", materialCount));
        Console.WriteLine(string.Format("Equipment / Apparel: {0}", equipmentCount));
        Console.WriteLine(string.Format("Total Unique Item Candidates: {0}", candidates.Count));
    }
}
