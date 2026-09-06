using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class AnalyzeBatch3Lore
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

        var roselle = new List<Tuple<string, string>>();
        var letters = new List<Tuple<string, string>>();
        var newspapers = new List<Tuple<string, string>>();
        var diariesBooks = new List<Tuple<string, string>>();
        var cityGossip = new List<Tuple<string, string>>();

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

                        // Skip internal debug strings
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID"))
                            continue;

                        string sk = SourceKey(cnKey);
                        var tags = lsiTagMap.ContainsKey(sk) ? lsiTagMap[sk] : new List<string>();

                        bool isRoselle = enVal.Contains("Roselle") || cnKey.Contains("罗塞尔");
                        bool isLetter = tags.Contains("lettertext") || cnKey.Contains("信件") || cnKey.Contains("书信") ||
                                        Regex.IsMatch(enVal, @"\b(Letter|Letter to|Dear |From:)\b");
                        bool isNewspaper = tags.Contains("newspaper") || cnKey.Contains("报纸") || cnKey.Contains("日报") ||
                                           cnKey.Contains("晨报") || Regex.IsMatch(enVal, @"\b(Newspaper|Daily|Morning Post|Tingen Daily|Backlund Morning News)\b");
                        bool isDiaryOrBook = cnKey.Contains("日记") || cnKey.Contains("手稿") || cnKey.Contains("文献") ||
                                             cnKey.Contains("典籍") || tags.Contains("book") ||
                                             Regex.IsMatch(enVal, @"\b(Diary|Manuscript|Chronicle|Journal)\b");
                        bool isGossip = tags.Contains("gossip");

                        if (isRoselle) roselle.Add(Tuple.Create(cnKey, enVal));
                        else if (isDiaryOrBook) diariesBooks.Add(Tuple.Create(cnKey, enVal));
                        else if (isLetter) letters.Add(Tuple.Create(cnKey, enVal));
                        else if (isNewspaper) newspapers.Add(Tuple.Create(cnKey, enVal));
                        else if (isGossip && (enVal.Length > 20)) cityGossip.Add(Tuple.Create(cnKey, enVal));
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Roselle Diaries & Lore: {0}\nDiaries & Books & Manuscripts: {1}\nLetters & Mail: {2}\nNewspapers & Press: {3}\nCity Lore & Gossip: {4}",
            roselle.Count, diariesBooks.Count, letters.Count, newspapers.Count, cityGossip.Count));

        int total = roselle.Count + diariesBooks.Count + letters.Count + newspapers.Count + cityGossip.Count;
        Console.WriteLine(string.Format("Total Curated Lore Candidates: {0}", total));

        Console.WriteLine("\n--- Sample Roselle Texts ---");
        for (int i = 0; i < Math.Min(5, roselle.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] EN: {1}\n    CN: {2}\n", i + 1, roselle[i].Item2, roselle[i].Item1));
        }

        Console.WriteLine("\n--- Sample Letters & Diaries ---");
        for (int i = 0; i < Math.Min(5, diariesBooks.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] EN: {1}\n    CN: {2}\n", i + 1, diariesBooks[i].Item2, diariesBooks[i].Item1));
        }

        Console.WriteLine("\n--- Sample Newspapers ---");
        for (int i = 0; i < Math.Min(5, newspapers.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] EN: {1}\n    CN: {2}\n", i + 1, newspapers[i].Item2, newspapers[i].Item1));
        }
    }
}
