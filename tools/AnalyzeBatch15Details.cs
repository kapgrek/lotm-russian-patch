using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class AnalyzeBatch15Details
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
                        existingRu.Add(t.Substring(2, delim - 2));
                    }
                }
            }
        }

        var lsiTagMap = new Dictionary<string, List<string>>();
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
                        foreach (Match m in Regex.Matches(rest, @"""([a-zA-Z0-9_]+):(\d+)"""))
                        {
                            tags.Add(m.Groups[1].Value);
                        }
                        if (tags.Count > 0) lsiTagMap[hashKey] = tags;
                    }
                }
            }
        }

        var buffDescriptions = new List<Tuple<string, string>>();
        var buffNames = new List<Tuple<string, string>>();
        var monsterSkillList = new List<Tuple<string, string>>();
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

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        string sk = SourceKey(cnKey);
                        List<string> tags;
                        if (lsiTagMap.TryGetValue(sk, out tags))
                        {
                            if (tags.Contains("monsterskill"))
                            {
                                monsterSkillList.Add(Tuple.Create(cnKey, enVal));
                                seen.Add(cnKey); seen.Add(enVal);
                            }
                            else if (tags.Contains("buffdata"))
                            {
                                if (enVal.Length > 30 || enVal.Contains("%") || enVal.Contains("damage") || enVal.Contains("Damage") || enVal.Contains("reduces") || enVal.Contains("increases"))
                                {
                                    buffDescriptions.Add(Tuple.Create(cnKey, enVal));
                                }
                                else
                                {
                                    buffNames.Add(Tuple.Create(cnKey, enVal));
                                }
                                seen.Add(cnKey); seen.Add(enVal);
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("Monster Skills: " + monsterSkillList.Count);
        Console.WriteLine("Buff Descriptions (long/effects): " + buffDescriptions.Count);
        Console.WriteLine("Buff Names (short/status): " + buffNames.Count);

        Console.WriteLine("\n--- Sample Buff Descriptions (First 15) ---");
        for (int i = 0; i < Math.Min(15, buffDescriptions.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] EN: {1} | CN: {2}", i, buffDescriptions[i].Item2, buffDescriptions[i].Item1));
        }

        Console.WriteLine("\n--- Sample Buff Names (First 15) ---");
        for (int i = 0; i < Math.Min(15, buffNames.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] EN: {1} | CN: {2}", i, buffNames[i].Item2, buffNames[i].Item1));
        }
    }
}
