using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectBatch7Details
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

        var maintasks = new List<Tuple<string, string>>();
        var sidetasks = new List<Tuple<string, string>>();
        var gossips = new List<Tuple<string, string>>();

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

                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") || enVal.EndsWith(".lua") || enVal.EndsWith(".uasset"))
                            continue;

                        string sk = SourceKey(cnKey);
                        if (lsiTagMap.ContainsKey(sk))
                        {
                            var tags = lsiTagMap[sk];
                            if (tags.Contains("maintask") || tags.Contains("main"))
                            {
                                maintasks.Add(Tuple.Create(cnKey, enVal));
                            }
                            else if (tags.Contains("sidetask") || tags.Contains("itemtask"))
                            {
                                sidetasks.Add(Tuple.Create(cnKey, enVal));
                            }
                            else if (tags.Contains("gossip"))
                            {
                                gossips.Add(Tuple.Create(cnKey, enVal));
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Found untranslated:\n  maintask/main: {0}\n  sidetask/itemtask: {1}\n  gossip: {2}",
            maintasks.Count, sidetasks.Count, gossips.Count));

        Console.WriteLine("\n--- Sample maintasks (10) ---");
        for (int i = 0; i < Math.Min(10, maintasks.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] CN: {1}\n      EN: {2}", i + 1, maintasks[i].Item1, maintasks[i].Item2));
        }

        Console.WriteLine("\n--- Sample sidetasks (10) ---");
        for (int i = 0; i < Math.Min(10, sidetasks.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] CN: {1}\n      EN: {2}", i + 1, sidetasks[i].Item1, sidetasks[i].Item2));
        }

        Console.WriteLine("\n--- Sample gossips (10) ---");
        for (int i = 0; i < Math.Min(10, gossips.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] CN: {1}\n      EN: {2}", i + 1, gossips[i].Item1, gossips[i].Item2));
        }
    }
}
