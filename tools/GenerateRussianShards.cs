using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class GenerateRussianShards
{
    // Lua bit.tobit(x) treats as int32
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
        string shardsSrcDir = @"d:\gameDev\translate lotm\mod_base\Saved\Mods\lua\mods\cpdd_runtime_fixes";
        string outDir = @"d:\gameDev\translate lotm\data\shards";

        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

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
                        
                        // We unescape \n, \", etc. to have raw string keys
                        string unescapedKey = Regex.Unescape(k);
                        string unescapedVal = Regex.Unescape(v);
                        ruDict[unescapedKey] = unescapedVal;
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Loaded {0} Russian dictionary entries.", ruDict.Count));

        Console.WriteLine("=== STEP 2: Processing 1024 Shards ===");
        var shardFiles = Directory.GetFiles(shardsSrcDir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);
        Console.WriteLine(string.Format("Found {0} shard files.", shardFiles.Length));

        // We will store all shard entries: prefix -> Dictionary<rawKey, rawVal>
        var shardData = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
        for (int i = 0; i < 1024; i++)
        {
            shardData[i.ToString("x3")] = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        int totalChineseKeys = 0;
        int russianTranslatedCn = 0;
        int fallbackEnglish = 0;

        foreach (var file in shardFiles)
        {
            string prefix = Path.GetFileNameWithoutExtension(file).Replace("RuntimeTextGemini_", "").ToLower();
            if (!shardData.ContainsKey(prefix))
            {
                shardData[prefix] = new Dictionary<string, string>(StringComparer.Ordinal);
            }

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
                            totalChineseKeys++;
                            string cnKeyEscaped = t.Substring(2, delim - 2);
                            int valStart = delim + 6;
                            int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                            string enValEscaped = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                            string cnRaw = Regex.Unescape(cnKeyEscaped);
                            string enRaw = Regex.Unescape(enValEscaped);

                            string ruVal = null;
                            if (ruDict.TryGetValue(cnRaw, out ruVal) && !string.IsNullOrWhiteSpace(ruVal))
                            {
                                russianTranslatedCn++;
                            }
                            else if (ruDict.TryGetValue(enRaw, out ruVal) && !string.IsNullOrWhiteSpace(ruVal))
                            {
                                russianTranslatedCn++;
                            }
                            else
                            {
                                ruVal = enRaw;
                                fallbackEnglish++;
                            }

                            shardData[prefix][cnRaw] = ruVal;

                            // Also index the English key into its own hash shard if not already present
                            string enPrefix = GetPrefix(enRaw);
                            if (shardData.ContainsKey(enPrefix) && !shardData[enPrefix].ContainsKey(enRaw))
                            {
                                shardData[enPrefix][enRaw] = ruVal;
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Total Chinese keys: {0}", totalChineseKeys));
        Console.WriteLine(string.Format("Translated to Russian: {0} ({1:F2}%)", russianTranslatedCn, (russianTranslatedCn * 100.0 / totalChineseKeys)));
        Console.WriteLine(string.Format("Fallback English: {0}", fallbackEnglish));

        Console.WriteLine("=== STEP 3: Writing Russian Shards to data/shards and mod_base ===");
        int totalWrittenEntries = 0;
        for (int i = 0; i < 1024; i++)
        {
            string prefix = i.ToString("x3");
            var entries = shardData[prefix];
            totalWrittenEntries += entries.Count;

            string outPath = Path.Combine(outDir, "RuntimeTextGemini_" + prefix + ".lua");
            string modBasePath = Path.Combine(shardsSrcDir, "RuntimeTextGemini_" + prefix + ".lua");

            using (var sw = new StreamWriter(outPath, false, new UTF8Encoding(false)))
            {
                sw.WriteLine("-- Generated Russian translation shard " + prefix + "/3ff.");
                sw.WriteLine("-- Dual-indexed (CN + EN keys) for instant zero-limit lookup.");
                sw.WriteLine("return {");
                foreach (var kvp in entries)
                {
                    sw.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", CleanForLua(kvp.Key), CleanForLua(kvp.Value)));
                }
                sw.WriteLine("}");
            }

            // Sync to mod_base as well
            File.Copy(outPath, modBasePath, true);

            if ((i + 1) % 128 == 0 || i == 1023)
            {
                Console.WriteLine(string.Format("  Written {0} / 1024 shards...", i + 1));
            }
        }

        Console.WriteLine(string.Format("All 1024 Russian shards successfully generated! Total entries: {0}", totalWrittenEntries));
    }
}
