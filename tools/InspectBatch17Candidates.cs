using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectBatch17Candidates
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

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
                        string k = t.Substring(2, delim - 2);
                        existingRu.Add(k);
                    }
                }
            }
        }
        Console.WriteLine("Existing entries in Ru dictionary: " + existingRu.Count);

        var seen = new HashSet<string>(StringComparer.Ordinal);

        var uiPrompts = new List<Tuple<string, string>>();
        var richTextStory = new List<Tuple<string, string>>();
        var shortUiAndMenu = new List<Tuple<string, string>>();
        var sentences = new List<Tuple<string, string>>();
        var mechanicsAndSkills = new List<Tuple<string, string>>();
        var otherText = new List<Tuple<string, string>>();

        int totalUntranslated = 0;

        using (var reader = new StreamReader(GeminiPath, Encoding.UTF8))
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
                        string cnKey = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string enVal = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        if (existingRu.Contains(cnKey) || existingRu.Contains(enVal) || seen.Contains(cnKey) || seen.Contains(enVal))
                            continue;

                        seen.Add(cnKey);
                        seen.Add(enVal);

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        // Исключаем технический мусор
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") ||
                            enVal.StartsWith(".p4config") || enVal.Contains("-----") || enVal.Contains(".cpp") || enVal.Contains(".h") ||
                            enVal.StartsWith("523") || enVal.StartsWith("520") || enVal.StartsWith("GTA-") || enVal.StartsWith("GTA ") ||
                            enVal.StartsWith("Camera_") || enVal.StartsWith("Mat_") || enVal.StartsWith("SK_") || enVal.StartsWith("SM_"))
                            continue;

                        totalUntranslated++;

                        if (enVal.StartsWith("<") && enVal.Contains("</>"))
                        {
                            richTextStory.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.StartsWith("<") || enVal.EndsWith(">") || enVal.Contains("Prompt") || enVal.Contains("Notice") ||
                                 enVal.Contains("Tips") || enVal.Contains("Failed") || enVal.Contains("Success") || enVal.Contains("Unlock") ||
                                 enVal.Contains("Warning") || enVal.Contains("Confirm") || enVal.Contains("Cancel"))
                        {
                            uiPrompts.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.Contains("Skill") || enVal.Contains("Cooldown") || enVal.Contains("Damage") || enVal.Contains("Buff") ||
                                 enVal.Contains("Debuff") || enVal.Contains("Target") || enVal.Contains("Range") || enVal.Contains("Radius") ||
                                 cnKey.Contains("技能") || cnKey.Contains("冷却") || cnKey.Contains("伤害") || cnKey.Contains("效果"))
                        {
                            mechanicsAndSkills.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.Contains(" ") && (enVal.EndsWith(".") || enVal.EndsWith("!") || enVal.EndsWith("?") || enVal.Contains(",") || enVal.Length > 35))
                        {
                            sentences.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.Length < 25 && !enVal.Contains("_") && Regex.IsMatch(enVal, @"^[A-Z][a-zA-Z\s]+$"))
                        {
                            shortUiAndMenu.Add(Tuple.Create(cnKey, enVal));
                        }
                        else
                        {
                            otherText.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Total Candidate Pool: {0}", totalUntranslated));
        Console.WriteLine(string.Format("1. RichText Story/Quests: {0}", richTextStory.Count));
        Console.WriteLine(string.Format("2. UI Prompts, Warnings, Tips: {0}", uiPrompts.Count));
        Console.WriteLine(string.Format("3. Combat, Mechanics, Skills: {0}", mechanicsAndSkills.Count));
        Console.WriteLine(string.Format("4. Full Sentences, Lore, Dialogues: {0}", sentences.Count));
        Console.WriteLine(string.Format("5. Short UI, Menu, Names: {0}", shortUiAndMenu.Count));
        Console.WriteLine(string.Format("6. Other Text: {0}", otherText.Count));

        Console.WriteLine("\n--- Sample RichText Story/Quests (5) ---");
        for (int i = 0; i < Math.Min(5, richTextStory.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", richTextStory[i].Item2, richTextStory[i].Item1));

        Console.WriteLine("\n--- Sample UI Prompts (5) ---");
        for (int i = 0; i < Math.Min(5, uiPrompts.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", uiPrompts[i].Item2, uiPrompts[i].Item1));

        Console.WriteLine("\n--- Sample Combat Mechanics (5) ---");
        for (int i = 0; i < Math.Min(5, mechanicsAndSkills.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", mechanicsAndSkills[i].Item2, mechanicsAndSkills[i].Item1));

        Console.WriteLine("\n--- Sample Full Sentences (10) ---");
        for (int i = 0; i < Math.Min(10, sentences.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", sentences[i].Item2, sentences[i].Item1));

        Console.WriteLine("\n--- Sample Short UI (10) ---");
        for (int i = 0; i < Math.Min(10, shortUiAndMenu.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", shortUiAndMenu[i].Item2, shortUiAndMenu[i].Item1));

        Console.WriteLine("\n--- Sample Other Text (10) ---");
        for (int i = 0; i < Math.Min(10, otherText.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", otherText[i].Item2, otherText[i].Item1));
    }
}
