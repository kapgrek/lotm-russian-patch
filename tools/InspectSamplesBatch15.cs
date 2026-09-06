using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectSamplesBatch15
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

        var monsterSkill = new List<Tuple<string, string>>();
        var buffData = new List<Tuple<string, string>>();
        var buffAppear = new List<Tuple<string, string>>();
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

                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") || enVal.StartsWith("[Discarded]") || enVal.StartsWith("dj"))
                            continue;

                        string sk = SourceKey(cnKey);
                        List<string> tags;
                        if (lsiTagMap.TryGetValue(sk, out tags))
                        {
                            if (tags.Contains("monsterskill")) monsterSkill.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("buffdata")) buffData.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("buffappear")) buffAppear.Add(Tuple.Create(cnKey, enVal));

                            seen.Add(cnKey);
                            seen.Add(enVal);
                        }
                    }
                }
            }
        }

        Console.WriteLine("\n--- Sample Monster Skills (Total: " + monsterSkill.Count + ") ---");
        for (int i = 0; i < Math.Min(15, monsterSkill.Count); i++)
        {
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", monsterSkill[i].Item2, monsterSkill[i].Item1));
        }

        Console.WriteLine("\n--- Sample Buff Data (Total: " + buffData.Count + ") ---");
        for (int i = 0; i < Math.Min(15, buffData.Count); i++)
        {
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", buffData[i].Item2, buffData[i].Item1));
        }

        Console.WriteLine("\n--- Sample Buff Appear (Total: " + buffAppear.Count + ") ---");
        for (int i = 0; i < Math.Min(15, buffAppear.Count); i++)
        {
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", buffAppear[i].Item2, buffAppear[i].Item1));
        }
    }
}
