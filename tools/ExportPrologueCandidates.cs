using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class ExportPrologueCandidates
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

    static void Main(string[] args)
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

        var prologueList = new List<Tuple<string, string, string, int>>(); // cn, en, tag, lineNum
        int lineNum = 0;

        using (var reader = new StreamReader(GeminiPath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNum++;
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

                        string sk = SourceKey(cnKey);
                        string tag = lsiTagMap.ContainsKey(sk) ? lsiTagMap[sk] : "";

                        // Filter criteria for Prologue and Tingen dialogue
                        bool isPrologue = tag.Contains("tingentalk") || tag.Contains("asidetalk") ||
                                          tag.Contains("oldtalk") || tag.Contains("talkother") ||
                                          enVal.Contains("Prologue") || cnKey.Contains("序章") ||
                                          tag.Contains("tingen") ||
                                          enVal.Contains("Klein") || enVal.Contains("Melissa") ||
                                          enVal.Contains("Benson") || enVal.Contains("Dunn") ||
                                          enVal.Contains("Nighthawk") || enVal.Contains("Moretti") ||
                                          enVal.Contains("Leonard") || enVal.Contains("Neil") ||
                                          enVal.Contains("Zoutland") || enVal.Contains("Daffodil") ||
                                          enVal.Contains("Iron Cross") || enVal.Contains("Blackthorn");

                        if (isPrologue)
                        {
                            prologueList.Add(Tuple.Create(cnKey, enVal, tag, lineNum));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Found {0} untranslated Prologue / Tingen dialogue candidates.", prologueList.Count));

        // Group by tag type
        var tagGroups = new Dictionary<string, int>();
        foreach (var p in prologueList)
        {
            string g = "other";
            if (p.Item3.Contains("tingentalk")) g = "tingentalk";
            else if (p.Item3.Contains("asidetalk")) g = "asidetalk";
            else if (p.Item3.Contains("oldtalk")) g = "oldtalk";
            else if (p.Item3.Contains("talkother")) g = "talkother";
            else if (p.Item3.Contains("tingen")) g = "tingen";
            else if (p.Item2.Contains("Prologue") || p.Item1.Contains("序章")) g = "prologue_direct";

            if (!tagGroups.ContainsKey(g)) tagGroups[g] = 0;
            tagGroups[g]++;
        }

        foreach (var kvp in tagGroups)
        {
            Console.WriteLine(string.Format("  Group {0}: {1}", kvp.Key, kvp.Value));
        }

        // Write first 1000 to a json or tsv file for inspection/processing
        using (var writer = new StreamWriter(@"tools\prologue_candidates.txt", false, Encoding.UTF8))
        {
            int maxExport = Math.Min(1200, prologueList.Count);
            for (int i = 0; i < maxExport; i++)
            {
                var p = prologueList[i];
                writer.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", p.Item4, p.Item3, p.Item1.Replace("\t", " "), p.Item2.Replace("\t", " ")));
            }
        }
        Console.WriteLine("Saved top candidates to tools\\prologue_candidates.txt");
    }
}
