using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class AnalyzePatch26Comprehensive
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string rootDir = @"d:\gameDev\translate lotm";
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");
        string oldSourceEnFile = Path.Combine(rootDir, @"source_en\RuntimeTextGemini.lua");
        string ruMasterFile = Path.Combine(rootDir, @"RuntimeTextRussian.lua");

        Console.WriteLine("========================================================================");
        Console.WriteLine("  LORD OF THE MYSTERIES - FULL COMPREHENSIVE TRANSLATION AUDIT (v2.6.0)  ");
        Console.WriteLine("========================================================================");

        // 1. Read all 2.6 shards
        Console.WriteLine("\n[1/5] Loading all 1,024 shards from English Patch 2.6.0...");
        var en26Entries = new Dictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shards26Dir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);

        foreach (var shardPath in shardFiles)
        {
            ParseLuaTable(shardPath, en26Entries);
        }
        Console.WriteLine("Loaded " + shardFiles.Length + " shards.");
        Console.WriteLine("Total unique CN->EN entries in English 2.6.0: " + en26Entries.Count);

        // 2. Read old English source (source_en/RuntimeTextGemini.lua)
        Console.WriteLine("\n[2/5] Loading previous English source (source_en/RuntimeTextGemini.lua)...");
        var oldEnEntries = new Dictionary<string, string>(StringComparer.Ordinal);
        if (File.Exists(oldSourceEnFile))
        {
            ParseLuaTable(oldSourceEnFile, oldEnEntries);
            Console.WriteLine("Total unique CN->EN entries in old English source: " + oldEnEntries.Count);
        }
        else
        {
            Console.WriteLine("WARNING: old source file not found at " + oldSourceEnFile);
        }

        // 3. Compare English 2.6.0 vs Old English source
        Console.WriteLine("\n[3/5] Comparing English 2.6.0 vs Old English source...");
        var newIn26 = new List<KeyValuePair<string, string>>();
        var removedIn26 = new List<KeyValuePair<string, string>>();
        var modifiedIn26 = new List<Tuple<string, string, string>>(); // CN, OldEN, NewEN

        string oldVal;
        foreach (var kvp in en26Entries)
        {
            if (!oldEnEntries.TryGetValue(kvp.Key, out oldVal))
            {
                newIn26.Add(kvp);
            }
            else if (oldVal != kvp.Value)
            {
                modifiedIn26.Add(Tuple.Create(kvp.Key, oldVal, kvp.Value));
            }
        }

        foreach (var kvp in oldEnEntries)
        {
            if (!en26Entries.ContainsKey(kvp.Key))
            {
                removedIn26.Add(kvp);
            }
        }

        Console.WriteLine("  -> Newly added strings in 2.6.0: " + newIn26.Count);
        Console.WriteLine("  -> Removed strings in 2.6.0:      " + removedIn26.Count);
        Console.WriteLine("  -> Modified translations in 2.6.0: " + modifiedIn26.Count);

        // 4. Load Master Russian Dictionary (RuntimeTextRussian.lua)
        Console.WriteLine("\n[4/5] Loading Russian master dictionary (RuntimeTextRussian.lua)...");
        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(ruMasterFile, ruDict);
        Console.WriteLine("Total entries in RuntimeTextRussian.lua: " + ruDict.Count);

        // 5. Check Coverage of English 2.6.0 in Russian Translation
        Console.WriteLine("\n[5/5] Auditing Russian translation coverage against English 2.6.0...");
        var missingCnInRu = new List<KeyValuePair<string, string>>();
        var missingEnInRu = new List<KeyValuePair<string, string>>();
        var fullyUntranslated = new List<KeyValuePair<string, string>>(); // neither CN nor EN is in ruDict

        foreach (var kvp in en26Entries)
        {
            string cn = kvp.Key;
            string en = kvp.Value;
            bool hasCn = ruDict.ContainsKey(cn);
            bool hasEn = ruDict.ContainsKey(en);

            if (!hasCn && !hasEn)
            {
                fullyUntranslated.Add(kvp);
            }
            else
            {
                if (!hasCn) missingCnInRu.Add(kvp);
                if (!hasEn) missingEnInRu.Add(kvp);
            }
        }

        Console.WriteLine("  -> Fully untranslated (neither CN nor EN key exists in RU): " + fullyUntranslated.Count);
        Console.WriteLine("  -> Has EN key but missing CN key in RU:                    " + missingCnInRu.Count);
        Console.WriteLine("  -> Has CN key but missing EN key in RU:                    " + missingEnInRu.Count);
        int translatedCount = en26Entries.Count - fullyUntranslated.Count;
        double coveragePct = (double)translatedCount / en26Entries.Count * 100.0;
        Console.WriteLine(string.Format("  -> OVERALL COVERAGE: {0} / {1} ({2:F2}%)", translatedCount, en26Entries.Count, coveragePct));

        // Output details to reports
        string reportPath = Path.Combine(rootDir, "tools", "patch_2.6_audit_report.txt");
        using (var writer = new StreamWriter(reportPath, false, Encoding.UTF8))
        {
            writer.WriteLine("========================================================================");
            writer.WriteLine("  LORD OF THE MYSTERIES - FULL COMPREHENSIVE TRANSLATION AUDIT (v2.6.0)  ");
            writer.WriteLine("========================================================================");
            writer.WriteLine("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            writer.WriteLine("Total entries in English 2.6.0: " + en26Entries.Count);
            writer.WriteLine("Total entries in Old English source: " + oldEnEntries.Count);
            writer.WriteLine("Total entries in RuntimeTextRussian.lua: " + ruDict.Count);
            writer.WriteLine(string.Format("Overall Coverage: {0} / {1} ({2:F2}%)\n", translatedCount, en26Entries.Count, coveragePct));

            writer.WriteLine("------------------------------------------------------------------------");
            writer.WriteLine("1. NEWLY ADDED ENTRIES IN ENGLISH 2.6.0 (" + newIn26.Count + "):");
            writer.WriteLine("------------------------------------------------------------------------");
            foreach (var item in newIn26)
            {
                bool inRuCn = ruDict.ContainsKey(item.Key);
                bool inRuEn = ruDict.ContainsKey(item.Value);
                writer.WriteLine(string.Format("CN: {0}", Escape(item.Key)));
                writer.WriteLine(string.Format("EN: {0}", Escape(item.Value)));
                writer.WriteLine(string.Format("RU Status: CN_in_RU={0}, EN_in_RU={1}", inRuCn, inRuEn));
                string ruValOut;
                if (ruDict.TryGetValue(item.Key, out ruValOut)) writer.WriteLine(string.Format("RU Value: {0}", Escape(ruValOut)));
                else if (ruDict.TryGetValue(item.Value, out ruValOut)) writer.WriteLine(string.Format("RU Value: {0}", Escape(ruValOut)));
                writer.WriteLine();
            }

            writer.WriteLine("------------------------------------------------------------------------");
            writer.WriteLine("2. MODIFIED TRANSLATIONS IN ENGLISH 2.6.0 (" + modifiedIn26.Count + "):");
            writer.WriteLine("------------------------------------------------------------------------");
            foreach (var item in modifiedIn26)
            {
                writer.WriteLine(string.Format("CN:     {0}", Escape(item.Item1)));
                writer.WriteLine(string.Format("Old EN: {0}", Escape(item.Item2)));
                writer.WriteLine(string.Format("New EN: {0}", Escape(item.Item3)));
                string ruValOut;
                if (ruDict.TryGetValue(item.Item1, out ruValOut))
                {
                    writer.WriteLine(string.Format("RU Val: {0}", Escape(ruValOut)));
                }
                else if (ruDict.TryGetValue(item.Item2, out ruValOut))
                {
                    writer.WriteLine(string.Format("RU (by Old EN): {0}", Escape(ruValOut)));
                }
                else
                {
                    writer.WriteLine("RU: NOT TRANSLATED");
                }
                writer.WriteLine();
            }

            writer.WriteLine("------------------------------------------------------------------------");
            writer.WriteLine("3. FULLY UNTRANSLATED ENTRIES IN 2.6.0 (" + fullyUntranslated.Count + "):");
            writer.WriteLine("------------------------------------------------------------------------");
            foreach (var item in fullyUntranslated)
            {
                writer.WriteLine(string.Format("CN: {0}", Escape(item.Key)));
                writer.WriteLine(string.Format("EN: {0}", Escape(item.Value)));
                writer.WriteLine();
            }

            writer.WriteLine("------------------------------------------------------------------------");
            writer.WriteLine("4. MISSING EN KEYS (CN is translated, but exact EN key not in RU) (" + missingEnInRu.Count + "):");
            writer.WriteLine("------------------------------------------------------------------------");
            foreach (var item in missingEnInRu)
            {
                writer.WriteLine(string.Format("CN: {0}", Escape(item.Key)));
                writer.WriteLine(string.Format("EN: {0}", Escape(item.Value)));
                writer.WriteLine(string.Format("RU: {0}", Escape(ruDict[item.Key])));
                writer.WriteLine();
            }

            writer.WriteLine("------------------------------------------------------------------------");
            writer.WriteLine("5. MISSING CN KEYS (EN is in RU, but exact CN key not in RU) (" + missingCnInRu.Count + "):");
            writer.WriteLine("------------------------------------------------------------------------");
            foreach (var item in missingCnInRu)
            {
                writer.WriteLine(string.Format("CN: {0}", Escape(item.Key)));
                writer.WriteLine(string.Format("EN: {0}", Escape(item.Value)));
                writer.WriteLine(string.Format("RU: {0}", Escape(ruDict[item.Value])));
                writer.WriteLine();
            }
        }
        Console.WriteLine("\nFull detailed report saved to: " + reportPath);
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

                int firstQuote = 2; // after ["
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
            if (s[i] == '"' && (i == 0 || s[i - 1] != '\\'))
            {
                return i;
            }
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
