using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class AnalyzePatch26Breakdown
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string rootDir = @"d:\gameDev\translate lotm";
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");
        string oldSourceEnFile = Path.Combine(rootDir, @"source_en\RuntimeTextGemini.lua");
        string ruMasterFile = Path.Combine(rootDir, @"RuntimeTextRussian.lua");

        // Load 2.6 shards
        var en26Entries = new Dictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shards26Dir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);
        foreach (var shardPath in shardFiles) ParseLuaTable(shardPath, en26Entries);

        // Load old source
        var oldEnEntries = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(oldSourceEnFile, oldEnEntries);

        // Load RU
        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(ruMasterFile, ruDict);

        // Analyze 35 modified strings
        Console.WriteLine("=== 35 MODIFIED STRINGS IN 2.6.0 ===");
        int modIdx = 0;
        foreach (var kvp in en26Entries)
        {
            string oldVal;
            if (oldEnEntries.TryGetValue(kvp.Key, out oldVal) && oldVal != kvp.Value)
            {
                modIdx++;
                Console.WriteLine(string.Format("[{0}] CN: {1}", modIdx, Escape(kvp.Key)));
                Console.WriteLine(string.Format("    Old EN: {0}", Escape(oldVal)));
                Console.WriteLine(string.Format("    New EN: {0}", Escape(kvp.Value)));
                string ruVal;
                if (ruDict.TryGetValue(kvp.Key, out ruVal)) Console.WriteLine(string.Format("    Current RU: {0}", Escape(ruVal)));
                Console.WriteLine();
            }
        }

        // Categorize 3,789 untranslated strings
        var untranslated = new List<KeyValuePair<string, string>>();
        foreach (var kvp in en26Entries)
        {
            if (!ruDict.ContainsKey(kvp.Key) && !ruDict.ContainsKey(kvp.Value))
            {
                untranslated.Add(kvp);
            }
        }

        Console.WriteLine("\n=== CATEGORIZATION OF 3,789 UNTRANSLATED STRINGS ===");
        var categories = new Dictionary<string, List<KeyValuePair<string, string>>>();
        categories["1. Dialogues & Story (Quests, NPC, Subtitles)"] = new List<KeyValuePair<string, string>>();
        categories["2. Combat Skills, Formulas & Buffs"] = new List<KeyValuePair<string, string>>();
        categories["3. Items, Weapons, Equipment & Potions"] = new List<KeyValuePair<string, string>>();
        categories["4. UI, Menus, Settings & System Prompts"] = new List<KeyValuePair<string, string>>();
        categories["5. Lore, Books, Notes & Diaries"] = new List<KeyValuePair<string, string>>();
        categories["6. World Map, Locations, Dungeons & Instances"] = new List<KeyValuePair<string, string>>();
        categories["7. Monsters, Bosses & NPC Names"] = new List<KeyValuePair<string, string>>();
        categories["8. Technical & Debug / Internal Keys"] = new List<KeyValuePair<string, string>>();
        categories["9. Other / Uncategorized"] = new List<KeyValuePair<string, string>>();

        foreach (var item in untranslated)
        {
            string cn = item.Key;
            string en = item.Value;

            if (Regex.IsMatch(en, @"\*(?:d|f)\*?|spellfielddisc|buffdisc|bulletdisc|mul\("))
            {
                categories["2. Combat Skills, Formulas & Buffs"].Add(item);
            }
            else if (cn.Contains("：") || cn.Contains("“") || cn.Contains("”") || cn.StartsWith("<P_") || en.Contains("\"") || en.Contains("...") || en.Contains("—") || cn.Contains("！") || cn.Contains("？"))
            {
                categories["1. Dialogues & Story (Quests, NPC, Subtitles)"].Add(item);
            }
            else if (en.IndexOf("skill", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("cooldown", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("buff", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("stance", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                categories["2. Combat Skills, Formulas & Buffs"].Add(item);
            }
            else if (en.IndexOf("potion", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("characteristic", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("artifact", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("sword", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("material", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("item", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("recipe", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                categories["3. Items, Weapons, Equipment & Potions"].Add(item);
            }
            else if (en.IndexOf("diary", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("letter", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("record", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("tales", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("chronicle", StringComparison.OrdinalIgnoreCase) >= 0 || en.Length > 200)
            {
                categories["5. Lore, Books, Notes & Diaries"].Add(item);
            }
            else if (en.IndexOf("dungeon", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("street", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("city", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("manor", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("borough", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("tingic", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("tingen", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("backlund", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                categories["6. World Map, Locations, Dungeons & Instances"].Add(item);
            }
            else if (en.IndexOf("button", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("click", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("confirm", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("cancel", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("settings", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("press", StringComparison.OrdinalIgnoreCase) >= 0 || en.IndexOf("select", StringComparison.OrdinalIgnoreCase) >= 0 || en.Length < 25)
            {
                categories["4. UI, Menus, Settings & System Prompts"].Add(item);
            }
            else if (Regex.IsMatch(cn, @"^[A-Za-z0-9_#%:\-\.]+$"))
            {
                categories["8. Technical & Debug / Internal Keys"].Add(item);
            }
            else
            {
                categories["9. Other / Uncategorized"].Add(item);
            }
        }

        foreach (var cat in categories)
        {
            Console.WriteLine(string.Format("  {0,-55} : {1,5} entries", cat.Key, cat.Value.Count));
        }

        // Save detailed categories to a file
        string catReport = Path.Combine(rootDir, "tools", "patch_2.6_untranslated_categories.txt");
        using (var writer = new StreamWriter(catReport, false, Encoding.UTF8))
        {
            foreach (var cat in categories)
            {
                writer.WriteLine("========================================================================");
                writer.WriteLine(string.Format("=== {0} ({1} entries) ===", cat.Key, cat.Value.Count));
                writer.WriteLine("========================================================================");
                int count = 0;
                foreach (var item in cat.Value)
                {
                    count++;
                    writer.WriteLine(string.Format("[{0}] CN: {1}", count, Escape(item.Key)));
                    writer.WriteLine(string.Format("    EN: {0}", Escape(item.Value)));
                    writer.WriteLine();
                }
                writer.WriteLine();
            }
        }
        Console.WriteLine("\nDetailed category breakdown saved to: " + catReport);
    }

    static void ParseLuaTable(string filePath, Dictionary<string, string> dict)
    {
        using (var reader = new StreamReader(filePath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (!line.StartsWith("[\"")) continue;
                int firstQuote = 2;
                int keyEndQuote = FindClosingQuote(line, firstQuote);
                if (keyEndQuote == -1) continue;
                string key = Unescape(line.Substring(firstQuote, keyEndQuote - firstQuote));

                int eqIdx = line.IndexOf('=', keyEndQuote);
                if (eqIdx == -1) continue;
                int valStartQuote = line.IndexOf('"', eqIdx);
                if (valStartQuote == -1) continue;
                valStartQuote++;
                int valEndQuote = FindClosingQuote(line, valStartQuote);
                if (valEndQuote == -1) continue;
                string val = Unescape(line.Substring(valStartQuote, valEndQuote - valStartQuote));
                dict[key] = val;
            }
        }
    }

    static int FindClosingQuote(string s, int start)
    {
        for (int i = start; i < s.Length; i++)
        {
            if (s[i] == '"' && (i == 0 || s[i - 1] != '\\')) return i;
        }
        return -1;
    }

    static string Unescape(string s)
    {
        return s.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\\", "\\");
    }

    static string Escape(string s)
    {
        if (s == null) return "";
        return s.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }
}
