using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslateBatch2Items
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string DataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
    static string GameRuPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";
    static string LsiDir = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes";

    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Gods and Entities
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
        { "Seer Pathway", "Путь Провидца" },
        { "The Fool Pathway", "Путь Шута" },
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

        // Items, Potions, Materials & Artifacts Canon
        { "Beyonder Characteristic", "Потустороннее свойство" },
        { "Beyonder Characteristics", "Потусторонние свойства" },
        { "Beyonder Material", "Потусторонний материал" },
        { "Beyonder Materials", "Потусторонние материалы" },
        { "Beyonder", "Потусторонний" },
        { "Beyonders", "Потусторонние" },
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "Mystical Item", "Мистический предмет" },
        { "Mystical Items", "Мистические предметы" },
        { "Main Ingredient", "Основной ингредиент" },
        { "Main Ingredients", "Основные ингредиенты" },
        { "Supplementary Ingredient", "Дополнительный ингредиент" },
        { "Supplementary Ingredients", "Дополнительные ингредиенты" },
        { "Auxiliary Ingredient", "Дополнительный ингредиент" },
        { "Auxiliary Ingredients", "Дополнительные ингредиенты" },
        { "Acting Method", "Метод Лицедейства" },
        { "Paper Figurine Substitute", "Замена бумажным человечком" },
        { "Spirit Body Threads", "Нити духовного тела" },
        { "Historical Projection", "Историческая проекция" },
        { "Flame Controlling", "Управление пламенем" },
        { "Holy Water", "Святая вода" },
        { "Spirituality", "Духовность" },
        { "Spirit World", "Мир Духов" },
        { "Spirit Body", "Духовное тело" },
        { "Astral Projection", "Астральная проекция" },

        // Combat Stats & Equipment
        { "Physical ATK", "Физ. атака" },
        { "Magic ATK", "Маг. атака" },
        { "Physical DEF", "Физ. защита" },
        { "Magic DEF", "Маг. защита" },
        { "Crit Rate", "Шанс крита" },
        { "Crit Damage", "Крит. урон" },
        { "Damage Reduction", "Снижение урона" },
        { "Super Armor", "Суперброня" },
        { "Cleanse Skill", "Снятие контроля" },
        { "Cleanse", "Снятие контроля" },
        { "Cooldown Reduction", "Сокращение перезарядки" },
        { "Cooldown", "Перезарядка" },
        { "Life Steal", "Вампиризм" },
        { "Lifesteal", "Вампиризм" },
        { "Attack Speed", "Скорость атаки" },
        { "Movement Speed", "Скорость бега" },
        { "Move Speed", "Скорость бега" },

        // Currencies & Lore Locations
        { "Gold Pound", "Золотой фунт" },
        { "Gold Pounds", "Золотых фунтов" },
        { "Pound", "Фунт" },
        { "Pounds", "Фунтов" },
        { "Soli", "Суле" },
        { "Sol", "Суле" },
        { "Pence", "Пенсов" },
        { "Penny", "Пенни" },
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
        { "Feynapotter", "Фейнапоттер" }
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
        Console.WriteLine("   LOTM - Пакет №2: Предметы, экипировка, зелья и артефакты ");
        Console.WriteLine("==========================================================");
        Console.WriteLine(string.Format("Целевой объем перевода: {0} уникальных строк", targetCount));

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

        // 3. Сбор кандидатов по категориям приоритета
        var p1Potions = new List<Tuple<string, string>>();      // Рецепты и зелья
        var p2Materials = new List<Tuple<string, string>>();    // Материалы и свойства
        var p3Artifacts = new List<Tuple<string, string>>();    // Запечатанные артефакты
        var p4Items = new List<Tuple<string, string>>();        // Прямые теги предметов
        var p5Equip = new List<Tuple<string, string>>();        // Оружие и экипировка

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

                        string sk = SourceKey(cnKey);
                        bool isItemTag = false;
                        if (lsiTagMap.ContainsKey(sk))
                        {
                            foreach (var tag in lsiTagMap[sk])
                            {
                                if (tag.StartsWith("item") || tag.StartsWith("equip") || tag.StartsWith("potion") ||
                                    tag.StartsWith("formula") || tag.StartsWith("artifact") || tag.StartsWith("prop"))
                                {
                                    isItemTag = true;
                                    break;
                                }
                            }
                        }

                        bool isPotionOrFormula = cnKey.Contains("魔药") || cnKey.Contains("配方") || enVal.Contains("Potion") || enVal.Contains("Formula") || enVal.Contains("Recipe");
                        bool isMaterial = cnKey.Contains("主材料") || cnKey.Contains("辅助材料") || cnKey.Contains("非凡特性") || enVal.Contains("Main Ingredient") || enVal.Contains("Supplementary Ingredient") || enVal.Contains("Beyonder Characteristic");
                        bool isArtifact = cnKey.Contains("封印物") || enVal.Contains("Sealed Artifact") || enVal.Contains("Artifact 2-") || enVal.Contains("Artifact 3-") || enVal.Contains("Artifact 1-") || enVal.Contains("Artifact 0-");
                        bool isEquip = cnKey.Contains("装备") || cnKey.Contains("长袍") || cnKey.Contains("手套") || cnKey.Contains("靴") || enVal.Contains("Equipment") || enVal.Contains("Robe") || enVal.Contains("Boots") || enVal.Contains("Gloves") || enVal.Contains("Ring") || enVal.Contains("Necklace");

                        if (isPotionOrFormula)
                        {
                            seenCn.Add(cnKey);
                            p1Potions.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isMaterial)
                        {
                            seenCn.Add(cnKey);
                            p2Materials.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isArtifact)
                        {
                            seenCn.Add(cnKey);
                            p3Artifacts.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isItemTag)
                        {
                            seenCn.Add(cnKey);
                            p4Items.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (isEquip)
                        {
                            seenCn.Add(cnKey);
                            p5Equip.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Найдено кандидатов:\n  - P1 (Зелья и рецепты): {0}\n  - P2 (Материалы и свойства): {1}\n  - P3 (Запечатанные артефакты): {2}\n  - P4 (Предметы инвентаря): {3}\n  - P5 (Экипировка и снаряжение): {4}",
            p1Potions.Count, p2Materials.Count, p3Artifacts.Count, p4Items.Count, p5Equip.Count));

        // Формирование финального набора
        var candidates = new List<Tuple<string, string>>();
        candidates.AddRange(p1Potions);
        candidates.AddRange(p2Materials);
        candidates.AddRange(p3Artifacts);

        foreach (var c in p4Items)
        {
            if (candidates.Count >= targetCount) break;
            candidates.Add(c);
        }

        foreach (var c in p5Equip)
        {
            if (candidates.Count >= targetCount) break;
            candidates.Add(c);
        }

        Console.WriteLine(string.Format("Итого отобрано для перевода в Пакет №2: {0} уникальных строк", candidates.Count));

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

                // Очистка и нормализация диалогов/описаний
                ru = PostProcessItemDescription(ru);

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

        Console.WriteLine("Пакет №2 успешно интегрирован!");
    }

    static string ApplyCanonGlossary(string text)
    {
        string res = text;
        foreach (var pair in CanonExact)
        {
            res = Regex.Replace(res, @"\b" + Regex.Escape(pair.Key) + @"\b", pair.Value, RegexOptions.IgnoreCase);
        }

        // Строгие каноничные исправления падежей и машинных опечаток для предметов
        res = Regex.Replace(res, @"\b(Потусторонняя характеристика|потусторонняя характеристика|потусторонней характеристики|потустороннюю характеристику|потусторонней характеристике)\b", "Потустороннее свойство", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Потусторонние характеристики|потусторонние характеристики|потусторонних характеристик)\b", "Потусторонние свойства", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Запечатанные данные артефакта)\b", "Данные запечатанного артефакта", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Запечатанная реликвия|запечатанная реликвия|запечатанные реликвии)\b", "Запечатанный артефакт", RegexOptions.IgnoreCase);
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
        res = Regex.Replace(res, @"\b(переваривание зелья|переваривание)\b", "усвоение зелья", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(метод действия)\b", "Метод Лицедейства", RegexOptions.IgnoreCase);

        // Боевые атрибуты
        res = Regex.Replace(res, @"\b(Физ\. Атк|Физ Атк|Физ\. Атака)\b", "Физ. атака", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Маг\. Атк|Маг Атк|Маг\. Атака)\b", "Маг. атака", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Физ\. Деф|Физ Деф|Физ\. Защита)\b", "Физ. защита", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Маг\. Деф|Маг Деф|Маг\. Защита)\b", "Маг. защита", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Шанс Крит|Шанс Крита)\b", "Шанс крита", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Крит Урон)\b", "Крит. урон", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Снижение Урона)\b", "Снижение урона", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Скорость Передвижения)\b", "Скорость бега", RegexOptions.IgnoreCase);

        return res;
    }

    static string PostProcessItemDescription(string text)
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

        // Защита RichText тегов и макросов
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

                // Нормализация плейсхолдеров
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
