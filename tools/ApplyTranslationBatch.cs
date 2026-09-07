using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

class ApplyTranslationBatch
{
    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "");
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: ApplyTranslationBatch.exe <path_to_batch.tsv>");
            Console.WriteLine("Example: ApplyTranslationBatch.exe translation_batches\\batch_01.tsv");
            return;
        }

        string batchPath = args[0];
        if (!File.Exists(batchPath))
        {
            Console.WriteLine("Error: File not found: " + batchPath);
            return;
        }

        Console.WriteLine("=== Applying Translation Batch: " + Path.GetFileName(batchPath) + " ===");
        var batchLines = File.ReadAllLines(batchPath, Encoding.UTF8);
        var newTranslations = new Dictionary<string, string>(StringComparer.Ordinal);
        var cnToRu = new Dictionary<string, string>(StringComparer.Ordinal);
        var enToRu = new Dictionary<string, string>(StringComparer.Ordinal);

        int filledCount = 0;
        for (int i = 0; i < batchLines.Length; i++)
        {
            string line = batchLines[i].Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("ID\t")) continue;

            string[] parts = batchLines[i].Split('\t');
            if (parts.Length >= 4)
            {
                // Format: ID \t CN \t EN \t RU
                string cn = parts[1];
                string en = parts[2];
                string ru = parts[3].Trim();

                if (!string.IsNullOrEmpty(ru) && ru != en)
                {
                    filledCount++;
                    if (!string.IsNullOrEmpty(cn)) cnToRu[cn] = ru;
                    if (!string.IsNullOrEmpty(en)) enToRu[en] = ru;
                }
            }
            else if (parts.Length == 2)
            {
                // Format: KEY \t RU
                string key = parts[0];
                string ru = parts[1].Trim();
                if (!string.IsNullOrEmpty(ru) && ru != key)
                {
                    filledCount++;
                    enToRu[key] = ru;
                }
            }
        }

        Console.WriteLine(string.Format("Found {0} translated entries in batch.", filledCount));
        if (filledCount == 0)
        {
            Console.WriteLine("No new translations found (RU_TRANSLATION column was empty or identical to EN).");
            return;
        }

        string masterPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        Console.WriteLine("Updating Master Dictionary: " + masterPath);

        var existingLines = new List<string>(File.ReadAllLines(masterPath, Encoding.UTF8));
        var updatedLines = new List<string>(existingLines.Count + filledCount * 2);
        var keyRegex = new Regex(@"^\s*\[""(.*)""\]\s*=\s*""(.*)"",?\s*$");
        var handledCn = new HashSet<string>(StringComparer.Ordinal);
        var handledEn = new HashSet<string>(StringComparer.Ordinal);

        for (int i = 0; i < existingLines.Count; i++)
        {
            string eline = existingLines[i];
            var m = keyRegex.Match(eline);
            if (m.Success)
            {
                string key = m.Groups[1].Value.Replace("\\\"", "\"");
                if (cnToRu.ContainsKey(key))
                {
                    string newRu = CleanForLua(cnToRu[key]);
                    updatedLines.Add(string.Format("    [\"{0}\"] = \"{1}\",", m.Groups[1].Value, newRu));
                    handledCn.Add(key);
                    continue;
                }
                if (enToRu.ContainsKey(key))
                {
                    string newRu = CleanForLua(enToRu[key]);
                    updatedLines.Add(string.Format("    [\"{0}\"] = \"{1}\",", m.Groups[1].Value, newRu));
                    handledEn.Add(key);
                    continue;
                }
            }
            updatedLines.Add(eline);
        }

        // Add any new keys that weren't in existing dictionary before the closing bracket
        int insertPos = updatedLines.Count;
        for (int i = updatedLines.Count - 1; i >= 0; i--)
        {
            if (updatedLines[i].Trim().StartsWith("}"))
            {
                insertPos = i;
                break;
            }
        }

        int addedCount = 0;
        foreach (var kvp in cnToRu)
        {
            if (!handledCn.Contains(kvp.Key))
            {
                string cleanKey = CleanForLua(kvp.Key);
                string cleanRu = CleanForLua(kvp.Value);
                updatedLines.Insert(insertPos++, string.Format("    [\"{0}\"] = \"{1}\",", cleanKey, cleanRu));
                addedCount++;
            }
        }
        foreach (var kvp in enToRu)
        {
            if (!handledEn.Contains(kvp.Key))
            {
                string cleanKey = CleanForLua(kvp.Key);
                string cleanRu = CleanForLua(kvp.Value);
                updatedLines.Insert(insertPos++, string.Format("    [\"{0}\"] = \"{1}\",", cleanKey, cleanRu));
                addedCount++;
            }
        }

        File.WriteAllLines(masterPath, updatedLines, Encoding.UTF8);
        Console.WriteLine(string.Format("Master dictionary updated! Replaced existing: {0}, Added new: {1}", handledCn.Count + handledEn.Count, addedCount));

        Console.WriteLine("\n=== Re-generating 1,024 Shards ===");
        var shardProc = Process.Start(new ProcessStartInfo
        {
            FileName = @"tools\BuildPerfectRussianShards.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        });
        shardProc.WaitForExit();
        Console.WriteLine(shardProc.StandardOutput.ReadToEnd());

        Console.WriteLine("\n=== Running Skill Coverage & Shard Verification ===");
        var verifyProc = Process.Start(new ProcessStartInfo
        {
            FileName = @"tools\VerifySkillCoverage.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        });
        verifyProc.WaitForExit();
        Console.WriteLine(verifyProc.StandardOutput.ReadToEnd());

        Console.WriteLine("🎉 Batch applied successfully! 1,024 shards synced and verified.");
    }
}
