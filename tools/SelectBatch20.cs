using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class SelectBatch20
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

    static bool IsTechnicalCodeOrDebug(string enVal, string cnKey)
    {
        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
            return true;

        if (enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
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
            enVal.StartsWith("[UIFrame") || enVal.StartsWith("HUD_") || enVal.Contains("GuildSystem:") ||
            enVal.Contains("PreviewAppearanceOnDummy") || enVal.Contains("missing appearance data") ||
            enVal.Contains("ByOwner:") || enVal.Contains("ByUnit:") || enVal.Contains("ByPos:") ||
            enVal.Contains("errorCode=") ||
            Regex.IsMatch(enVal, @"^\d+:\s*(Disable|Enable|Hide|Start|End|Override)") ||
            Regex.IsMatch(enVal, @"^\d+-(Disable|Enable|Correct)"))
            return true;

        return false;
    }

    static int GetStringPriorityScore(string enVal, string cnKey)
    {
        int score = 0;

        // 1. Диалоги и нарратив персонажей, мысли, цитаты
        if (enVal.Contains("?") || enVal.Contains("!") || enVal.Contains("...") || enVal.Contains("\"") || enVal.Contains("“") ||
            Regex.IsMatch(enVal, @"\b(I|you|he|she|we|they|my|your|his|her|our|their|me|him|us|them)\b", RegexOptions.IgnoreCase) ||
            Regex.IsMatch(enVal, @"\b(am|is|are|was|were|will|would|can|could|should|must|don't|can't|won't|didn't|haven't|isn't)\b", RegexOptions.IgnoreCase) ||
            enVal.Contains("{{"))
        {
            score += 100;
        }

        // 2. Действия квестов, задачи, геймплей
        if (Regex.IsMatch(enVal, @"\b(Click|Talk|Go to|Find|Defeat|Obtain|Collect|Use|Enter|Leave|Open|Close|Select|Investigate|Inspect|Arrive|Explore|Follow|Escort|Protect|Interact)\b", RegexOptions.IgnoreCase))
        {
            score += 60;
        }

        // 3. Предметы, экипировка, рецепты, зелья, способности, характеристики
        if (Regex.IsMatch(enVal, @"\b(Potion|Formula|Recipe|Weapon|Blade|Sword|Armor|Robe|Ring|Pendant|Boots|Crown|Mask|Badge|Scroll|Crystal|Essence|Gem|Skill|Talent|Passive|Attack|Defense|HP|Damage|Critical|Speed|Cooldown)\b", RegexOptions.IgnoreCase))
        {
            score += 50;
        }

        // 4. Локации, фракции, боссы, имена мира
        if (Regex.IsMatch(enVal, @"\b(Tingen|Backlund|Bieber|Sharon|House|Factory|Dock|Bar|Street|Square|Cathedral|Church|Order|School of Thought|Battlefield|Dungeon|Ruins|Camp|Forest|Tower|Gate)\b", RegexOptions.IgnoreCase))
        {
            score += 40;
        }

        // 5. Игровой UI, меню, гильдии, матчи, социал
        if (Regex.IsMatch(enVal, @"\b(Club|Guild|Mail|Chat|Friend|Team|Party|Rank|Score|Round|Match|Server|Online|Offline|Confirm|Cancel|Delete|Save|Load|Settings|Option|Display|Audio|Music|Sound|Video|Graphics|Resolution|Reward|Drop|Level|Price|Cost|Limit)\b", RegexOptions.IgnoreCase))
        {
            score += 30;
        }

        // Бонус за чистый читаемый текст нормальной длины (не слишком короткий, не машинный код)
        if (enVal.Length >= 8 && enVal.Length <= 120 && !enVal.Contains("_"))
        {
            score += 15;
        }

        // Штраф за технические префиксы с подчеркиванием типа "00_Test"
        if (enVal.StartsWith("00_") || enVal.StartsWith("01_") || enVal.Contains("Test Scene"))
        {
            score -= 50;
        }

        // Штраф за слишком короткие строки (< 3 символов)
        if (enVal.Length < 3)
        {
            score -= 40;
        }

        return score;
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

        var list = new List<Tuple<int, string, string>>();
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

                        if (existingRu.Contains(cnKey) || existingRu.Contains(enVal))
                            continue;

                        if (seen.Contains(cnKey) || seen.Contains(enVal))
                            continue;
                        seen.Add(cnKey);
                        seen.Add(enVal);

                        if (IsTechnicalCodeOrDebug(enVal, cnKey))
                            continue;

                        int score = GetStringPriorityScore(enVal, cnKey);
                        list.Add(Tuple.Create(score, cnKey, enVal));
                    }
                }
            }
        }

        // Сортировка по убыванию приоритета
        list.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        Console.WriteLine(string.Format("Total scored candidates: {0}", list.Count));
        Console.WriteLine("\nTop 20 candidates for Batch 20:");
        for (int i = 0; i < 20; i++)
        {
            Console.WriteLine(string.Format("  [{0}] (Score {1}) EN: {2} | CN: {3}", i, list[i].Item1, list[i].Item3, list[i].Item2));
        }

        Console.WriteLine("\nCandidate #4000:");
        Console.WriteLine(string.Format("  [4000] (Score {0}) EN: {1} | CN: {2}", list[4000].Item1, list[4000].Item3, list[4000].Item2));

        Console.WriteLine("\nCandidate #8499:");
        Console.WriteLine(string.Format("  [8499] (Score {0}) EN: {1} | CN: {2}", list[8499].Item1, list[8499].Item3, list[8499].Item2));
    }
}
