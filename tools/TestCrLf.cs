using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class TestCrLf
{
    static void Main()
    {
        string rootDir = @"d:\gameDev\translate lotm";
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");
        string ruMasterFile = Path.Combine(rootDir, @"RuntimeTextRussian.lua");

        var en26Entries = new Dictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shards26Dir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);
        foreach (var f in shardFiles) ParseLuaTable(f, en26Entries);

        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(ruMasterFile, ruDict);

        int crlfIn26 = 0;
        int crlfFoundInRu = 0;
        int crlfMissingInRu = 0;

        foreach (var kvp in en26Entries)
        {
            if (kvp.Key.Contains("\r") || kvp.Value.Contains("\r"))
            {
                crlfIn26++;
                if (ruDict.ContainsKey(kvp.Key) || ruDict.ContainsKey(kvp.Value)) crlfFoundInRu++;
                else crlfMissingInRu++;
            }
        }

        Console.WriteLine("Entries with \\r in 2.6: " + crlfIn26);
        Console.WriteLine("Found in RU:            " + crlfFoundInRu);
        Console.WriteLine("Missing in RU:          " + crlfMissingInRu);
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
}
