using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class FilterUntranslatedStrings
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string rootDir = @"d:\gameDev\translate lotm";
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");
        string ruMasterFile = Path.Combine(rootDir, @"RuntimeTextRussian.lua");

        var en26Entries = new Dictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shards26Dir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);
        foreach (var shardPath in shardFiles) ParseLuaTable(shardPath, en26Entries);

        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(ruMasterFile, ruDict);

        var realStrings = new List<KeyValuePair<string, string>>();
        var devPlaceholders = new List<KeyValuePair<string, string>>();

        foreach (var kvp in en26Entries)
        {
            if (!ruDict.ContainsKey(kvp.Key) && !ruDict.ContainsKey(kvp.Value))
            {
                if (IsDevPlaceholder(kvp.Key, kvp.Value))
                {
                    devPlaceholders.Add(kvp);
                }
                else
                {
                    realStrings.Add(kvp);
                }
            }
        }

        Console.WriteLine("=== UNTRANSLATED STRINGS ANALYSIS ===");
        Console.WriteLine("Total untranslated in 2.6.0:           " + (realStrings.Count + devPlaceholders.Count));
        Console.WriteLine("  -> Real player-facing strings:       " + realStrings.Count);
        Console.WriteLine("  -> Dev placeholder / dummy test:     " + devPlaceholders.Count);

        // Export real strings to a clean TSV for translation
        string realTsv = Path.Combine(rootDir, "tools", "patch_2.6_real_untranslated.tsv");
        using (var writer = new StreamWriter(realTsv, false, Encoding.UTF8))
        {
            writer.WriteLine("CN\tEN");
            foreach (var item in realStrings)
            {
                writer.WriteLine(Escape(item.Key) + "\t" + Escape(item.Value));
            }
        }
        Console.WriteLine("\nSaved real untranslated strings to: " + realTsv);
    }

    static bool IsDevPlaceholder(string cn, string en)
    {
        if (cn.Contains("占位") || en.IndexOf("placeholder", StringComparison.OrdinalIgnoreCase) >= 0) return true;
        if (cn.Contains("文本文本") || en.IndexOf("text text text", StringComparison.OrdinalIgnoreCase) >= 0) return true;
        if (cn.Contains("测试") && (cn.Length < 10 || cn.Contains("test"))) return true;
        if (cn.Contains("一二三四五六七八九十")) return true;
        if (cn.Contains("11111") || cn.Contains("aaaaa") || cn.Contains("asdf")) return true;
        if (Regex.IsMatch(cn, @"^[\d\s\-_:\.]+$") && Regex.IsMatch(en, @"^[\d\s\-_:\.]+$")) return true;
        if (cn.Contains("七个字") && en.Contains("Seven Chars")) return true;
        return false;
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
