using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class UpdateSourceEn26
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string rootDir = @"d:\gameDev\translate lotm";
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");
        string outGemini = Path.Combine(rootDir, @"source_en\RuntimeTextGemini.lua");

        Console.WriteLine("Loading all 1,024 shards from 2.6.0...");
        var en26Entries = new SortedDictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shards26Dir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);

        foreach (var shardPath in shardFiles)
        {
            ParseLuaTable(shardPath, en26Entries);
        }
        Console.WriteLine("Total unique CN->EN entries: " + en26Entries.Count);

        // Backup existing
        string bakPath = outGemini + ".bak";
        if (File.Exists(outGemini) && !File.Exists(bakPath))
        {
            File.Copy(outGemini, bakPath);
            Console.WriteLine("Created backup at " + bakPath);
        }

        Console.WriteLine("Writing updated source_en/RuntimeTextGemini.lua...");
        using (var writer = new StreamWriter(outGemini, false, Encoding.UTF8))
        {
            writer.WriteLine("-- Extracted and merged from English Patch v2.6.0 (1,024 shards).");
            writer.WriteLine("-- Total unique entries: " + en26Entries.Count);
            writer.WriteLine("return {");
            foreach (var kvp in en26Entries)
            {
                writer.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", CleanForLua(kvp.Key), CleanForLua(kvp.Value)));
            }
            writer.WriteLine("}");
        }
        Console.WriteLine("Successfully wrote " + en26Entries.Count + " entries to " + outGemini);
    }

    static void ParseLuaTable(string filePath, SortedDictionary<string, string> dict)
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

    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\n").Replace("\r", "");
        r = r.Replace("\\", "\\\\");
        r = r.Replace("\"", "\\\"");
        r = r.Replace("\n", "\\n");
        r = r.Replace("\t", "\\t");
        return r;
    }
}
