using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class ClassifyRemaining
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

    static bool IsTechnicalCodeOrDebug(string enVal, string cnKey)
    {
        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
            return true;

        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") ||
            enVal.StartsWith(".p4config") || enVal.Contains("-----") || enVal.Contains(".cpp") || enVal.Contains(".h") ||
            enVal.StartsWith("523") || enVal.StartsWith("520") || enVal.StartsWith("GTA-") || enVal.StartsWith("GTA ") ||
            enVal.StartsWith("Camera_") || enVal.StartsWith("Mat_") || enVal.StartsWith("SK_") || enVal.StartsWith("SM_") ||
            enVal.StartsWith("SkillID: ") || enVal.Contains("InterruptMode") || enVal.Contains("BindSkillID") ||
            enVal.StartsWith("AI ") || enVal.Contains("AI ") || enVal.Contains("AOI Primary Layer") ||
            enVal.Contains("ExcelCfg") || enVal.Contains("LuaList") || enVal.Contains("returned nil") ||
            enVal.Contains("failed to retrieve") || enVal.Contains("GetTask") || enVal.Contains("QuestSystem") ||
            enVal.Contains("Combat Attribute") || enVal.Contains("Attribute mode") ||
            enVal.Contains("_Panel") || enVal.Contains("_Item") || enVal.Contains("MailId") ||
            Regex.IsMatch(enVal, @"^\d+:\s*(Disable|Enable|Hide|Start|End|Override)") ||
            Regex.IsMatch(enVal, @"^\d+-(Disable|Enable|Correct)"))
            return true;

        return false;
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var existingRu = new HashSet<string>(StringComparer.Ordinal);
        if (File.Exists(RuPath))
        {
            foreach (var line in File.ReadAllLines(RuPath, Encoding.UTF8))
            {
                string t = line.Trim();
                if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 0)
                    {
                        existingRu.Add(t.Substring(2, delim - 2));
                    }
                }
            }
        }

        int totalGemini = 0;
        int alreadyTranslated = 0;
        var candidates = new List<Tuple<string, string>>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        using (var reader = new StreamReader(GeminiPath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string t = line.Trim();
                if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
                {
                    totalGemini++;
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 0)
                    {
                        string cnKey = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string enVal = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        if (existingRu.Contains(cnKey) || existingRu.Contains(enVal))
                        {
                            alreadyTranslated++;
                            continue;
                        }

                        if (seen.Contains(cnKey) || seen.Contains(enVal))
                            continue;
                        seen.Add(cnKey);
                        seen.Add(enVal);

                        if (!IsTechnicalCodeOrDebug(enVal, cnKey))
                        {
                            candidates.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Total Gemini entries: {0}", totalGemini));
        Console.WriteLine(string.Format("Already matched in Russian: {0} ({1:F2}%)", alreadyTranslated, (alreadyTranslated * 100.0 / totalGemini)));
        Console.WriteLine(string.Format("Clean Candidates available: {0}", candidates.Count));
    }
}
