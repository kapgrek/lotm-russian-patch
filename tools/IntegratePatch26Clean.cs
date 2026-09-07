using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class IntegratePatch26Clean
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string rootDir = @"d:\gameDev\translate lotm";
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");
        string ruMasterFile = Path.Combine(rootDir, @"RuntimeTextRussian.lua");
        string cacheFile = Path.Combine(rootDir, @"tools\patch26_translation_cache.tsv");

        Console.WriteLine("========================================================================");
        Console.WriteLine("             CLEAN INTEGRATION OF TRANSLATIONS (PATCH 2.6.0)            ");
        Console.WriteLine("========================================================================");

        // 1. Load English 2.6 Shards
        Console.WriteLine("\n[1/5] Loading all 1,024 shards from 2.6...");
        var en26Entries = new Dictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shards26Dir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);
        foreach (var f in shardFiles) ParseLuaTable(f, en26Entries);
        Console.WriteLine("Loaded " + en26Entries.Count + " entries from 2.6 shards.");

        // 2. Load Russian Master Dictionary
        Console.WriteLine("\n[2/5] Loading RuntimeTextRussian.lua...");
        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(ruMasterFile, ruDict);
        Console.WriteLine("Loaded " + ruDict.Count + " existing entries in RuntimeTextRussian.lua.");

        // 3. Load Translation Cache
        Console.WriteLine("\n[3/5] Loading clean translation cache...");
        var cache = new Dictionary<string, string>(StringComparer.Ordinal);
        using (var reader = new StreamReader(cacheFile, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split('\t');
                if (parts.Length >= 2)
                {
                    string en = Unescape(parts[0]);
                    string ru = Unescape(parts[1]);
                    cache[en] = ru;
                    // Also store with normalized newlines
                    cache[en.Replace("\r\n", "\n")] = ru;
                }
            }
        }
        Console.WriteLine("Loaded " + cache.Count + " entries from cache.");

        // 4. Apply fixes for 35 modified strings and combat formula
        Console.WriteLine("\n[4/5] Applying fixes for 35 modified strings and combat formula...");
        ApplyModifiedFixes(ruDict);

        // 5. Integrate all 2.6 entries (both CN and EN keys)
        Console.WriteLine("Integrating all 2.6 entries...");
        int synced = 0;
        int newlyAdded = 0;

        foreach (var kvp in en26Entries)
        {
            string cn = kvp.Key;
            string en = kvp.Value;

            bool hasCn = ruDict.ContainsKey(cn);
            bool hasEn = ruDict.ContainsKey(en);

            if (hasCn && hasEn) continue;

            if (hasCn && !hasEn)
            {
                ruDict[en] = ruDict[cn];
                synced++;
                continue;
            }

            if (!hasCn && hasEn)
            {
                ruDict[cn] = ruDict[en];
                synced++;
                continue;
            }

            // Neither key found: lookup in cache
            string ruVal;
            if (!cache.TryGetValue(en, out ruVal))
            {
                cache.TryGetValue(en.Replace("\r\n", "\n"), out ruVal);
            }

            if (ruVal != null)
            {
                ruDict[cn] = ruVal;
                ruDict[en] = ruVal;
                newlyAdded++;
            }
            else
            {
                // Fallback translation if not in cache
                ruVal = TranslateFallback(en);
                ruDict[cn] = ruVal;
                ruDict[en] = ruVal;
                newlyAdded++;
                Console.WriteLine("Fallback translated: " + Escape(en) + " -> " + Escape(ruVal));
            }
        }

        Console.WriteLine(string.Format("Synced keys: {0}, Newly integrated: {1}", synced, newlyAdded));

        // 6. Write updated RuntimeTextRussian.lua with exact CRLF preservation
        Console.WriteLine("\n[5/5] Writing updated RuntimeTextRussian.lua with exact CRLF preservation...");
        using (var writer = new StreamWriter(ruMasterFile, false, Encoding.UTF8))
        {
            writer.WriteLine("-- Russian Master Translation Dictionary");
            writer.WriteLine("-- Total unique dual-indexed entries: " + ruDict.Count);
            writer.WriteLine("return {");
            foreach (var kvp in ruDict)
            {
                writer.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", CleanForLua(kvp.Key), CleanForLua(kvp.Value)));
            }
            writer.WriteLine("}");
        }
        Console.WriteLine("Saved RuntimeTextRussian.lua with " + ruDict.Count + " entries.");
    }

    static string TranslateFallback(string en)
    {
        if (en.Contains("Daffodil Street")) return en.Replace("Daffodil Street", "улица Нарциссов");
        if (en.Contains("Howes Street") || en.Contains("Howls Street")) return en.Replace("Howes Street", "улица Хоуэс").Replace("Howls Street", "улица Хоуэс");
        if (en.Contains("Iron Cross Street")) return en.Replace("Iron Cross Street", "улица Железного Креста");
        return en;
    }

    static void ApplyModifiedFixes(Dictionary<string, string> ruDict)
    {
        // Add Sealed Artifact formula
        string cnFormula = "普通技能进入冷却时立刻返还冷却，如果是充能技能则返还全部充能次数，单个技能最多触发1次返还。{CheckStar(Type=\"sealed\",ID=2085021)=1?返还的技能降低<Yellow>*f**</>伤害和治疗。}{CheckStar(Type=\"sealed\",ID=2085021)=3?返还的技能额外提高<Yellow>*f**</>伤害和治疗。}";
        string enFormula = "When an ordinary skill enters cooldown, the cooldown is immediately returned. If it is a charge skill, all charge counts are returned. A single skill can trigger this return at most once. {CheckStar(Type=\"sealed\",ID=2085021)=1?The returned skill deals <Yellow>*f**</> less damage and healing. }{CheckStar(Type=\"sealed\",ID=2085021)=3?The returned skill deals <Yellow>*f**</> more damage and healing. }";
        string ruFormula = "Когда обычный навык уходит на перезарядку, перезарядка немедленно сбрасывается. Если это заряжаемый навык, восстанавливаются все заряды. Сброс может сработать максимум 1 раз для одного навыка. {CheckStar(Type=\"sealed\",ID=2085021)=1?Сброшенный навык наносит на <Yellow>*f**</> меньше урона и исцеления. }{CheckStar(Type=\"sealed\",ID=2085021)=3?Сброшенный навык дополнительно наносит на <Yellow>*f**</> больше урона и исцеления. }";
        ruDict[cnFormula] = ruFormula;
        ruDict[enFormula] = ruFormula;

        // Fix gender tokens and malformed tags
        var keys = new List<string>(ruDict.Keys);
        foreach (var k in keys)
        {
            string val = ruDict[k];
            bool changed = false;

            if (val.Contains("{Mr.{ Ms.|}}") || val.Contains("{Mr.{Ms.|}}") || val.Contains("{Mr.{ Ms. |}}"))
            {
                val = val.Replace("{Mr.{ Ms.|}}", "{{мистер|мисс}}")
                         .Replace("{Mr.{Ms.|}}", "{{мистер|мисс}}")
                         .Replace("{Mr.{ Ms. |}}", "{{мистер|мисс}}");
                changed = true;
            }
            if (val.Contains("</h>"))
            {
                val = val.Replace("</h>", "</>");
                changed = true;
            }
            if (val.Contains("</mark>"))
            {
                val = val.Replace("</mark>", "</>");
                changed = true;
            }

            if (changed)
            {
                ruDict[k] = val;
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
        return s.Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\");
    }

    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\\r\\n").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
    }

    static string Escape(string s)
    {
        if (s == null) return "";
        return s.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }
}
