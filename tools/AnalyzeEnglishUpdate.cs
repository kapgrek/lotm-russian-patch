using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class AnalyzeEnglishUpdate
{
    static void Main(string[] args)
    {
        string shardsDir = @"temp_en_2.6.0\extracted\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes";
        string oldSourceEnFile = @"source_en\RuntimeTextGemini.lua";
        string masterRuFile = @"RuntimeTextRussian.lua";
        string ruLocFile = @"RussianLocalization.lua";
        string enInitFile = Path.Combine(shardsDir, "Init.lua");
        string myInitFile = @"data\Init.lua";
        string enOverridesFile = @"temp_en_2.6.0\extracted\payload\bridge\game\Saved\Mods\translation-overrides.lua";

        Console.WriteLine("=== LOTM Translation Analysis Tool ===");

        // 1. Read all entries from new English 1024 shards
        Console.WriteLine("1. Reading shards from " + shardsDir);
        var newEnShardsEntries = new Dictionary<string, string>(); // CN key -> EN val
        var shardFiles = Directory.GetFiles(shardsDir, "RuntimeTextGemini_*.lua");
        Console.WriteLine("Found " + shardFiles.Length + " shard files.");

        Regex entryRegex = new Regex(@"^\s*\[""(.*)""\]\s*=\s*""(.*)"",?\s*$", RegexOptions.Compiled);

        foreach (var file in shardFiles)
        {
            var lines = File.ReadAllLines(file, Encoding.UTF8);
            foreach (var line in lines)
            {
                var match = entryRegex.Match(line);
                if (match.Success)
                {
                    string cnKey = Unescape(match.Groups[1].Value);
                    string enVal = Unescape(match.Groups[2].Value);
                    newEnShardsEntries[cnKey] = enVal;
                }
            }
        }
        Console.WriteLine("Total entries in new EN shards: " + newEnShardsEntries.Count);

        // 2. Read old source_en/RuntimeTextGemini.lua
        Console.WriteLine("\n2. Reading old source_en/RuntimeTextGemini.lua...");
        var oldEnEntries = new Dictionary<string, string>();
        if (File.Exists(oldSourceEnFile))
        {
            var lines = File.ReadAllLines(oldSourceEnFile, Encoding.UTF8);
            foreach (var line in lines)
            {
                var match = entryRegex.Match(line);
                if (match.Success)
                {
                    string cnKey = Unescape(match.Groups[1].Value);
                    string enVal = Unescape(match.Groups[2].Value);
                    oldEnEntries[cnKey] = enVal;
                }
            }
            Console.WriteLine("Total entries in old source_en: " + oldEnEntries.Count);
        }

        // 3. Compare old source_en vs new EN shards
        int brandNewKeys = 0;
        int modifiedEnTranslations = 0;
        var newKeysSample = new List<KeyValuePair<string, string>>();
        var modifiedKeysSample = new List<Tuple<string, string, string>>(); // key, oldEn, newEn

        foreach (var kvp in newEnShardsEntries)
        {
            if (!oldEnEntries.ContainsKey(kvp.Key))
            {
                brandNewKeys++;
                if (newKeysSample.Count < 20) newKeysSample.Add(kvp);
            }
            else if (oldEnEntries[kvp.Key] != kvp.Value)
            {
                modifiedEnTranslations++;
                if (modifiedKeysSample.Count < 20) modifiedKeysSample.Add(Tuple.Create(kvp.Key, oldEnEntries[kvp.Key], kvp.Value));
            }
        }

        int removedKeys = 0;
        foreach (var k in oldEnEntries.Keys)
        {
            if (!newEnShardsEntries.ContainsKey(k)) removedKeys++;
        }

        Console.WriteLine("Keys in new shards not in old source_en (NEW): " + brandNewKeys);
        Console.WriteLine("Keys with updated EN text (MODIFIED): " + modifiedEnTranslations);
        Console.WriteLine("Keys in old source_en missing in new shards (REMOVED): " + removedKeys);

        // 4. Check coverage in Russian Master Dictionary (RuntimeTextRussian.lua)
        Console.WriteLine("\n3. Checking coverage in master Russian dictionary...");
        var ruDictionary = new Dictionary<string, string>();
        if (File.Exists(masterRuFile))
        {
            var lines = File.ReadAllLines(masterRuFile, Encoding.UTF8);
            foreach (var line in lines)
            {
                var match = entryRegex.Match(line);
                if (match.Success)
                {
                    string key = Unescape(match.Groups[1].Value);
                    string val = Unescape(match.Groups[2].Value);
                    ruDictionary[key] = val;
                }
            }
            Console.WriteLine("Total entries in RuntimeTextRussian.lua: " + ruDictionary.Count);
        }

        int missingByCnKey = 0;
        int missingByEnKey = 0;
        var missingCompletelyList = new List<KeyValuePair<string, string>>();

        foreach (var kvp in newEnShardsEntries)
        {
            bool hasCn = ruDictionary.ContainsKey(kvp.Key);
            bool hasEn = ruDictionary.ContainsKey(kvp.Value);

            if (!hasCn) missingByCnKey++;
            if (!hasEn) missingByEnKey++;

            if (!hasCn && !hasEn)
            {
                missingCompletelyList.Add(kvp);
            }
        }

        Console.WriteLine("New shards entries missing in RU by CN key: " + missingByCnKey);
        Console.WriteLine("New shards entries missing in RU by EN key: " + missingByEnKey);
        Console.WriteLine("New shards entries missing in RU completely (neither CN nor EN key): " + missingCompletelyList.Count);

        // 5. Check translation-overrides.lua in new patch vs our RussianLocalization.lua / Init.lua
        Console.WriteLine("\n4. Checking translation-overrides.lua and other files...");
        if (File.Exists(enOverridesFile))
        {
            var lines = File.ReadAllLines(enOverridesFile, Encoding.UTF8);
            Console.WriteLine("translation-overrides.lua lines: " + lines.Length);
        }

        // Print details
        Console.WriteLine("\n=== SAMPLES OF NEW KEYS ===");
        foreach (var sample in newKeysSample)
        {
            Console.WriteLine(string.Format("[CN]: {0}\n[EN]: {1}\n", sample.Key, sample.Value));
        }

        Console.WriteLine("=== SAMPLES OF MODIFIED TRANSLATIONS ===");
        foreach (var sample in modifiedKeysSample)
        {
            Console.WriteLine(string.Format("[CN]: {0}\n[OLD EN]: {1}\n[NEW EN]: {2}\n", sample.Item1, sample.Item2, sample.Item3));
        }

        Console.WriteLine("=== SAMPLES OF MISSING COMPLETELY IN RU ===");
        int count = 0;
        foreach (var sample in missingCompletelyList)
        {
            if (count++ >= 20) break;
            Console.WriteLine(string.Format("[CN]: {0}\n[EN]: {1}\n", sample.Key, sample.Value));
        }
    }

    static string Unescape(string s)
    {
        return s.Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
    }
}
