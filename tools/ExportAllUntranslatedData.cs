using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class ExportAllUntranslatedData
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string geminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        string initPath = @"d:\gameDev\translate lotm\data\Init.lua";
        string ruLocPath = @"d:\gameDev\translate lotm\RussianLocalization.lua";

        Console.WriteLine("=== EXPORTING ALL UNTRANSLATED DATA ===");

        // 1. Load Russian dictionary
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
                        ruDict[k] = v;
                    }
                }
            }
        }

        // 2. Classify Gemini strings
        var missingCn = new List<Tuple<string, string>>();
        var missingEn = new List<Tuple<string, string>>();
        var identicalToEn = new List<Tuple<string, string>>();

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
                        string cn = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string en = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        if (!ruDict.ContainsKey(cn))
                        {
                            missingCn.Add(Tuple.Create(cn, en));
                        }

                        if (!ruDict.ContainsKey(en))
                        {
                            missingEn.Add(Tuple.Create(cn, en));
                        }

                        string ru = null;
                        if (ruDict.ContainsKey(cn)) ru = ruDict[cn];
                        else if (ruDict.ContainsKey(en)) ru = ruDict[en];

                        if (ru != null && ru.Trim() == en.Trim())
                        {
                            identicalToEn.Add(Tuple.Create(cn, en));
                        }
                    }
                }
            }
        }

        File.WriteAllLines(@"d:\gameDev\translate lotm\tools\untranslated_identical_to_en.tsv", 
            GetTsvLines(identicalToEn), Encoding.UTF8);
        Console.WriteLine("Exported identical to EN: " + identicalToEn.Count);

        File.WriteAllLines(@"d:\gameDev\translate lotm\tools\untranslated_missing_cn_keys.tsv", 
            GetTsvLines(missingCn), Encoding.UTF8);
        Console.WriteLine("Exported missing CN keys: " + missingCn.Count);

        File.WriteAllLines(@"d:\gameDev\translate lotm\tools\untranslated_missing_en_keys.tsv", 
            GetTsvLines(missingEn), Encoding.UTF8);
        Console.WriteLine("Exported missing EN keys: " + missingEn.Count);

        // 3. Check Init.lua overrides missing in RussianLocalization.lua
        var missingOverrides = new List<string>();
        string ruLoc = File.ReadAllText(ruLocPath, Encoding.UTF8);
        string[] initLines = File.ReadAllLines(initPath, Encoding.UTF8);

        bool inConst = false;
        bool inExact = false;
        for (int i = 0; i < initLines.Length; i++)
        {
            string line = initLines[i].Trim();
            if (line.StartsWith("local stringConstOverrides = {")) { inConst = true; continue; }
            if (inConst && line == "}") { inConst = false; continue; }
            if (line.StartsWith("local visibleTextExactOverrides = {")) { inExact = true; continue; }
            if (inExact && line == "}") { inExact = false; continue; }

            if (inConst)
            {
                int eq = line.IndexOf(" = \"");
                if (eq > 0)
                {
                    string key = line.Substring(0, eq).Trim();
                    int valEnd = line.LastIndexOf("\"");
                    string val = line.Substring(eq + 4, valEnd - (eq + 4));
                    if (!ruLoc.Contains(key + " = \""))
                    {
                        missingOverrides.Add("stringConstOverrides\t" + key + "\t" + val);
                    }
                }
            }

            if (inExact)
            {
                if (line.StartsWith("[\""))
                {
                    int delim = line.IndexOf("\"] =");
                    if (delim > 0)
                    {
                        string key = line.Substring(2, delim - 2);
                        if (!ruLoc.Contains("[\"" + key + "\"]"))
                        {
                            missingOverrides.Add("visibleTextExactOverrides\t" + key + "\t" + line);
                        }
                    }
                }
            }
        }

        File.WriteAllLines(@"d:\gameDev\translate lotm\tools\untranslated_missing_init_overrides.tsv", 
            missingOverrides, Encoding.UTF8);
        Console.WriteLine("Exported missing Init.lua overrides: " + missingOverrides.Count);

        Console.WriteLine("\nAll datasets generated and saved in tools/ directory!");
    }

    static List<string> GetTsvLines(List<Tuple<string, string>> items)
    {
        var list = new List<string>(items.Count + 1);
        list.Add("CN\tEN");
        foreach (var it in items)
        {
            list.Add(it.Item1 + "\t" + it.Item2);
        }
        return list;
    }
}
