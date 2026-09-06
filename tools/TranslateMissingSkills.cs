using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslateMissingSkills
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string DataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
    static string GameRuPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";

    static readonly Dictionary<string, string> CanonMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "The Fool", "Шут" },
        { "Lord of Mysteries", "Повелитель Тайн" },
        { "Lord of the Mysteries", "Повелитель Тайн" },
        { "Beyonder", "Потусторонний" },
        { "Beyonders", "Потусторонние" },
        { "Beyonder Characteristic", "Потустороннее свойство" },
        { "Beyonder Characteristics", "Потусторонние свойства" },
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "Spirit Body Threads", "Нити духовного тела" },
        { "Spirit Body Thread", "Нить духовного тела" },
        { "Marionette", "Марионетка" },
        { "Marionettes", "Марионетки" },
        { "Historical Projection", "Историческая проекция" },
        { "Historical Projections", "Исторические проекции" },
        { "Paper Figurine Substitute", "Замена бумажным человечком" },
        { "Paper Figurine Substitutes", "Замена бумажным человечком" },
        { "Paper Figurine", "Бумажный человечек" },
        { "Flame Controlling", "Управление пламенем" },
        { "Tarot Club", "Клуб Таро" },
        { "Nighthawks", "Ночные Ястребы" },
        { "Tingen", "Тинген" },
        { "Backlund", "Бэкланд" },
        { "Seer", "Провидец" },
        { "Clown", "Клоун" },
        { "Magician", "Фокусник" },
        { "Faceless", "Безликий" },
        { "Marionettist", "Марионеточник" },
        { "Bizarro Sorcerer", "Маг Непостижимого" },
        { "Scholar of Yore", "Учёный Прошлого" },
        { "Miracle Invoker", "Творец Чудес" },
        { "Attendant of Mysteries", "Служитель Тайн" },
        { "Spectator", "Зритель" },
        { "Telepathist", "Телепат" },
        { "Psychiatrist", "Психиатр" },
        { "Sleepless", "Бессонный" },
        { "Midnight Poet", "Полуночный Поэт" },
        { "Nightmare", "Кошмар" },
        { "Hunter", "Охотник" },
        { "Pyromaniac", "Пироман" },
        { "Reaper", "Жнец" },
        { "Damage Reduction", "Снижение урона" },
        { "Super Armor", "Суперброня" },
        { "Cleanse", "Снятие контроля" },
        { "Cooldown Reduction", "Сокращение перезарядки" },
        { "Cooldown", "Перезарядка" },
        { "Mind Fire", "Пламя разума" },
        { "Induction Mark", "Метка внушения" },
        { "Hypnosis", "Гипноз" },
        { "Imprisonment", "Заточение" },
        { "Imprison", "Заточить" },
        { "Healing Reduction", "Снижение лечения" },
        { "Nightmare Stance", "Стойка кошмара" },
        { "Imagination Stance", "Стойка фантазии" },
        { "Psychotherapy", "Психотерапия" },
        { "Consciousness Shock", "Удар сознания" },
        { "Mental Plague", "Ментальная чума" },
        { "Dream Weaving", "Плетение снов" },
        { "Dream Analysis", "Анализ снов" },
        { "Mind Control", "Контроль сознания" },
        { "Mental Suggestion", "Ментальное внушение" },
        { "Psychological Suggestion", "Ментальное внушение" },
        { "Dream Return", "Возврат сна" },
        { "Rebirth", "Перерождение" },
        { "Intimidation", "Устрашение" },
        { "Invisibility", "Незримость" },
        { "Insight", "Прозрение" },
        { "Frenzy", "Бешенство" },
        { "Physical Damage", "Физ. урон" },
        { "Magic Damage", "Маг. урон" },
        { "True Damage", "Чистый урон" },
        { "Physical ATK", "Физ. атака" },
        { "Magic ATK", "Маг. атака" },
        { "Physical DEF", "Физ. защита" },
        { "Magic DEF", "Маг. защита" },
        { "Critical Strike", "Критический удар" },
        { "Crit Rate", "Шанс крита" },
        { "Crit Damage", "Крит. урон" },
        { "Vulnerability", "Уязвимость" },
        { "Stagnation", "Тягучесть" },
        { "Knockdown", "Сбивание с ног" },
        { "Knocking Down", "Сбивание с ног" },
        { "Airborne", "Подбрасывание" },
        { "Single Target", "Одиночная цель" },
        { "Basic Attack", "Базовая атака" },
        { "Combat Skill", "Боевой навык" },
        { "Special Skill", "Особый навык" }
    };

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("==========================================================");
        Console.WriteLine("   LOTM - Полный перевод всех недостающих навыков игры   ");
        Console.WriteLine("==========================================================");

        // 1. Загрузка существующего словаря RuntimeTextRussian.lua
        var existingRu = new Dictionary<string, string>(StringComparer.Ordinal);
        var orderedKeys = new List<string>();

        if (File.Exists(RuPath))
        {
            string content = File.ReadAllText(RuPath, Encoding.UTF8);
            int pos = 0;
            int len = content.Length;
            while (pos < len)
            {
                int keyStart = content.IndexOf("[\"", pos);
                if (keyStart == -1) break;
                keyStart += 2;

                StringBuilder sbKey = new StringBuilder();
                bool escaped = false;
                int keyEnd = -1;
                for (int i = keyStart; i < len - 1; i++)
                {
                    char c = content[i];
                    if (escaped) { sbKey.Append(c); escaped = false; }
                    else if (c == '\\') { sbKey.Append(c); escaped = true; }
                    else if (c == '"' && content[i + 1] == ']') { keyEnd = i; break; }
                    else { sbKey.Append(c); }
                }
                if (keyEnd == -1) break;

                int valAssign = content.IndexOf("=", keyEnd + 2);
                if (valAssign == -1) break;
                int valQuoteStart = content.IndexOf("\"", valAssign + 1);
                if (valQuoteStart == -1) break;
                valQuoteStart += 1;

                StringBuilder sbVal = new StringBuilder();
                escaped = false;
                int valEnd = -1;
                for (int i = valQuoteStart; i < len; i++)
                {
                    char c = content[i];
                    if (escaped) { sbVal.Append(c); escaped = false; }
                    else if (c == '\\') { sbVal.Append(c); escaped = true; }
                    else if (c == '"') { valEnd = i; break; }
                    else { sbVal.Append(c); }
                }
                if (valEnd == -1) break;

                string k = sbKey.ToString();
                string v = sbVal.ToString();
                if (!existingRu.ContainsKey(k))
                {
                    orderedKeys.Add(k);
                }
                existingRu[k] = v;
                pos = valEnd + 1;
            }
        }
        Console.WriteLine("Загружено существующих ключей: " + existingRu.Count);

        // 2. Сканирование ВСЕХ 126 621 строк RuntimeTextGemini.lua
        var formulaRegex = new Regex(@"(\*d|\*f|spellfielddisc|bulletdisc|buffdisc|mul\(|CheckStar\()", RegexOptions.IgnoreCase);
        var stanceRegex = new Regex(@"Stance.*switches to|switches to.*Stance|Only available in.*Stance|仅.*姿态可用", RegexOptions.IgnoreCase);
        var skillNodeRegex = new Regex(@"u=\""\d{8}\""|HyperLink.*(心灵之火|心理治疗|梦境|狂乱|洞察|震慑|衰败|精神刺穿|风刃|烈火|纸人|空气弹|控火|灵体之线|操纵师|秘偶|无面人|占卜家|魔术师)", RegexOptions.IgnoreCase);
        var combatDescRegex = new Regex(@"(magic damage|physical damage|magic ATK|physical ATK|restores.*Health|recovers.*Health|deals.*damage|dealing.*damage|damage to enemies|Induction Mark|Healing Reduction|Super Armor|Imprisonment|Vulnerability|Hypnosis|Crowd Control|Knocking Down|Stun the target|Silence the target)", RegexOptions.IgnoreCase);

        var candidates = new List<Tuple<int, string, string>>();

        int lineNum = 0;
        using (var reader = new StreamReader(GeminiPath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNum++;
                string t = line.Trim();
                if (t.StartsWith("[\""))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 2)
                    {
                        string cnKey = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : (t.EndsWith("\"") ? t.Length - 1 : -1);
                        if (valEnd >= valStart)
                        {
                            string enVal = t.Substring(valStart, valEnd - valStart);

                            if (existingRu.ContainsKey(cnKey) && existingRu.ContainsKey(enVal))
                                continue;

                            bool isCandidate = formulaRegex.IsMatch(enVal)
                                            || stanceRegex.IsMatch(enVal)
                                            || skillNodeRegex.IsMatch(cnKey) || skillNodeRegex.IsMatch(enVal)
                                            || (combatDescRegex.IsMatch(enVal) && enVal.Length > 20);

                            if (isCandidate)
                            {
                                candidates.Add(Tuple.Create(lineNum, cnKey, enVal));
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Отобрано для перевода строк способностей/формул: {0}", candidates.Count));
        if (candidates.Count == 0)
        {
            Console.WriteLine("Все боевые описания уже переведены!");
            return;
        }

        // 3. Параллельный перевод с защитой токенов (3 потока с паузой для избежания 429)
        var translatedResults = new ConcurrentDictionary<string, string>();
        int processed = 0;
        int successes = 0;

        Parallel.ForEach(candidates, new ParallelOptions { MaxDegreeOfParallelism = 3 }, item =>
        {
            string cn = item.Item2;
            string en = item.Item3;

            string ru = TranslateSingleWithRetry(en);
            if (!string.IsNullOrWhiteSpace(ru))
            {
                // Применение каноничных терминов
                foreach (var pair in CanonMap)
                {
                    ru = Regex.Replace(ru, @"\b" + Regex.Escape(pair.Key) + @"\b", pair.Value, RegexOptions.IgnoreCase);
                }

                // Исправление частых игровых сокращений
                ru = Regex.Replace(ru, @"\bмагический урон\b", "маг. урона", RegexOptions.IgnoreCase);
                ru = Regex.Replace(ru, @"\bфизический урон\b", "физ. урона", RegexOptions.IgnoreCase);
                ru = Regex.Replace(ru, @"\bсекунд(ы|а)?\b", "сек.", RegexOptions.IgnoreCase);

                // Экранирование для Lua
                ru = EscapeForLua(ru);

                translatedResults[cn] = ru;
                translatedResults[en] = ru;
                Interlocked.Increment(ref successes);
            }

            int p = Interlocked.Increment(ref processed);
            if (p % 20 == 0 || p == candidates.Count)
            {
                Console.Write("\rПрогресс перевода навыков: " + p + " / " + candidates.Count + " (Успешно: " + successes + ")");
            }
            Thread.Sleep(80);
        });

        Console.WriteLine("\nПеревод завершен! Успешно получено ключей: " + translatedResults.Count);

        // 4. Слияние со словарем
        foreach (var kvp in translatedResults)
        {
            if (!existingRu.ContainsKey(kvp.Key))
            {
                orderedKeys.Add(kvp.Key);
            }
            existingRu[kvp.Key] = kvp.Value;
        }

        // 5. Запись в RuntimeTextRussian.lua
        Console.WriteLine(string.Format("Сохранение обновленного словаря ({0} записей) в {1}...", existingRu.Count, RuPath));
        SaveLuaDictionary(RuPath, orderedKeys, existingRu);

        // Копирование в data/ и в игру
        try
        {
            File.Copy(RuPath, DataRuPath, true);
            Console.WriteLine("Синхронизировано с data/RuntimeTextRussian.lua");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Предупреждение при копировании в data: " + ex.Message);
        }

        try
        {
            if (File.Exists(GameRuPath) || Directory.Exists(Path.GetDirectoryName(GameRuPath)))
            {
                File.Copy(RuPath, GameRuPath, true);
                Console.WriteLine("Синхронизировано с клиентом игры: " + GameRuPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Предупреждение при копировании в игру: " + ex.Message);
        }

        Console.WriteLine("Готово!");
    }

    static string EscapeForLua(string text)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == '"')
            {
                if (i == 0 || text[i - 1] != '\\')
                {
                    sb.Append('\\');
                }
                sb.Append(c);
            }
            else if (c == '\r')
            {
                // пропуск
            }
            else if (c == '\n')
            {
                sb.Append("\\n");
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    static void SaveLuaDictionary(string filePath, List<string> orderedKeys, Dictionary<string, string> dict)
    {
        using (var sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
        {
            sw.WriteLine("-- Generated Russian Translation Dictionary for Lord of Mysteries");
            sw.WriteLine(string.Format("-- Entries: {0}", dict.Count));
            sw.WriteLine("return {");
            foreach (var key in orderedKeys)
            {
                string val = dict[key];
                sw.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", key, val));
            }
            sw.WriteLine("}");
        }
    }

    static string TranslateSingleWithRetry(string text)
    {
        for (int retry = 0; retry < 3; retry++)
        {
            string res = TranslateSingle(text, retry);
            if (!string.IsNullOrWhiteSpace(res)) return res;
            Thread.Sleep(300 * (retry + 1));
        }
        return null;
    }

    static string TranslateSingle(string text, int attempt)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        var tagMap = new Dictionary<string, string>();
        int tagIdx = 0;

        string protectedText = Regex.Replace(text, @"<[^>]+>|\{[^{}]+\}|\\n|\*d\*\*|\*d|\*f\*\*|\*f|mul\([^)]+\)|spellfielddisc\([^)]+\)|buffdisc\([^)]+\)|bulletdisc\([^)]+\)|buffappear\([^)]+\)|CheckStar\([^)]+\)|%[sdif]", m =>
        {
            string ph = "XTAG" + (tagIdx++) + "X";
            tagMap[ph] = m.Value;
            return ph;
        });

        string client = (attempt % 2 == 0) ? "dict-chrome-ex" : "gtx";
        string url = "https://translate.googleapis.com/translate_a/single?client=" + client + "&sl=en&tl=ru&dt=t&q=" + Uri.EscapeDataString(protectedText);

        try
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                byte[] data = wc.DownloadData(url);
                string json = Encoding.UTF8.GetString(data);

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
                if (string.IsNullOrWhiteSpace(result)) return null;

                // Нормализация возможных искажений плейсхолдеров
                result = Regex.Replace(result, @"X\s*TAG\s*(\d+)\s*X", "XTAG$1X", RegexOptions.IgnoreCase);

                // Восстановление тегов
                foreach (var kvp in tagMap)
                {
                    result = result.Replace(kvp.Key, kvp.Value);
                }

                return result;
            }
        }
        catch
        {
            return null;
        }
    }
}
