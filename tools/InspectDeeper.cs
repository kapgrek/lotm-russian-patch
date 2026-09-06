using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectDeeper
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

        var realLoreAndDialogue = new List<Tuple<string, string>>();
        var realUIAndNotifications = new List<Tuple<string, string>>();
        var combatAndSkills = new List<Tuple<string, string>>();

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

                        // Исключаем технический мусор
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") ||
                            enVal.StartsWith(".p4config") || enVal.Contains("-----") || enVal.Contains(".cpp") || enVal.Contains(".h") ||
                            enVal.StartsWith("523") || enVal.StartsWith("520") || enVal.StartsWith("GTA-") || enVal.StartsWith("GTA ") ||
                            enVal.StartsWith("Camera_") || enVal.StartsWith("Mat_") || enVal.StartsWith("SK_") || enVal.StartsWith("SM_") ||
                            Regex.IsMatch(enVal, @"^\d+:\s*(Disable|Enable|Hide)"))
                            continue;

                        // Проверяем боевые описания
                        if (enVal.Contains("HyperLink") || enVal.Contains("damage") || enVal.Contains("Damage") || enVal.Contains("Cooldown") ||
                            enVal.Contains("cooldown") || enVal.Contains("skill") || enVal.Contains("Skill") || enVal.Contains("Bleed") ||
                            enVal.Contains("Stun") || enVal.Contains("Crit") || enVal.Contains("Dusk") || enVal.Contains("Slash"))
                        {
                            combatAndSkills.Add(Tuple.Create(cnKey, enVal));
                        }
                        // Проверяем диалоги, сюжет, записки, лор
                        else if (enVal.Length > 25 && (enVal.Contains("\"") || enVal.Contains("!") || enVal.Contains("?") || enVal.Contains("...") ||
                                 enVal.Contains("—") || enVal.Contains("Goddess") || enVal.Contains("Lord") || enVal.Contains("Church") ||
                                 enVal.Contains("Tingen") || enVal.Contains("Backlund") || enVal.Contains("Beyonder") || enVal.Contains("Potion") ||
                                 enVal.Contains("Artifact") || enVal.Contains("Mr.") || enVal.Contains("Miss") || enVal.Contains("you") ||
                                 enVal.Contains("You") || enVal.Contains("I ") || enVal.Contains("We ") || enVal.Contains("They ")))
                        {
                            realLoreAndDialogue.Add(Tuple.Create(cnKey, enVal));
                        }
                        // Интерфейс, подсказки, уведомления
                        else if (enVal.Length >= 4 && !enVal.Contains("_") && Regex.IsMatch(enVal, @"^[A-Za-z0-9\s,\.\-'\?!%]+$"))
                        {
                            realUIAndNotifications.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Combat & Skills: {0}", combatAndSkills.Count));
        Console.WriteLine(string.Format("Real Lore & Dialogue: {0}", realLoreAndDialogue.Count));
        Console.WriteLine(string.Format("Real UI & Notifications: {0}", realUIAndNotifications.Count));

        Console.WriteLine("\n--- Sample Real Lore & Dialogue (15) ---");
        for (int i = 0; i < Math.Min(15, realLoreAndDialogue.Count); i++)
            Console.WriteLine(string.Format("[{0}] EN: {1}\n     CN: {2}", i, realLoreAndDialogue[i].Item2, realLoreAndDialogue[i].Item1));

        Console.WriteLine("\n--- Sample Combat & Skills (10) ---");
        for (int i = 0; i < Math.Min(10, combatAndSkills.Count); i++)
            Console.WriteLine(string.Format("[{0}] EN: {1}\n     CN: {2}", i, combatAndSkills[i].Item2, combatAndSkills[i].Item1));

        Console.WriteLine("\n--- Sample Real UI (10) ---");
        for (int i = 0; i < Math.Min(10, realUIAndNotifications.Count); i++)
            Console.WriteLine(string.Format("[{0}] EN: {1}\n     CN: {2}", i, realUIAndNotifications[i].Item2, realUIAndNotifications[i].Item1));
    }
}
