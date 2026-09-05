using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LotmTranslator
{
    class Program
    {
        static string GeminiPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextGemini.lua";
        static string RussianPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";
        static string RepoRuPath = @"C:\Users\yapug\.gemini\antigravity\scratch\lotm-russian-patch\RuntimeTextRussian.lua";
        static string RepoDataRuPath = @"C:\Users\yapug\.gemini\antigravity\scratch\lotm-russian-patch\data\RuntimeTextRussian.lua";

        static readonly Dictionary<string, string> CanonMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "The Fool", "Шут" },
            { "Lord of Mysteries", "Повелитель Тайн" },
            { "Lord of the Mysteries", "Повелитель Тайн" },
            { "Seer", "Провидец" },
            { "Clown", "Клоун" },
            { "Magician", "Фокусник" },
            { "Faceless", "Безликий" },
            { "Marionettist", "Марионеточник" },
            { "Bizarro Sorcerer", "Маг Непостижимого" },
            { "Scholar of Yore", "Учёный Прошлого" },
            { "Miracle Invoker", "Творец Чудес" },
            { "Attendant of Mysteries", "Служитель Тайн" },
            { "Beyonder", "Потусторонний" },
            { "Beyonders", "Потусторонние" },
            { "Beyonder Material", "Потусторонний материал" },
            { "Beyonder Materials", "Потусторонние материалы" },
            { "Beyonder Characteristic", "Потустороннее свойство" },
            { "Beyonder Characteristics", "Потусторонние свойства" },
            { "Beyonder Resonance", "Потусторонний резонанс" },
            { "Spirit Body Threads", "Нити духовного тела" },
            { "Spirit Body Thread", "Нить духовного тела" },
            { "Spirit Body", "Духовное тело" },
            { "Astral Projection", "Астральная проекция" },
            { "Historical Projection", "Историческая проекция" },
            { "Historical Projections", "Исторические проекции" },
            { "Historical Echoes", "Исторические отголоски" },
            { "Historical Echo", "Исторический отголосок" },
            { "Historical Crystals", "Исторические кристаллы" },
            { "Historical Crystal", "Исторический кристалл" },
            { "Sealed Artifact", "Запечатанный артефакт" },
            { "Sealed Artifacts", "Запечатанные артефакты" },
            { "Air Bullet", "Воздушная пуля" },
            { "Air Bullets", "Воздушные пули" },
            { "Paper Figurine Substitutes", "Замена бумажным человечком" },
            { "Paper Figurine Substitute", "Замена бумажным человечком" },
            { "Paper Figurine", "Бумажный человечек" },
            { "Paper Figurines", "Бумажные человечки" },
            { "Flame Controlling", "Управление пламенем" },
            { "Tarot Club", "Клуб Таро" },
            { "Nighthawks", "Ночные Ястребы" },
            { "Tingen", "Тинген" },
            { "Backlund", "Бэкланд" },
            { "Damage Reduction", "Снижение урона" },
            { "Physical Damage", "Физический урон" },
            { "Magic Damage", "Магический урон" },
            { "True Damage", "Чистый урон" },
            { "Fixed Damage", "Фиксированный урон" },
            { "Life Steal", "Вампиризм" },
            { "Lifesteal", "Вампиризм" },
            { "Shield Boost", "Усиление щита" },
            { "Super Armor", "Суперброня" },
            { "Cleanse Skill", "Снятие контроля" },
            { "Cleanse", "Снятие контроля" },
            { "Crowd-Control Break", "Снятие контроля" },
            { "Crowd-Control", "Контроль" },
            { "Cooldown Reduction", "Сокращение перезарядки" },
            { "Cooldown", "Перезарядка" },
            { "Basic Attack", "Базовая атака" },
            { "Combat Skill", "Боевой навык" },
            { "Special Skill", "Особый навык" },
            { "Acting Skill", "Навык лицедейства" },
            { "Skill Enhancement", "Усиление навыков" },
            { "Skill Block", "Блок навыков" },
            { "Critical Strike", "Критический удар" },
            { "Critical Rate", "Шанс крит. удара" },
            { "Crit Rate", "Шанс крита" },
            { "Crit Damage", "Крит. урон" },
            { "Spirituality", "Духовность" },
            { "Sanity", "Рассудок" },
            { "Madness", "Безумие" },
            { "Corruption", "Искажение" },
            { "Sequence", "Последовательность" },
            { "Spectator", "Зритель" },
            { "Telepathist", "Телепат" },
            { "Psychiatrist", "Психиатр" },
            { "Mystery Pryer", "Подглядывающий за Тайнами" },
            { "Sleepless", "Бессонный" },
            { "Midnight Poet", "Полуночный Поэт" },
            { "Nightmare", "Кошмар" },
            { "Hunter", "Охотник" },
            { "Pyromaniac", "Пироман" },
            { "Reaper", "Жнец" },
            { "Marionette", "Марионетка" },
            { "Marionettes", "Марионетки" },
            { "Puppet", "Марионетка" },
            { "Puppets", "Марионетки" },
            { "Divination", "Гадание" },
            { "Talisman", "Амулет" },
            { "Talismans", "Амулеты" },
            { "Physical DEF", "Физ. защита" },
            { "Magic DEF", "Маг. защита" },
            { "Physical ATK", "Физ. атака" },
            { "Magic ATK", "Маг. атака" },
            { "Attack Speed", "Скорость атаки" },
            { "Movement Speed", "Скорость бега" },
            { "Move Speed", "Скорость бега" }
        };

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            int count = 500;
            string mode = "skills"; // "skills", "ui", "all"

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--count" && i + 1 < args.Length) int.TryParse(args[++i], out count);
                if (args[i] == "--mode" && i + 1 < args.Length) mode = args[++i].ToLowerInvariant();
            }

            Console.WriteLine("=====================================================");
            Console.WriteLine("  Lord of the Mysteries - High-Speed C# Translator   ");
            Console.WriteLine("=====================================================");
            Console.WriteLine("Режим: " + mode + " | Лимит строк: " + count);

            // 1. Загрузка существующих переводов
            var existingRu = new Dictionary<string, string>(StringComparer.Ordinal);
            if (File.Exists(RussianPath))
            {
                foreach (var line in File.ReadAllLines(RussianPath, Encoding.UTF8))
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
                            if (valEnd >= valStart)
                            {
                                string v = t.Substring(valStart, valEnd - valStart);
                                existingRu[k] = v;
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Текущий размер словаря: " + existingRu.Count + " записей");

            // 2. Отбор кандидатов
            var candidates = new List<Tuple<string, string>>();
            using (var reader = new StreamReader(GeminiPath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null && candidates.Count < count)
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
                            if (valEnd >= valStart)
                            {
                                string enVal = t.Substring(valStart, valEnd - valStart);
                                if (!existingRu.ContainsKey(cnKey) && !existingRu.ContainsKey(enVal) && enVal.Length > 1)
                                {
                                    bool accept = false;
                                    if (mode == "skills")
                                    {
                                        accept = IsSkillText(enVal, cnKey);
                                    }
                                    else if (mode == "ui")
                                    {
                                        accept = IsUiText(enVal, cnKey);
                                    }
                                    else // "all"
                                    {
                                        accept = true;
                                    }

                                    if (accept)
                                    {
                                        candidates.Add(Tuple.Create(cnKey, enVal));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Отобрано для перевода: " + candidates.Count + " записей");
            if (candidates.Count == 0)
            {
                Console.WriteLine("Нет новых строк для перевода в выбранном режиме!");
                return;
            }

            // 3. Параллельный перевод с ограничением параллелизма (6 потоков)
            var translatedResults = new ConcurrentDictionary<string, string>();
            int processed = 0;

            Parallel.ForEach(candidates, new ParallelOptions { MaxDegreeOfParallelism = 6 }, item =>
            {
                string cn = item.Item1;
                string en = item.Item2;

                string ru = TranslateSingle(en);
                if (!string.IsNullOrWhiteSpace(ru))
                {
                    // Применяем каноничные термины
                    foreach (var pair in CanonMap)
                    {
                        ru = Regex.Replace(ru, Regex.Escape(pair.Key), pair.Value, RegexOptions.IgnoreCase);
                    }

                    translatedResults[cn] = ru;
                    translatedResults[en] = ru;
                }

                int p = Interlocked.Increment(ref processed);
                if (p % 10 == 0 || p == candidates.Count)
                {
                    Console.Write("\rПрогресс перевода: " + p + " / " + candidates.Count);
                }
                Thread.Sleep(80);
            });

            Console.WriteLine("\nПеревод завершен. Успешно получено: " + translatedResults.Count + " записей.");

            // 4. Слияние со словарем
            foreach (var kvp in translatedResults)
            {
                existingRu[kvp.Key] = kvp.Value;
            }

            // 5. Запись и валидация
            SaveAndValidate(RussianPath, existingRu);

            // 6. Синхронизация с репозиторием
            if (File.Exists(RepoRuPath)) File.Copy(RussianPath, RepoRuPath, true);
            if (File.Exists(RepoDataRuPath)) File.Copy(RussianPath, RepoDataRuPath, true);
            Console.WriteLine("Словарь успешно синхронизирован с репозиторием!");
        }

        static bool IsSkillText(string en, string cn)
        {
            if (Regex.IsMatch(en, @"(Cooldown|Damage|Spell|Health|Physical|Magic|Energy|Cast|Charge|Passive|Active Skill|Combat Skill|Deals \*|reduces cooldown|trigger|Crowd-Control|Stun|Bleed|Poison|Shield|Heal|Super Armor|Skill Enhancement|Combat skill|Invincibility|Skill Block|Skill Name|Talent|Combo|Ultimate|Buff|Debuff|Crit|Parry|Dodge|Spirituality|Sanity|Corruption|Resist|Penetration|Vampiric|Lifesteal|Bleeding|Ignite|Freeze|Shock|Silence|Disarm|Knockdown|Airborne|Stagger|Break|Sealed Artifact|Beyonder|Air Bullet|Paper Figurine|Flame Controlling|Spirit Body)", RegexOptions.IgnoreCase))
                return true;
            if (Regex.IsMatch(cn, @"(技能|冷却|伤害|被动|主动|攻击|防御|治疗|护盾|霸体|解控|法术|绝技|天赋|连击|终结|奥义|连招|增益|减益|暴击|闪避|格挡|招架|灵性|理智|疯狂|侵蚀|序列|秘偶|占卜|塔罗|符咒|魔药|仪式|领域|光环|怒气|抗性|穿透|吸血|流血|点燃|冰冻|感电|沉默|缴械|击倒|击飞|硬直|破防|破甲|破霸|封印物|非凡|空气子弹|纸人替身|操纵火焰|灵体)"))
                return true;
            return false;
        }

        static bool IsUiText(string en, string cn)
        {
            if (Regex.IsMatch(en, @"(Button|Menu|Setting|Settings|Interface|Window|Confirm|Cancel|Back|Next|Previous|Exit|Quit|Option|Options|Level|Quest|Task|Reward|Inventory|Bag|Item|Items|Equip|Equipment|Shop|Store|Mail|Message|Chat|Friend|Friends|Guild|Club|Rank|Ranking|Audio|Video|Graphic|Display|Resolution|Key|Binding|Click|Press|Select|Choose|Filter|Sort|Search|Tab|Tips|Notice|Announcement|System|Tutorial|Guide|Help)", RegexOptions.IgnoreCase))
                return true;
            if (Regex.IsMatch(cn, @"(按钮|菜单|设置|界面|窗口|确定|取消|返回|下一步|上一步|退出|选项|等级|任务|奖励|背包|道具|物品|装备|商店|商城|邮件|消息|聊天|好友|公会|俱乐部|排行|音频|画面|图像|显示|分辨率|按键|快捷键|点击|按|选择|筛选|排序|搜索|标签|提示|公告|系统|教程|指引|帮助)"))
                return true;
            return false;
        }

        static string TranslateSingle(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            // Защита тегов Slate, токенов формата и {SendAnswer:...}
            var tagMap = new Dictionary<string, string>();
            int tagIdx = 0;
            string protectedText = Regex.Replace(text, @"<[^>]+>|\{[^{}]+\}|\*d\*\*|\*d|\*f\*\*|\*f|mul\([^)]+\)|spellfielddisc\([^)]+\)|CheckStar\([^)]+\)", m =>
            {
                string ph = "XTAG" + (tagIdx++) + "X";
                tagMap[ph] = m.Value;
                return ph;
            });

            try
            {
                string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=ru&dt=t&q=" + Uri.EscapeDataString(protectedText);
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    byte[] data = wc.DownloadData(url);
                    string json = Encoding.UTF8.GetString(data);

                    // Парсинг ответа Google Translate
                    int endFirstBlock = json.IndexOf("],null,");
                    if (endFirstBlock < 0) endFirstBlock = json.IndexOf("]],");
                    string sentencesPart = endFirstBlock > 0 ? json.Substring(0, endFirstBlock) : json;

                    var sb = new StringBuilder();
                    var match = Regex.Matches(sentencesPart, @"\[\""((\\""|[^\""])+)\""\s*,\s*\""");
                    foreach (Match m in match)
                    {
                        if (m.Groups.Count > 1)
                        {
                            string segment = Regex.Unescape(m.Groups[1].Value);
                            sb.Append(segment);
                        }
                    }

                    string result = sb.ToString();
                    result = Regex.Replace(result, @"[a-f0-9]{32}\b", "");
                    if (string.IsNullOrWhiteSpace(result)) result = text;

                    // Восстановление тегов
                    foreach (var kvp in tagMap)
                    {
                        result = result.Replace(kvp.Key, kvp.Value);
                    }

                    return result.Trim();
                }
            }
            catch
            {
                return text;
            }
        }

        static string CleanLua(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            string r = Regex.Replace(s, @"(\\+)\""", "\"");
            r = r.Replace("\"", "\\\"");
            return r;
        }

        static void SaveAndValidate(string path, Dictionary<string, string> dict)
        {
            using (var sw = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                sw.WriteLine("-- Generated Russian Translation Dictionary for Lord of Mysteries");
                sw.WriteLine("-- Entries: " + dict.Count);
                sw.WriteLine("return {");
                foreach (var kvp in dict)
                {
                    string k = CleanLua(kvp.Key);
                    string cleanVal = Regex.Replace(kvp.Value, @"[a-f0-9]{32}\b", "").Trim();
                    string v = CleanLua(cleanVal);
                    sw.WriteLine("    [\"" + k + "\"] = \"" + v + "\",");
                }
                sw.WriteLine("}");
            }

            Console.WriteLine("Словарь записан: " + path + " (" + dict.Count + " записей)");
        }
    }
}
