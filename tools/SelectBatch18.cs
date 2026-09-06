using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class SelectBatch18
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
            enVal.StartsWith("AI ") || enVal.Contains("AI ") ||
            enVal.Contains("ExcelCfg") || enVal.Contains("LuaList") || enVal.Contains("returned nil") ||
            enVal.Contains("failed to retrieve") || enVal.Contains("GetTask") || enVal.Contains("QuestSystem") ||
            enVal.Contains("Combat Attribute") || enVal.Contains("Attribute mode") ||
            Regex.IsMatch(enVal, @"^\d+:\s*(Disable|Enable|Hide|Start|End|Override)") ||
            Regex.IsMatch(enVal, @"^\d+-(Disable|Enable|Correct)"))
            return true;

        return false;
    }

    static void Main(string[] args)
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

        var listStoryAndDialogues = new List<Tuple<string, string>>();
        var listTalentsAndEquipment = new List<Tuple<string, string>>();
        var listPromptsAndActions = new List<Tuple<string, string>>();
        var listCleanUI = new List<Tuple<string, string>>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

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

                        if (IsTechnicalCodeOrDebug(enVal, cnKey))
                            continue;

                        // 1. Сюжет, диалоги, мысли, дневники, цитаты, TRPG-нарратив
                        if (enVal.Contains("<Talent>") || enVal.Contains("【") || enVal.Contains("dice") ||
                            enVal.Contains("\"") || enVal.Contains("...") || enVal.Contains("—") ||
                            (enVal.Length > 28 && (enVal.Contains("you") || enVal.Contains("You") || enVal.Contains("I ") ||
                             enVal.Contains("We ") || enVal.Contains("They ") || enVal.Contains("He ") || enVal.Contains("She ") ||
                             enVal.Contains("my ") || enVal.Contains("your ") || enVal.Contains("his ") || enVal.Contains("her ") ||
                             enVal.Contains("was ") || enVal.Contains("were ") || enVal.Contains("will ") || enVal.Contains("have ") ||
                             enVal.Contains("has ") || enVal.Contains("had "))))
                        {
                            listStoryAndDialogues.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 2. Таланты, экипировка, предметы, артефакты, оружие, духи
                        else if (enVal.Contains("Staff") || enVal.Contains("Blade") || enVal.Contains("Sword") || enVal.Contains("Ring") ||
                                 enVal.Contains("Armor") || enVal.Contains("Robe") || enVal.Contains("Pendant") || enVal.Contains("Spirit Line") ||
                                 enVal.Contains("Potion") || enVal.Contains("Formula") || enVal.Contains("Badge") || enVal.Contains("Scroll") ||
                                 enVal.Contains("Mask") || enVal.Contains("Crown") || enVal.Contains("Boots") || enVal.Contains("Shield") ||
                                 enVal.Contains("Gem") || enVal.Contains("Crystal") || enVal.Contains("Shard") || enVal.Contains("Essence") ||
                                 enVal.Contains("Skill") || enVal.Contains("Talent") || enVal.Contains("Node") || enVal.Contains("Unlock") ||
                                 enVal.Contains("Upgrade") || enVal.Contains("Level") || enVal.Contains("Attribute"))
                        {
                            listTalentsAndEquipment.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 3. Подсказки, промпты, системные уведомления, действия квестов
                        else if (enVal.Contains("Please") || enVal.Contains("Cannot") || enVal.Contains("Reached") || enVal.Contains("Limit") ||
                                 enVal.Contains("Success") || enVal.Contains("Failed") || enVal.Contains("Available") || enVal.Contains("Start") ||
                                 enVal.Contains("Finish") || enVal.Contains("Confirm") || enVal.Contains("Cancel") || enVal.Contains("Select") ||
                                 enVal.Contains("Current") || enVal.Contains("Total") || enVal.Contains("Cost") || enVal.Contains("Price") ||
                                 enVal.Contains("Duel") || enVal.Contains("Battle") || enVal.Contains("Trigger") || enVal.Contains("Location"))
                        {
                            listPromptsAndActions.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 4. Чистый читаемый UI, названия локаций, NPC и объектов
                        else if (enVal.Length >= 3 && enVal.Length <= 60 && !enVal.Contains("_") && !enVal.Contains("/") &&
                                 Regex.IsMatch(enVal, @"^[A-Za-z0-9\s,\.\-'\?!%:;]+$") &&
                                 (enVal.Contains(" ") || char.IsUpper(enVal[0])))
                        {
                            listCleanUI.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine("\n=== Candidates Count by Priority ===");
        Console.WriteLine("1. Story, Dialogues & TRPG Narrative: " + listStoryAndDialogues.Count);
        Console.WriteLine("2. Talents, Items, Equipment & Spirits: " + listTalentsAndEquipment.Count);
        Console.WriteLine("3. Prompts, Actions, Quest Steps & Systems: " + listPromptsAndActions.Count);
        Console.WriteLine("4. Clean UI, Locations & Names: " + listCleanUI.Count);

        Console.WriteLine("\n=== Samples of Tier 1 (Story & Dialogues, First 10) ===");
        for (int i = 0; i < Math.Min(10, listStoryAndDialogues.Count); i++)
            Console.WriteLine(string.Format("  [{0}] EN: {1} | CN: {2}", i, listStoryAndDialogues[i].Item2, listStoryAndDialogues[i].Item1));

        Console.WriteLine("\n=== Samples of Tier 2 (Talents & Equipment, First 10) ===");
        for (int i = 0; i < Math.Min(10, listTalentsAndEquipment.Count); i++)
            Console.WriteLine(string.Format("  [{0}] EN: {1} | CN: {2}", i, listTalentsAndEquipment[i].Item2, listTalentsAndEquipment[i].Item1));

        Console.WriteLine("\n=== Samples of Tier 3 (Prompts & Actions, First 10) ===");
        for (int i = 0; i < Math.Min(10, listPromptsAndActions.Count); i++)
            Console.WriteLine(string.Format("  [{0}] EN: {1} | CN: {2}", i, listPromptsAndActions[i].Item2, listPromptsAndActions[i].Item1));

        Console.WriteLine("\n=== Samples of Tier 4 (Clean UI & Locations, First 10) ===");
        for (int i = 0; i < Math.Min(10, listCleanUI.Count); i++)
            Console.WriteLine(string.Format("  [{0}] EN: {1} | CN: {2}", i, listCleanUI[i].Item2, listCleanUI[i].Item1));
    }
}
