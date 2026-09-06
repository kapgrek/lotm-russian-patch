using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectBatch9Candidates
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

        var seen = new HashSet<string>(StringComparer.Ordinal);
        var mainCandidates = new List<Tuple<string, string>>();
        var otherCandidates = new List<Tuple<string, string>>();
        var asideCandidates = new List<Tuple<string, string>>();
        var oldCandidates = new List<Tuple<string, string>>();

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
                        var tags = lsiTagMap.ContainsKey(sk) ? lsiTagMap[sk] : new List<string>();

                        bool isMain = tags.Contains("maintask") || tags.Contains("main");
                        bool isOther = tags.Contains("othertalk");
                        bool isAside = tags.Contains("asidetalk");
                        bool isOld = tags.Contains("oldtalk");

                        if (isMain)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            mainCandidates.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isOther)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            otherCandidates.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isAside)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            asideCandidates.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isOld)
                        {
                            seen.Add(cnKey); seen.Add(enVal);
                            oldCandidates.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Available: maintask={0}, othertalk={1}, asidetalk={2}, oldtalk={3}",
            mainCandidates.Count, otherCandidates.Count, asideCandidates.Count, oldCandidates.Count));

        Console.WriteLine("\n--- Sample MainTask ---");
        for (int i = 0; i < Math.Min(10, mainCandidates.Count); i++)
            Console.WriteLine(string.Format("[{0}] {1}", mainCandidates[i].Item1, mainCandidates[i].Item2));

        Console.WriteLine("\n--- Sample OtherTalk ---");
        for (int i = 0; i < Math.Min(5, otherCandidates.Count); i++)
            Console.WriteLine(string.Format("[{0}] {1}", otherCandidates[i].Item1, otherCandidates[i].Item2));

        Console.WriteLine("\n--- Sample AsideTalk ---");
        for (int i = 0; i < Math.Min(5, asideCandidates.Count); i++)
            Console.WriteLine(string.Format("[{0}] {1}", asideCandidates[i].Item1, asideCandidates[i].Item2));

        Console.WriteLine("\n--- Sample OldTalk ---");
        for (int i = 0; i < Math.Min(5, oldCandidates.Count); i++)
            Console.WriteLine(string.Format("[{0}] {1}", oldCandidates[i].Item1, oldCandidates[i].Item2));
    }
}
