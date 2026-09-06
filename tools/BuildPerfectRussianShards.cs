using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class BuildPerfectRussianShards
{
    static int SourceKeyHash(byte[] bytes)
    {
        uint hash = 2166136261u;
        for (int i = 0; i < bytes.Length; i++)
        {
            hash = hash ^ bytes[i];
            uint h1 = hash << 1;
            uint h4 = hash << 4;
            uint h7 = hash << 7;
            uint h8 = hash << 8;
            uint h24 = hash << 24;
            hash = unchecked(hash + h1 + h4 + h7 + h8 + h24);
        }
        return unchecked((int)hash);
    }

    static string GetPrefix(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        int hash = SourceKeyHash(bytes);
        string hex = ((uint)hash).ToString("x8");
        string hashPrefix = hex.Substring(0, 3);
        int num = Convert.ToInt32(hashPrefix, 16);
        int shardNum = num / 4;
        return shardNum.ToString("x3");
    }

    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "");
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        string geminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
        string outDir = @"d:\gameDev\translate lotm\data\shards";
        string modBaseDir = @"d:\gameDev\translate lotm\mod_base\Saved\Mods\lua\mods\cpdd_runtime_fixes";

        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
        if (!Directory.Exists(modBaseDir)) Directory.CreateDirectory(modBaseDir);

        Console.WriteLine("=== STEP 1: Loading Russian Dictionary ===");
        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        using (var reader = new StreamReader(ruPath, Encoding.UTF8))
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

                        string unescapedKey = Regex.Unescape(k);
                        string unescapedVal = Regex.Unescape(v);
                        ruDict[unescapedKey] = unescapedVal;
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Loaded {0} entries from RuntimeTextRussian.lua.", ruDict.Count));

        Console.WriteLine("=== STEP 2: Initializing 1024 Shards ===");
        var shardData = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
        for (int i = 0; i < 1024; i++)
        {
            shardData[i.ToString("x3")] = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        Console.WriteLine("=== STEP 3: Indexing source_en/RuntimeTextGemini.lua ===");
        int geminiCount = 0;
        int ruMatchedFromGemini = 0;
        using (var reader = new StreamReader(geminiPath, Encoding.UTF8))
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
                        geminiCount++;
                        string cnKeyEscaped = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string enValEscaped = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        string cnRaw = Regex.Unescape(cnKeyEscaped);
                        string enRaw = Regex.Unescape(enValEscaped);

                        string ruVal;
                        if (!ruDict.TryGetValue(cnRaw, out ruVal) || string.IsNullOrWhiteSpace(ruVal))
                        {
                            if (!ruDict.TryGetValue(enRaw, out ruVal) || string.IsNullOrWhiteSpace(ruVal))
                            {
                                ruVal = enRaw;
                            }
                            else
                            {
                                ruMatchedFromGemini++;
                            }
                        }
                        else
                        {
                            ruMatchedFromGemini++;
                        }

                        // Index CN key into its exact hash shard
                        string cnPrefix = GetPrefix(cnRaw);
                        shardData[cnPrefix][cnRaw] = ruVal;

                        // Index EN key into its exact hash shard
                        string enPrefix = GetPrefix(enRaw);
                        shardData[enPrefix][enRaw] = ruVal;
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Indexed {0} Gemini source entries (RU matched: {1}).", geminiCount, ruMatchedFromGemini));

        Console.WriteLine("=== STEP 4: Indexing remaining entries from ruDict ===");
        int extraRu = 0;
        foreach (var kvp in ruDict)
        {
            string prefix = GetPrefix(kvp.Key);
            if (!shardData[prefix].ContainsKey(kvp.Key))
            {
                shardData[prefix][kvp.Key] = kvp.Value;
                extraRu++;
            }
        }
        Console.WriteLine(string.Format("Added {0} extra Russian dictionary keys not in Gemini source.", extraRu));

        Console.WriteLine("=== STEP 5: Writing all 1024 Shards to data/shards and mod_base ===");
        long totalEntriesWritten = 0;
        for (int i = 0; i < 1024; i++)
        {
            string prefix = i.ToString("x3");
            var map = shardData[prefix];
            totalEntriesWritten += map.Count;

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("-- Generated Russian translation shard {0}/3ff.", prefix));
            sb.AppendLine("-- Dual-indexed (CN + EN keys) for instant zero-limit lookup.");
            sb.AppendLine("return {");
            foreach (var kvp in map)
            {
                sb.AppendLine(string.Format("    [\"{0}\"] = \"{1}\",", CleanForLua(kvp.Key), CleanForLua(kvp.Value)));
            }
            sb.AppendLine("}");

            string text = sb.ToString();
            string outPath = Path.Combine(outDir, "RuntimeTextGemini_" + prefix + ".lua");
            string modBasePath = Path.Combine(modBaseDir, "RuntimeTextGemini_" + prefix + ".lua");

            File.WriteAllText(outPath, text, Encoding.UTF8);
            File.WriteAllText(modBasePath, text, Encoding.UTF8);
        }

        Console.WriteLine(string.Format("Successfully generated 1024 shards with {0} total entries!", totalEntriesWritten));
    }
}
