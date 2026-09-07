using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Inspect35
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string rootDir = @"d:\gameDev\translate lotm";
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");
        string oldSourceEnFile = Path.Combine(rootDir, @"source_en\RuntimeTextGemini.lua.bak");
        string ruMasterFile = Path.Combine(rootDir, @"RuntimeTextRussian.lua");

        var en26Entries = new Dictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shards26Dir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);
        foreach (var shardPath in shardFiles) ParseLuaTable(shardPath, en26Entries);

        var oldEnEntries = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(oldSourceEnFile, oldEnEntries);

        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(ruMasterFile, ruDict);

        int idx = 0;
        foreach (var kvp in en26Entries)
        {
            string oldVal;
            if (oldEnEntries.TryGetValue(kvp.Key, out oldVal) && oldVal != kvp.Value)
            {
                idx++;
                Console.WriteLine(string.Format("=== [{0}] ===", idx));
                Console.WriteLine("CN:     " + Escape(kvp.Key));
                Console.WriteLine("Old EN: " + Escape(oldVal));
                Console.WriteLine("New EN: " + Escape(kvp.Value));
                string ruVal;
                if (ruDict.TryGetValue(kvp.Key, out ruVal)) Console.WriteLine("Cur RU: " + Escape(ruVal));
                else if (ruDict.TryGetValue(oldVal, out ruVal)) Console.WriteLine("Old RU: " + Escape(ruVal));
                Console.WriteLine();
            }
        }
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
