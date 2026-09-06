using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectSamplesBatch13
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

        var oldTalk = new List<Tuple<string, string>>();
        var asideTalk = new List<Tuple<string, string>>();
        var otherTalk = new List<Tuple<string, string>>();
        var talkOther = new List<Tuple<string, string>>();
        var tingen = new List<Tuple<string, string>>();
        var itemNormal = new List<Tuple<string, string>>();
        var gossip = new List<Tuple<string, string>>();

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
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching"))
                            continue;

                        string sk = SourceKey(cnKey);
                        List<string> tags;
                        if (lsiTagMap.TryGetValue(sk, out tags))
                        {
                            if (tags.Contains("oldtalk") && oldTalk.Count < 5) oldTalk.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("asidetalk") && asideTalk.Count < 5) asideTalk.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("othertalk") && otherTalk.Count < 5) otherTalk.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("talkother") && talkOther.Count < 5) talkOther.Add(Tuple.Create(cnKey, enVal));
                            else if ((tags.Contains("tingentalk") || tags.Contains("tingen")) && tingen.Count < 5) tingen.Add(Tuple.Create(cnKey, enVal));
                            else if ((tags.Contains("itemnormal") || tags.Contains("item")) && itemNormal.Count < 5) itemNormal.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("gossip") && gossip.Count < 5) gossip.Add(Tuple.Create(cnKey, enVal));

                            seen.Add(cnKey);
                            seen.Add(enVal);
                        }
                    }
                }
            }
        }

        PrintSamples("OldTalk (Карточные клубы и встречи)", oldTalk);
        PrintSamples("AsideTalk (Уличные сценки и мысли)", asideTalk);
        PrintSamples("OtherTalk (Таверны и душевные разговоры)", otherTalk);
        PrintSamples("TalkOther (Поручения и горожане)", talkOther);
        PrintSamples("Tingen (Городские сцены Тингена)", tingen);
        PrintSamples("ItemNormal (Предметы и описания)", itemNormal);
        PrintSamples("Gossip (Городские сплетни)", gossip);
    }

    static void PrintSamples(string title, List<Tuple<string, string>> list)
    {
        Console.WriteLine("\n=== " + title + " ===");
        foreach (var item in list)
        {
            Console.WriteLine("CN: " + item.Item1);
            Console.WriteLine("EN: " + item.Item2);
            Console.WriteLine();
        }
    }
}
