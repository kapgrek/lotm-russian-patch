using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class InspectMissingAndUntranslated
{
    static bool HasCyrillic(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        foreach (char c in s)
        {
            if ((c >= 0x0400 && c <= 0x04FF) || c == 0x0500 || c == 0x0501)
                return true;
        }
        return false;
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string shardsDir = @"temp_en_2.6.0\extracted\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes";
        string masterRuFile = @"RuntimeTextRussian.lua";

        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        using (var reader = new StreamReader(masterRuFile, Encoding.UTF8))
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
                        string k = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string v = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";
                        ruDict[k] = v;
                    }
                }
            }
        }

        var newEnShards = new Dictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shardsDir, "RuntimeTextGemini_*.lua");
        foreach (var file in shardFiles)
        {
            using (var reader = new StreamReader(file, Encoding.UTF8))
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
                            string k = t.Substring(2, delim - 2);
                            int valStart = delim + 6;
                            int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                            string v = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";
                            newEnShards[k] = v;
                        }
                    }
                }
            }
        }

        Console.WriteLine("=== [1] 4 MISSING KEYS FROM RU DICTIONARY ===");
        int missingCount = 0;
        foreach (var kvp in newEnShards)
        {
            if (!ruDict.ContainsKey(kvp.Key))
            {
                missingCount++;
                Console.WriteLine(string.Format("Missing #{0}:\n  CN: {1}\n  EN: {2}\n", missingCount, kvp.Key, kvp.Value));
            }
        }

        Console.WriteLine("\n=== [2] NON-CYRILLIC IN RU DICTIONARY (SAMPLES OF 520) ===");
        int nonCyrCount = 0;
        int pureAsciiCount = 0;
        int pureNumOrPunct = 0;
        int untranslatedEnglish = 0;

        using (var writer = new StreamWriter(@"temp_en_2.6.0\non_cyrillic_ru_strings.txt", false, Encoding.UTF8))
        {
            foreach (var kvp in newEnShards)
            {
                string ruVal;
                if (ruDict.TryGetValue(kvp.Key, out ruVal) && !HasCyrillic(ruVal))
                {
                    nonCyrCount++;
                    writer.WriteLine(string.Format("[{0}] CN: {1}", nonCyrCount, kvp.Key));
                    writer.WriteLine(string.Format("    EN: {0}", kvp.Value));
                    writer.WriteLine(string.Format("    RU: {0}\n", ruVal));

                    if (System.Text.RegularExpressions.Regex.IsMatch(ruVal, @"^[0-9\.\,\-\+\%\:\s/\\_\(\)]+$"))
                    {
                        pureNumOrPunct++;
                    }
                    else if (ruVal == kvp.Value)
                    {
                        untranslatedEnglish++;
                    }
                    else
                    {
                        pureAsciiCount++;
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Total non-cyrillic: {0}", nonCyrCount));
        Console.WriteLine(string.Format(" - Pure numbers / punctuation / technical symbols: {0}", pureNumOrPunct));
        Console.WriteLine(string.Format(" - Identical to English (untranslated / English source): {0}", untranslatedEnglish));
        Console.WriteLine(string.Format(" - Other ASCII / formatting: {0}", pureAsciiCount));
    }
}
