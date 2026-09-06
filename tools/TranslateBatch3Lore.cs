using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslateBatch3Lore
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string DataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
    static string GameRuPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";
    static string LsiDir = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes";

    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Gods and High Entities
        { "The Fool", "Шут" },
        { "Lord of Mysteries", "Повелитель Тайн" },
        { "Lord of the Mysteries", "Повелитель Тайн" },
        { "Evernight Goddess", "Богиня Вечной Ночи" },
        { "Lord of Storms", "Владыка Шторма" },
        { "God of Steam and Machinery", "Бог Пара и Машин" },
        { "God of Knowledge and Wisdom", "Бог Знаний и Мудрости" },
        { "Eternal Blazing Sun", "Вечно Пылающее Солнце" },
        { "Mother Earth", "Мать-Земля" },
        { "God of Combat", "Бог Битвы" },
        { "True Creator", "Истинный Творец" },
        { "The True Creator", "Истинный Творец" },
        { "Original Creator", "Изначальный Творец" },
        { "Great Old Ones", "Великие Древние" },
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
        { "Xio Derecha", "Сио Дереча" },
        { "Xio", "Сио" },
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

        // Pathways & Sequences
        { "The Fool Pathway", "Путь Шута" },
        { "Seer Pathway", "Путь Провидца" },
        { "Spectator Pathway", "Путь Зрителя" },
        { "Sleepless Pathway", "Путь Бессонного" },
        { "Hunter Pathway", "Путь Охотника" },
        { "Apprentice Pathway", "Путь Ученика" },
        { "Arbiter Pathway", "Путь Арбитра" },
        { "Bizarro Sorcerer", "Маг Непостижимого" },
        { "Scholar of Yore", "Учёный Прошлого" },
        { "Miracle Invoker", "Творец Чудес" },
        { "Attendant of Mysteries", "Служитель Тайн" },
        { "Marionettist", "Марионеточник" },
        { "Faceless", "Безликий" },
        { "Magician", "Фокусник" },
        { "Clown", "Клоун" },
        { "Seer", "Провидец" },
        { "Midnight Poet", "Полуночный Поэт" },
        { "Nightmare", "Кошмар" },
        { "Sleepless", "Бессонный" },
        { "Telepathist", "Телепат" },
        { "Psychiatrist", "Психиатр" },
        { "Spectator", "Зритель" },
        { "Provoker", "Провокатор" },
        { "Pyromaniac", "Пироман" },
        { "Reaper", "Жнец" },
        { "Hunter", "Охотник" },
        { "Trickmaster", "Мастер Трикстер" },
        { "Astrologer", "Астролог" },
        { "Apprentice", "Ученик" },
        { "Sheriff", "Шериф" },
        { "Judge", "Судья" },
        { "Arbiter", "Арбитр" },

        // Items, Concepts & Artifacts
        { "Beyonder Characteristic", "Потустороннее свойство" },
        { "Beyonder Characteristics", "Потусторонние свойства" },
        { "Beyonder Material", "Потусторонний материал" },
        { "Beyonder Materials", "Потусторонние материалы" },
        { "Beyonder", "Потусторонний" },
        { "Beyonders", "Потусторонние" },
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "Acting Method", "Метод Лицедейства" },
        { "Paper Figurine Substitute", "Замена бумажным человечком" },
        { "Spirit Body Threads", "Нити духовного тела" },
        { "Historical Projection", "Историческая проекция" },
        { "Flame Controlling", "Управление пламенем" },
        { "Spirit World", "Мир Духов" },
        { "Spirit Body", "Духовное тело" },
        { "Astral Projection", "Астральная проекция" },

        // Currencies, Newspapers & Places
        { "Gold Pound", "Золотой фунт" },
        { "Gold Pounds", "Золотых фунтов" },
        { "Pound", "Фунт" },
        { "Pounds", "Фунтов" },
        { "Soli", "Суле" },
        { "Sol", "Суле" },
        { "Pence", "Пенсов" },
        { "Penny", "Пенни" },
        { "Tingen Daily", "Тингенский ежедневный вестник" },
        { "Backlund Morning News", "Утренние новости Бэкланда" },
        { "Morning Post", "Утренняя почта" },
        { "Daily News", "Ежедневные новости" },
        { "Nighthawks", "Ночные Ястребы" },
        { "Nighthawk", "Ночной Ястреб" },
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
        { "Tussock", "Тасок" }
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
        int targetCount = 1800;
        if (args.Length > 0) int.TryParse(args[0], out targetCount);

        Console.WriteLine("==========================================================");
        Console.WriteLine("   LOTM - Пакет №3: Записки, газеты и Дневники Розеля    ");
        Console.WriteLine("==========================================================");
        Console.WriteLine(string.Format("Целевой объем перевода: {0} уникальных записей", targetCount));

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
        var lsiTagMap = new Dictionary<string, List<string>>();
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
                            string rest = line.Substring(end + 2).Trim();
                            var tags = new List<string>();
                            var matches = Regex.Matches(rest, @"""([a-zA-Z0-9_]+):(\d+)""");
                            foreach (Match m in matches)
                            {
                                tags.Add(m.Groups[1].Value);
                            }
                            if (tags.Count > 0)
                            {
                                lsiTagMap[hashKey] = tags;
                            }
                        }
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Загружено {0} LSI индексов.", lsiTagMap.Count));

        // 3. Отбор кандидатов для Пакета №3
        var p1Roselle = new List<Tuple<string, string>>();      // Дневники и тексты Розеля
        var p2DiariesBooks = new List<Tuple<string, string>>(); // Дневники, книги, рукописи, хроники
        var p3Letters = new List<Tuple<string, string>>();      // Письма, записки
        var p4Newspapers = new List<Tuple<string, string>>();   // Газеты и периодика
        var p5Gossip = new List<Tuple<string, string>>();       // Городские слухи, уличные истории

        var seenCn = new HashSet<string>(StringComparer.Ordinal);

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

                        if (seenCn.Contains(cnKey))
                            continue;

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        // Игнорируем внутренние отладочные ID
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID"))
                            continue;

                        string sk = SourceKey(cnKey);
                        var tags = lsiTagMap.ContainsKey(sk) ? lsiTagMap[sk] : new List<string>();

                        bool isRoselle = enVal.Contains("Roselle") || cnKey.Contains("罗塞尔");
                        bool isDiaryOrBook = cnKey.Contains("日记") || cnKey.Contains("手稿") || cnKey.Contains("文献") ||
                                             cnKey.Contains("典籍") || tags.Contains("book") ||
                                             Regex.IsMatch(enVal, @"\b(Diary|Manuscript|Chronicle|Journal)\b");
                        bool isLetter = tags.Contains("lettertext") || cnKey.Contains("信件") || cnKey.Contains("书信") ||
                                        Regex.IsMatch(enVal, @"\b(Letter|Letter to|Dear |From:)\b");
                        bool isNewspaper = tags.Contains("newspaper") || cnKey.Contains("报纸") || cnKey.Contains("日报") ||
                                           cnKey.Contains("晨报") || Regex.IsMatch(enVal, @"\b(Newspaper|Daily|Morning Post|Tingen Daily|Backlund Morning News)\b");
                        bool isGossip = tags.Contains("gossip") && enVal.Length >= 15;

                        if (isRoselle)
                        {
                            seenCn.Add(cnKey);
                            p1Roselle.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isDiaryOrBook)
                        {
                            seenCn.Add(cnKey);
                            p2DiariesBooks.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isLetter)
                        {
                            seenCn.Add(cnKey);
                            p3Letters.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isNewspaper)
                        {
                            seenCn.Add(cnKey);
                            p4Newspapers.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isGossip)
                        {
                            seenCn.Add(cnKey);
                            p5Gossip.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Найдено кандидатов:\n  - P1 (Розель): {0}\n  - P2 (Дневники и книги): {1}\n  - P3 (Письма): {2}\n  - P4 (Газеты): {3}\n  - P5 (Слухи и лор): {4}",
            p1Roselle.Count, p2DiariesBooks.Count, p3Letters.Count, p4Newspapers.Count, p5Gossip.Count));

        // Формирование набора
        var candidates = new List<Tuple<string, string>>();
        candidates.AddRange(p1Roselle);
        candidates.AddRange(p2DiariesBooks);
        candidates.AddRange(p3Letters);
        candidates.AddRange(p4Newspapers);

        foreach (var c in p5Gossip)
        {
            if (candidates.Count >= targetCount) break;
            candidates.Add(c);
        }

        Console.WriteLine(string.Format("Итого отобрано для перевода в Пакет №3: {0} уникальных строк", candidates.Count));

        // 4. Параллельный перевод с защитой разметки
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
                // Применение каноничного глоссария
                ru = ApplyCanonGlossary(ru);

                // Очистка и нормализация
                ru = PostProcessLore(ru);

                // Экранирование для Lua
                string escapedRu = CleanForLua(ru);
                string escapedCn = CleanForLua(cn);
                string escapedEn = CleanForLua(en);

                translatedResults[escapedCn] = escapedRu;
                translatedResults[escapedEn] = escapedRu;
                Interlocked.Increment(ref successes);
            }

            int p = Interlocked.Increment(ref processed);
            if (p % 25 == 0 || p == candidates.Count)
            {
                Console.Write(string.Format("\rПрогресс перевода: {0} / {1} (Успешно: {2})", p, candidates.Count, successes));
            }
            Thread.Sleep(65);
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

        // 6. Сохранение в RuntimeTextRussian.lua
        Console.WriteLine(string.Format("Сохранение в {0} (всего {1} записей)...", RuPath, existingRu.Count));
        SaveLuaDictionary(RuPath, orderedKeys, existingRu);

        // 7. Синхронизация
        try
        {
            File.Copy(RuPath, DataRuPath, true);
            Console.WriteLine("✅ Синхронизировано с data/RuntimeTextRussian.lua");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка data: " + ex.Message);
        }

        try
        {
            if (File.Exists(GameRuPath) || Directory.Exists(Path.GetDirectoryName(GameRuPath)))
            {
                File.Copy(RuPath, GameRuPath, true);
                Console.WriteLine("✅ Синхронизировано с клиентом игры: " + GameRuPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка game: " + ex.Message);
        }

        Console.WriteLine("Пакет №3 успешно интегрирован!");
    }

    static string ApplyCanonGlossary(string text)
    {
        string res = text;
        foreach (var pair in CanonExact)
        {
            res = Regex.Replace(res, @"\b" + Regex.Escape(pair.Key) + @"\b", pair.Value, RegexOptions.IgnoreCase);
        }

        // Исправления частых машинно-переводческих отклонений в лоре
        res = Regex.Replace(res, @"\b(Потусторонняя характеристика|потусторонняя характеристика|потусторонней характеристики|потустороннюю характеристику|потусторонней характеристике)\b", "Потустороннее свойство", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Потусторонние характеристики|потусторонние характеристики|потусторонних характеристик)\b", "Потусторонние свойства", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Мистер Дурак|господин Дурак|Господин Дурак|мистер Дурак)\b", "Мистер Шут", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Повелитель тайн|повелитель тайн)\b", "Повелитель Тайн", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Черный чертополох|Черный Терновник|Черный терновник|Блэкторн)\b", "Чёрный Чертополох", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Старый Нил|старый Нил|старику Нилу|старика Нила)\b", "Старина Нил", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Кляйн|Кляйна|Кляйну|Кляйне)\b", "Клейн", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Антигонуса|Антигонус)\b", "Антигона", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Райел Бибер|Рейл Бибер|Риэль Бибер)\b", "Райэль Бибер", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(золотой фунт|золотых фунтов|золотых фунта)\b", "золотых фунтов", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(1 золотых фунтов)\b", "1 золотой фунт", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(пенсов|пенни|пенса)\b", "пенсов", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(1 пенсов)\b", "1 пенс", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(солей|соля|соли)\b", "суле", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Тингенская газета|Тингенской газете|Тингенский ежедневник)\b", "Тингенский ежедневный вестник", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Бэкландские утренние новости)\b", "Утренние новости Бэкланда", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(переваривание зелья|переваривание)\b", "усвоение зелья", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(метод действия)\b", "Метод Лицедейства", RegexOptions.IgnoreCase);

        return res;
    }

    static string PostProcessLore(string text)
    {
        string res = text;
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
            Thread.Sleep(250 * (retry + 1));
        }
        return null;
    }

    static string TranslateSingle(string text, int attempt)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        var tagMap = new Dictionary<string, string>();
        int tagIdx = 0;

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

                result = Regex.Replace(result, @"X\s*TAG\s*(\d+)\s*X", "XTAG$1X", RegexOptions.IgnoreCase);

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
