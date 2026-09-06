using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslateBatch1Prologue
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string DataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
    static string GameRuPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";
    static string LsiDir = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes";

    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Titles and Entities
        { "The Fool", "Шут" },
        { "Lord of Mysteries", "Повелитель Тайн" },
        { "Lord of the Mysteries", "Повелитель Тайн" },
        { "Evernight Goddess", "Богиня Вечной Ночи" },
        { "Lord of Storms", "Владыка Шторма" },
        { "God of Steam and Machinery", "Бог Пара и Машин" },
        { "God of Knowledge and Wisdom", "Бог Знаний и Мудрости" },
        { "Eternal Blazing Sun", "Вечно Пылающее Солнце" },
        { "Mother Earth", "Мать-Земля" },
        { "True Creator", "Истинный Творец" },
        { "The True Creator", "Истинный Творец" },
        { "Original Creator", "Изначальный Творец" },
        { "Above the Grey Fog", "Над Серым Туманом" },
        { "Tarot Club", "Клуб Таро" },

        // Characters
        { "Klein Moretti", "Клейн Моретти" },
        { "Klein", "Клейн" },
        { "Melissa Moretti", "Мелисса Моретти" },
        { "Melissa", "Мелисса" },
        { "Benson Moretti", "Бенсон Моретти" },
        { "Benson", "Бенсон" },
        { "Dunn Smith", "Данн Смит" },
        { "Dunn", "Данн" },
        { "Old Neil", "Старина Нил" },
        { "Leonard Mitchell", "Леонард Митчелл" },
        { "Leonard", "Леонард" },
        { "Frye", "Фрай" },
        { "Daly Simone", "Дейли Симон" },
        { "Daly", "Дейли" },
        { "Audrey Hall", "Одри Холл" },
        { "Audrey", "Одри" },
        { "Susie", "Сьюзи" },
        { "Alger Wilson", "Алджер Уилсон" },
        { "Alger", "Алджер" },
        { "Fors Wall", "Форс Уолл" },
        { "Fors", "Форс" },
        { "Azik Eggers", "Азик Эггерс" },
        { "Azik", "Азик" },
        { "Roselle Gustav", "Розель Густав" },
        { "Emperor Roselle", "Император Розель" },
        { "Roselle", "Розель" },
        { "Bernadette", "Бернадетт" },
        { "Mr. Fool", "Мистер Шут" },
        { "Miss Justice", "Мисс Справедливость" },
        { "Mr. Hanged Man", "Мистер Повешенный" },
        { "Miss Magician", "Мисс Фокусник" },
        { "Miss Judgment", "Мисс Судья" },
        { "Mr. Moon", "Мистер Луна" },
        { "Mr. Sun", "Мистер Солнце" },
        { "Mr. Star", "Мистер Звезда" },
        { "Antigonus Family", "Семья Антигона" },
        { "Antigonus", "Антигон" },
        { "Riel Bieber", "Райэль Бибер" },
        { "Ray Bieber", "Райэль Бибер" },
        { "Welch McGovern", "Уэлч Макговерн" },
        { "Welch", "Уэлч" },
        { "Naya", "Ная" },

        // Organizations & Locations
        { "Nighthawk", "Ночной Ястреб" },
        { "Nighthawks", "Ночные Ястребы" },
        { "Mandated Punishers", "Уполномоченные Каратели" },
        { "Machinery Hivemind", "Механический Разум" },
        { "Blackthorn Security Company", "Охранная компания «Чёрный Чертополох»" },
        { "Blackthorn", "Чёрный Чертополох" },
        { "Tingen", "Тинген" },
        { "Backlund", "Бэкланд" },
        { "Trier", "Трир" },
        { "Loen Kingdom", "Королевство Лоэн" },
        { "Loen", "Лоэн" },
        { "Intis Republic", "Интисская Республика" },
        { "Intis", "Интис" },
        { "Feysac Empire", "Империя Фейсак" },
        { "Feysac", "Фейсак" },
        { "Feynapotter Kingdom", "Королевство Фейнапоттер" },
        { "Feynapotter", "Фейнапоттер" },
        { "Tussock River", "Река Тасок" },
        { "Tussock", "Тасок" },
        { "Hornacis Mountain Range", "Горный хребет Хорнакис" },
        { "Hornacis", "Хорнакис" },
        { "Zoutland Street", "Улица Зотланд" },
        { "Zoutland", "Зотланд" },
        { "Daffodil Street", "Улица Нарциссов" },
        { "Iron Cross Street", "Улица Железного Креста" },
        { "Howls Street", "Улица Хаулс" },
        { "Golden Elm Street", "Улица Золотого Вяза" },
        { "Golden Autumn Lake", "Озеро Золотой Осени" },
        { "Tingen Lake", "Озеро Тинген" },
        { "Pritz Harbor", "Порт Приц" },
        { "Raphael Cemetery", "Кладбище Рафаэля" },
        { "Divination Club", "Клуб Гаданий" },

        // Pathways & Beyonder Mechanics
        { "Beyonder", "Потусторонний" },
        { "Beyonders", "Потусторонние" },
        { "Beyonder Characteristic", "Потустороннее свойство" },
        { "Beyonder Characteristics", "Потусторонние свойства" },
        { "Beyonder Material", "Потусторонний материал" },
        { "Beyonder Materials", "Потусторонние материалы" },
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "Mystical Item", "Мистический предмет" },
        { "Mystical Items", "Мистические предметы" },
        { "Spirit Body Threads", "Нити духовного тела" },
        { "Spirit Body Thread", "Нить духовного тела" },
        { "Spirit Body", "Духовное тело" },
        { "Astral Projection", "Астральная проекция" },
        { "Historical Projection", "Историческая проекция" },
        { "Historical Projections", "Исторические проекции" },
        { "Paper Figurine Substitute", "Замена бумажным человечком" },
        { "Paper Figurine Substitutes", "Замена бумажным человечком" },
        { "Paper Figurine", "Бумажный человечек" },
        { "Paper Figurines", "Бумажные человечки" },
        { "Flame Controlling", "Управление пламенем" },
        { "Acting Method", "Метод Лицедейства" },
        { "Spirituality", "Духовность" },
        { "Sanity", "Рассудок" },
        { "Madness", "Безумие" },
        { "Loss of Control", "Потеря контроля" },
        { "Corruption", "Искажение" },
        { "Divination", "Гадание" },
        { "Spirit World", "Мир Духов" },
        { "Sequence", "Последовательность" },
        { "Potion", "Зелье" },
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
        { "Marionette", "Марионетка" },
        { "Marionettes", "Марионетки" },

        // Terms and gameplay
        { "Dowsing Rod Navigation", "Поиск лозой" },
        { "Dowsing Rod", "Лозоходство" },
        { "Travel Obelisk", "Путевой обелиск" },
        { "Single-Player Instance", "Одиночный инстанс" },
        { "Solo Instance", "Одиночный инстанс" },
        { "Multiplayer Scene", "Групповая сцена" },
        { "Open World", "Открытый мир" },
        { "Dungeon", "Подземелье" },
        { "Instance", "Инстанс" },
        { "Damage Reduction", "Снижение урона" },
        { "Super Armor", "Суперброня" },
        { "Cleanse", "Снятие контроля" },
        { "Cooldown Reduction", "Сокращение перезарядки" },
        { "Cooldown", "Перезарядка" },
        { "Mind Fire", "Пламя разума" },
        { "Induction Mark", "Метка внушения" },
        { "Hypnosis", "Гипноз" },
        { "Imprisonment", "Заточение" },
        { "Physical Damage", "Физ. урон" },
        { "Magic Damage", "Маг. урон" },
        { "True Damage", "Чистый урон" },
        { "Physical ATK", "Физ. атака" },
        { "Magic ATK", "Маг. атака" },
        { "Physical DEF", "Физ. защита" },
        { "Magic DEF", "Маг. защита" },
        { "Crit Rate", "Шанс крита" },
        { "Crit Damage", "Крит. урон" }
    };

    static string SourceKey(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        uint hash = 2166136261u;
        for (int i = 0; i < bytes.Length; i++)
        {
            hash ^= bytes[i];
            hash = unchecked(hash + (hash << 1) + (hash << 4) + (hash << 7) + (hash << 8) + (hash << 24));
        }
        return bytes.Length + ":" + hash.ToString("x8");
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        int targetCount = 1000;
        if (args.Length > 0) int.TryParse(args[0], out targetCount);

        Console.WriteLine("==========================================================");
        Console.WriteLine("   LOTM - Пакет №1: Сюжетный пролог и диалоги Тингена    ");
        Console.WriteLine("==========================================================");
        Console.WriteLine(string.Format("Целевой объем перевода: {0} ключевых строк", targetCount));

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
        Console.WriteLine(string.Format("Существующих записей в словаре: {0}", existingRu.Count));

        // 2. Сбор LSI тегов
        var lsiTagMap = new Dictionary<string, string>();
        if (Directory.Exists(LsiDir))
        {
            foreach (var file in Directory.GetFiles(LsiDir, "LanguageSourceIndex_*.lua"))
            {
                foreach (var line in File.ReadAllLines(file, Encoding.UTF8))
                {
                    int start = line.IndexOf("[\"");
                    if (start >= 0)
                    {
                        int end = line.IndexOf("\"]", start + 2);
                        if (end > start)
                        {
                            string hashKey = line.Substring(start + 2, end - start - 2);
                            string tag = line.Substring(end + 2).Trim();
                            lsiTagMap[hashKey] = tag;
                        }
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Загружено {0} LSI индексов.", lsiTagMap.Count));

        // 3. Отбор кандидатов с приоритизацией
        var p1List = new List<Tuple<string, string>>(); // прямой пролог
        var p2List = new List<Tuple<string, string>>(); // персонажи: Клейн, Мелисса, Бенсон, Данн, Нил, Леонард, 2-049
        var p3List = new List<Tuple<string, string>>(); // tingentalk & asidetalk
        var p4List = new List<Tuple<string, string>>(); // oldtalk & tingen world

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

                        if (existingRu.ContainsKey(cnKey) || existingRu.ContainsKey(enVal))
                            continue;

                        // Игнорируем пустые или чисто цифровые
                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        string sk = SourceKey(cnKey);
                        string tag = lsiTagMap.ContainsKey(sk) ? lsiTagMap[sk] : "";

                        bool isDirectPrologue = enVal.Contains("Prologue") || cnKey.Contains("序章");
                        bool isMainChar = enVal.Contains("Klein") || enVal.Contains("Melissa") ||
                                          enVal.Contains("Benson") || enVal.Contains("Dunn") ||
                                          enVal.Contains("Old Neil") || enVal.Contains("Leonard") ||
                                          enVal.Contains("Blackthorn") || enVal.Contains("Antigonus") ||
                                          enVal.Contains("2-049") || enVal.Contains("3-888") ||
                                          enVal.Contains("3-116") || enVal.Contains("Zoutland") ||
                                          enVal.Contains("Daffodil Street") || enVal.Contains("Iron Cross");

                        bool isTingenTalk = tag.Contains("tingentalk") || tag.Contains("asidetalk");
                        bool isOldTalkOrTingen = tag.Contains("oldtalk") || tag.Contains("tingen") || tag.Contains("talkother");

                        if (isDirectPrologue) p1List.Add(Tuple.Create(cnKey, enVal));
                        else if (isMainChar) p2List.Add(Tuple.Create(cnKey, enVal));
                        else if (isTingenTalk) p3List.Add(Tuple.Create(cnKey, enVal));
                        else if (isOldTalkOrTingen) p4List.Add(Tuple.Create(cnKey, enVal));
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Найдено кандидатов:\n  - P1 (Пролог прямой): {0}\n  - P2 (Главные герои и локации): {1}\n  - P3 (Диалоги Тингена / Реплики): {2}\n  - P4 (Лор и мир Тингена): {3}",
            p1List.Count, p2List.Count, p3List.Count, p4List.Count));

        // Формируем финальный список до targetCount
        var candidates = new List<Tuple<string, string>>();
        candidates.AddRange(p1List);
        foreach (var c in p2List) { if (candidates.Count < targetCount) candidates.Add(c); }
        foreach (var c in p3List) { if (candidates.Count < targetCount) candidates.Add(c); }
        foreach (var c in p4List) { if (candidates.Count < targetCount) candidates.Add(c); }

        Console.WriteLine(string.Format("Отобрано для перевода в Пакет №1: {0} строк", candidates.Count));

        // 4. Параллельный перевод
        var translatedResults = new ConcurrentDictionary<string, string>();
        int processed = 0;
        int successes = 0;

        Parallel.ForEach(candidates, new ParallelOptions { MaxDegreeOfParallelism = 8 }, item =>
        {
            string cn = item.Item1;
            string en = item.Item2;

            string ru = TranslateSingleWithRetry(en);
            if (!string.IsNullOrWhiteSpace(ru))
            {
                // Применяем каноничный глоссарий
                ru = ApplyCanonGlossary(ru);

                // Нормализация кавычек и форматирования диалогов
                ru = PostProcessDialogue(ru);

                // Очистка и экранирование для Lua
                string escapedRu = CleanForLua(ru);
                string escapedCn = CleanForLua(cn);
                string escapedEn = CleanForLua(en);

                translatedResults[escapedCn] = escapedRu;
                translatedResults[escapedEn] = escapedRu;
                Interlocked.Increment(ref successes);
            }

            int p = Interlocked.Increment(ref processed);
            if (p % 20 == 0 || p == candidates.Count)
            {
                Console.Write("\rПрогресс перевода: " + p + " / " + candidates.Count + " (Успешно: " + successes + ")");
            }
            Thread.Sleep(70);
        });

        Console.WriteLine(string.Format("\nПеревод завершен! Получено {0} новых записей словаря (CN + EN).", translatedResults.Count));

        // 5. Слияние со словарем
        foreach (var kvp in translatedResults)
        {
            if (!existingRu.ContainsKey(kvp.Key))
            {
                orderedKeys.Add(kvp.Key);
            }
            existingRu[kvp.Key] = kvp.Value;
        }

        // 6. Запись в RuntimeTextRussian.lua
        Console.WriteLine(string.Format("Сохранение в {0} (всего {1} записей)...", RuPath, existingRu.Count));
        SaveLuaDictionary(RuPath, orderedKeys, existingRu);

        // Синхронизация
        try { File.Copy(RuPath, DataRuPath, true); Console.WriteLine("✅ Синхронизировано с data/RuntimeTextRussian.lua"); } catch (Exception ex) { Console.WriteLine("Ошибка data: " + ex.Message); }
        try { if (File.Exists(GameRuPath) || Directory.Exists(Path.GetDirectoryName(GameRuPath))) { File.Copy(RuPath, GameRuPath, true); Console.WriteLine("✅ Синхронизировано с клиентом игры: " + GameRuPath); } } catch (Exception ex) { Console.WriteLine("Ошибка game: " + ex.Message); }

        Console.WriteLine("Пакет №1 успешно переведен и интегрирован!");
    }

    static string ApplyCanonGlossary(string text)
    {
        string res = text;
        foreach (var pair in CanonExact)
        {
            // Словарная граница для латиницы и кириллицы
            res = Regex.Replace(res, @"\b" + Regex.Escape(pair.Key) + @"\b", pair.Value, RegexOptions.IgnoreCase);
        }

        // Дополнительные правила для падежей и частых опечаток машинного перевода
        res = Regex.Replace(res, @"\b(Ночных Ястребов|Ночные ястребы|Ночные Ястребы)\b", "Ночные Ястребы", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Старый Нил|старый Нил|старику Нилу|старика Нила)\b", "Старина Нил", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Мистер Дурак|господин Дурак|Господин Дурак|мистер Дурак)\b", "Мистер Шут", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Повелитель тайн|повелитель тайн)\b", "Повелитель Тайн", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Потусторонняя характеристика|потусторонняя характеристика|потусторонней характеристики|потустороннюю характеристику|потусторонней характеристике)\b", "Потустороннее свойство", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Потусторонние характеристики|потусторонние характеристики|потусторонних характеристик)\b", "Потусторонние свойства", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Черный чертополох|Черный Терновник|Черный терновник|Блэкторн)\b", "Чёрный Чертополох", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(улице Нарцисс|улице Нарцисса|улица Нарцисс|улица Нарцисса)\b", "улице Нарциссов", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(улице Железный Крест|улице Железного креста)\b", "улице Железного Креста", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(золотой фунт|золотых фунтов|золотых фунта)\b", "золотых фунтов", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(1 золотых фунтов)\b", "1 золотой фунт", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(пенсов|пенни|пенса)\b", "пенсов", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(1 пенсов)\b", "1 пенс", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(суле)\b", "суле", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Антигонуса|Антигонус|семьи Антигонуса|семья Антигонуса)\b", "Антигона", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Райел Бибер|Рейл Бибер|Риэль Бибер)\b", "Райэль Бибер", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Оригинальная старая работа Dreams|Оригинальная старая работа)\b", "Старые сны оригинала", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Мировой игровой процесс)\b", "Мировые активности", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Запечатанные данные артефакта)\b", "Данные запечатанного артефакта", RegexOptions.IgnoreCase);

        return res;
    }

    static string PostProcessDialogue(string text)
    {
        string res = text;
        // Убираем машинные артефакты
        res = Regex.Replace(res, @"[a-f0-9]{32}\b", "");
        res = res.Replace(" ,", ",");
        res = res.Replace(" .", ".");
        res = res.Replace(" !", "!");
        res = res.Replace(" ?", "?");
        res = res.Replace(" :", ":");
        res = res.Replace(" ;", ";");
        res = res.Replace("( ", "(");
        res = res.Replace(" )", ")");
        return res.Trim();
    }

    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "");
        r = r.Replace("\\п", "\\n").Replace("\\т", "\\t").Replace("\\р", "\\r");
        // Избегаем двойного экранирования
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
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

        // Защита всех возможных видов тегов и плейсхолдеров
        string protectedText = Regex.Replace(text, @"<[^>]+>|\{\{[^{}]+\}\}|\{[^{}]+\}|\\n|\\t|%(\d+\$)?[-+0 #]*\d*(\.\d+)?[sdif]|\*d\*\*|\*d|\*f\*\*|\*f|mul\([^)]+\)|spellfielddisc\([^)]+\)|buffdisc\([^)]+\)|bulletdisc\([^)]+\)|buffappear\([^)]+\)|CheckStar\([^)]+\)", m =>
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
