using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectBatch6Categories
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
        Console.WriteLine("Existing RU keys: " + existingRu.Count);

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

        var pManor = new List<Tuple<string, string>>();
        var pBeckland = new List<Tuple<string, string>>();
        var pGossip = new List<Tuple<string, string>>();
        var pOldTalk = new List<Tuple<string, string>>();
        var pOtherTalk = new List<Tuple<string, string>>();
        var pAsideTalk = new List<Tuple<string, string>>();

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
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden"))
                            continue;

                        string sk = SourceKey(cnKey);
                        var tags = lsiTagMap.ContainsKey(sk) ? lsiTagMap[sk] : new List<string>();

                        bool isManor = tags.Contains("manor");
                        bool isBeckland = tags.Contains("beckland") || enVal.Contains("Backlund");
                        bool isGossip = tags.Contains("gossip");
                        bool isOldTalk = tags.Contains("oldtalk");
                        bool isOtherTalk = tags.Contains("othertalk");
                        bool isAsideTalk = tags.Contains("asidetalk");

                        if (isManor)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            pManor.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isBeckland)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            pBeckland.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isGossip)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            pGossip.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isOldTalk)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            pOldTalk.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isOtherTalk)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            pOtherTalk.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isAsideTalk)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            pAsideTalk.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Available candidates:\n  - Manor (мебель, поместье, убранство, поручения): {0}\n  - Beckland (события и упоминания Бэкланда): {1}\n  - Gossip (слухи, городские сплетни): {2}\n  - OldTalk (сюжетные и характерные реплики): {3}\n  - OtherTalk (разговоры горожан): {4}\n  - AsideTalk (фоновые реплики): {5}",
            pManor.Count, pBeckland.Count, pGossip.Count, pOldTalk.Count, pOtherTalk.Count, pAsideTalk.Count));

        Console.WriteLine("\nSamples of Manor:");
        for (int i = 0; i < Math.Min(5, pManor.Count); i++) Console.WriteLine("  " + pManor[i].Item2);

        Console.WriteLine("\nSamples of Gossip:");
        for (int i = 0; i < Math.Min(5, pGossip.Count); i++) Console.WriteLine("  " + pGossip[i].Item2);

        Console.WriteLine("\nSamples of OldTalk:");
        for (int i = 0; i < Math.Min(5, pOldTalk.Count); i++) Console.WriteLine("  " + pOldTalk[i].Item2);
    }
}
