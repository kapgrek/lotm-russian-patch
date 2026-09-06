using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class AnalyzeBatch17Selection
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

        var listStoryAndLore = new List<Tuple<string, string>>();
        var listCombatAndSkills = new List<Tuple<string, string>>();
        var listUINoticesAndPrompts = new List<Tuple<string, string>>();
        var listGameplayAndWorld = new List<Tuple<string, string>>();
        var listOtherUI = new List<Tuple<string, string>>();

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

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        // Исключаем системный мусор
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") ||
                            enVal.StartsWith(".p4config") || enVal.Contains("-----") || enVal.Contains(".cpp") || enVal.Contains(".h") ||
                            enVal.StartsWith("523") || enVal.StartsWith("520") || enVal.StartsWith("GTA-") || enVal.StartsWith("GTA ") ||
                            enVal.StartsWith("Camera_") || enVal.StartsWith("Mat_") || enVal.StartsWith("SK_") || enVal.StartsWith("SM_") ||
                            enVal.StartsWith("SkillID: ") || enVal.Contains("InterruptMode") || enVal.Contains("BindSkillID") ||
                            enVal.StartsWith("AI ") || enVal.Contains("AI ") ||
                            Regex.IsMatch(enVal, @"^\d+:\s*(Disable|Enable|Hide)"))
                            continue;

                        // 1. Сюжет, диалоги, дневники, мысли, персонажи
                        if (enVal.Contains("<h>") || enVal.Contains("</>") || enVal.Contains("May ") || enVal.Contains("June ") ||
                            enVal.Contains("July ") || enVal.Contains("August ") || enVal.Contains("September ") || enVal.Contains("October ") ||
                            enVal.Contains("November ") || enVal.Contains("December ") || enVal.Contains("Neil") || enVal.Contains("Daly") ||
                            enVal.Contains("Edwin") || enVal.Contains("Goddess") || enVal.Contains("Church") || enVal.Contains("Beyonder") ||
                            enVal.Contains("Moretti") || enVal.Contains("Mr.") || enVal.Contains("Miss") ||
                            (enVal.Length > 30 && (enVal.Contains("\"") || enVal.Contains("...") || enVal.Contains("?") || enVal.Contains("!"))))
                        {
                            listStoryAndLore.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 2. Боевые навыки, статусы, подсказки боя
                        else if (enVal.Contains("HyperLink") || enVal.Contains("damage") || enVal.Contains("Damage") ||
                                 enVal.Contains("Cooldown") || enVal.Contains("cooldown") || enVal.Contains("Slash") ||
                                 enVal.Contains("Dusk") || enVal.Contains("Bleed") || enVal.Contains("Crit") ||
                                 enVal.Contains("AOE") || enVal.Contains("Mutated") || enVal.Contains("mutated") ||
                                 cnKey.Contains("技能") || cnKey.Contains("伤害") || cnKey.Contains("冷却"))
                        {
                            listCombatAndSkills.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 3. Системные уведомления, диалоговые окна, предупреждения
                        else if (enVal.Contains("Warning") || enVal.Contains("Confirm") || enVal.Contains("Cancel") ||
                                 enVal.Contains("Notice") || enVal.Contains("Prompt") || enVal.Contains("Tips") ||
                                 enVal.Contains("Unlock") || enVal.Contains("Success") || enVal.Contains("Failed") ||
                                 enVal.Contains("Discount") || enVal.Contains("Voucher") || enVal.Contains("Pound") ||
                                 enVal.Contains("Sole") || enVal.Contains("Battlefield") || enVal.Contains("Queue") ||
                                 enVal.Contains("Level") || enVal.Contains("Reward"))
                        {
                            listUINoticesAndPrompts.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 4. Геймплей мира, действия, квестовые шаги
                        else if (enVal.Contains("Pretend") || enVal.Contains("Teleport") || enVal.Contains("Move") ||
                                 enVal.Contains("Arrive") || enVal.Contains("Find") || enVal.Contains("Search") ||
                                 enVal.Contains("Talk") || enVal.Contains("Interact") || enVal.Contains("Inspect") ||
                                 enVal.Contains("Open") || enVal.Contains("Enter") || enVal.Contains("Leave"))
                        {
                            listGameplayAndWorld.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 5. Другой читаемый UI
                        else if (enVal.Length >= 5 && enVal.Length <= 80 && !enVal.Contains("_") &&
                                 Regex.IsMatch(enVal, @"^[A-Za-z0-9\s,\.\-'\?!%]+$") &&
                                 (enVal.Contains(" ") || char.IsUpper(enVal[0])))
                        {
                            listOtherUI.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("1. Story, Dialogue & Lore: {0}", listStoryAndLore.Count));
        Console.WriteLine(string.Format("2. Combat & Skills: {0}", listCombatAndSkills.Count));
        Console.WriteLine(string.Format("3. UI Notices & Prompts: {0}", listUINoticesAndPrompts.Count));
        Console.WriteLine(string.Format("4. Gameplay & World Steps: {0}", listGameplayAndWorld.Count));
        Console.WriteLine(string.Format("5. Other UI & Menus: {0}", listOtherUI.Count));

        int totalSelected = listStoryAndLore.Count + listCombatAndSkills.Count + listUINoticesAndPrompts.Count + listGameplayAndWorld.Count + listOtherUI.Count;
        Console.WriteLine(string.Format("\nTotal High-Quality Candidates: {0}", totalSelected));
    }
}
