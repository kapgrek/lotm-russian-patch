using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslateBatch17
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string DataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
    static string GameTestPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";
    static string CachePath = @"d:\gameDev\translate lotm\tools\batch17_cache.tsv";
    static object cacheLock = new object();

    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Боги и высшие сущности
        { "The Fool", "Шут" },
        { "Lord of Mysteries", "Повелитель Тайн" },
        { "Lord of the Mysteries", "Повелитель Тайн" },
        { "Evernight Goddess", "Богиня Вечной Ночи" },
        { "The Goddess", "Богиня" },
        { "Goddess", "Богиня" },
        { "Lord of Storms", "Владыка Шторма" },
        { "God of Steam and Machinery", "Бог Пара и Машин" },
        { "Church of the God of Steam and Machinery", "Церковь Бога Пара и Машин" },
        { "God of Knowledge and Wisdom", "Бог Знаний и Мудрости" },
        { "Eternal Blazing Sun", "Вечно Пылающее Солнце" },
        { "Mother Earth", "Мать-Земля" },
        { "God of Combat", "Бог Битвы" },
        { "True Creator", "Истинный Творец" },
        { "The True Creator", "Истинный Творец" },
        { "Original Creator", "Изначальный Творец" },
        { "Ancient Sun God", "Древний Бог Солнца" },
        { "Great Old Ones", "Великие Древние" },
        { "Above the Grey Fog", "Над Серым Туманом" },
        { "Grey Fog", "Серый Туман" },
        { "Gray Fog", "Серый Туман" },
        { "Tarot Club", "Клуб Таро" },
        { "Angel of Fate", "Ангел Судьбы" },
        { "King of Angels", "Король Ангелов" },
        { "Kings of Angels", "Короли Ангелов" },

        // Запечатанные артефакты
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "0-08 writing mirror", "0-08 зеркало для письма" },
        { "008 Parchment", "0-08 пергамент" },

        // Персонажи
        { "Klein Moretti", "Клейн Моретти" },
        { "Klein", "Клейн" },
        { "Little Klein", "Юный Клейн" },
        { "Gehrman Sparrow", "Герман Спэрроу" },
        { "Gehrman", "Герман" },
        { "Sherlock Moriarty", "Шерлок Мориарти" },
        { "Sherlock", "Шерлок" },
        { "Dwayne Dantes", "Дуэйн Дантес" },
        { "Melissa Moretti", "Мелисса Моретти" },
        { "Melissa", "Мелисса" },
        { "Benson Moretti", "Бенсон Моретти" },
        { "Benson", "Бенсон" },
        { "Dunn Smith", "Данн Смит" },
        { "Dunn", "Данн" },
        { "Old Neil", "Старина Нил" },
        { "Leonard Mitchell", "Леонард Митчелл" },
        { "Leonard", "Леонард" },
        { "Captain Frye", "капитан Фрай" },
        { "Frye", "Фрай" },
        { "Daly Simone", "Дейли Симон" },
        { "Daly", "Дейли" },
        { "Rozanne", "Розанна" },
        { "Audrey Hall", "Одри Холл" },
        { "Audrey", "Одри" },
        { "Susie", "Сьюзи" },
        { "Alger Wilson", "Алджер Уилсон" },
        { "Alger", "Алджер" },
        { "Fors Wall", "Форс Уолл" },
        { "Fors", "Форс" },
        { "Xio Derecha", "Сио Дереча" },
        { "Xio", "Сио" },
        { "Derrick Berg", "Деррик Берг" },
        { "Derrick", "Деррик" },
        { "Cattleya", "Каттлея" },
        { "Emlyn White", "Эмлин Уайт" },
        { "Emlyn", "Эмлин" },
        { "Danitz", "Даниц" },
        { "Blazing Danitz", "Пылающий Даниц" },
        { "Azik Eggers", "Азик Эггерс" },
        { "Azik", "Азик" },
        { "Roselle Gustav", "Розель Густав" },
        { "Emperor Roselle", "Император Розель" },
        { "Roselle", "Розель" },
        { "Bernadette", "Бернадетт" },
        { "Ouroboros", "Уроборос" },
        { "Amon", "Амон" },
        { "Medici", "Медичи" },
        { "Adam", "Адам" },
        { "Arrodes", "Арродес" },
        { "Magic Mirror", "Волшебное зеркало" },
        { "Mr. Fool", "Мистер Шут" },
        { "Miss Justice", "Мисс Справедливость" },
        { "Mr. Hanged Man", "Мистер Повешенный" },
        { "Miss Magician", "Мисс Фокусник" },
        { "Miss Judgement", "Мисс Правосудие" },
        { "The Sun", "Солнце" },
        { "The Moon", "Луна" },
        { "The Star", "Звезда" },
        { "The Hermit", "Отшельник" },
        { "The World", "Мир" },

        // Пути и Последовательности
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
        { "Soul Assigner", "Ловец снов" },
        { "Mystery Pryer", "Тайноведец" },
        { "Melee Scholar", "Учёный ближнего боя" },
        { "Warrior", "Воин" },
        { "Hunter", "Охотник" },
        { "Assassin", "Ассасин" },
        { "Witch", "Ведьма" },
        { "Monster", "Монстр" },
        { "Sailor", "Моряк" },
        { "Reader", "Чтец" },
        { "Secrets Suppliant", "Молящий о тайнах" },
        { "Corpse Collector", "Сборщик трупов" },
        { "Apothecary", "Аптекарь" },
        { "Arbiter", "Арбитр" },
        { "Lawyer", "Адвокат" },
        { "Prisoner", "Узник" },
        { "Villain", "Злодей" },
        { "Criminal", "Преступник" },
        { "Savant", "Эрудит" },
        { "Planter", "Плантатор" },

        // Организации и фракции
        { "Nighthawks", "Ночные Ястребы" },
        { "Nighthawk", "Ночной Ястреб" },
        { "Mandated Punishers", "Каратели" },
        { "Machinery Hivemind", "Разум Машин" },
        { "MI9", "МИ-9" },
        { "Aurora Order", "Орден Авроры" },
        { "Iron and Blood Cross Order", "Орден Железного и Кровавого Креста" },
        { "Secret Order", "Тайный Орден" },
        { "Element Dawn", "Рассвет Элементов" },
        { "Blackthorn Security Company", "Охранная компания «Чёрный Чертополох»" },
        { "Blackthorn", "Чёрный Чертополох" },
        { "Divination Club", "Гадательный клуб" },
        { "Shooting Club", "Стрелковый клуб" },
        { "Howes Circus", "Цирк Хауэса" },
        { "Requiem Poetry Society", "Поэтическое общество Заупокойной службы" },

        // Места
        { "Tingen", "Тинген" },
        { "Backlund", "Бэкланд" },
        { "Beckland", "Бэкланд" },
        { "Zouteland Street", "улица Зотланд" },
        { "Daffodil Street", "улица Нарциссов" },
        { "Iron Cross Street", "улица Железного Креста" },
        { "Loen Kingdom", "Королевство Лоэн" },
        { "Loen", "Лоэн" },
        { "Intis Republic", "Интисская Республика" },
        { "Intis", "Интис" },
        { "Feysac Empire", "Империя Фейсак" },
        { "Feysac", "Фейсак" },
        { "Feynapotter Kingdom", "Королевство Фейнапоттер" },
        { "Feynapotter", "Фейнапоттер" },
        { "Feynapot", "Фейнапоттер" },
        { "Tussock River", "Река Тасок" },
        { "Tussock", "Тасок" },
        { "Chanis Gate", "Врата Чаниса" },
        { "City of Silver", "Город Серебра" },
        { "Forsaken Land of the Gods", "Земля Покинутая Богами" },
        { "Bravehearts Bar", "бар «Храброе Сердце»" },
        { "Evil Dragon Bar", "бар «Злой Дракон»" },

        // Механики и системы
        { "Honorific Name", "Почётное имя" },
        { "Honorific Names", "Почётные имена" },
        { "Partner Synthesis", "Синтез спутников" },
        { "Partner Gacha", "Призыв спутника" },
        { "End Slide Rail", "Конечный рельс" },
        { "Destination Slide Rail", "Конечный рельс" },
        { "Slide Rail", "Направляющий рельс" },
        { "Puzzle Treasure Location", "Тайник головоломки" },
        { "TRPG-Order World", "TRPG — Мир Порядка" },
        { "Gold Pound", "Золотой фунт" },
        { "Gold Pounds", "Золотых фунтов" },
        { "Gold Pound Voucher", "Ваучер на золотые фунты" },
        { "10% Discount Coupon", "Купон на скидку 10%" },
        { "Bound Soles", "привязанных суле" },
        { "1000-Player Battlefield", "Поле боя на 1000 игроков" },
        { "1000-Player Stronghold Battle", "Битва за крепость на 1000 игроков" },
        { "Super Armor", "Суперброня" },
        { "Damage Reduction", "Снижение урона" },
        { "Cleanse", "Снятие контроля" },
        { "Cooldown Reduction", "Сокращение перезарядки" },
        { "Grievous Injury", "Тяжёлое ранение" },
        { "Burn", "Горение" },
        { "Execution", "Казнь" },
        { "Power of Dusk", "Сила Сумерек" },
        { "Greatsword Slash", "Рассечение двуручным мечом" },
        { "Torrent of Information", "Информационный поток" },
        { "Secret Peeking", "Тайное знание" },
        { "Mystery Peeking", "Взор тайновидца" },
        { "Guardian Statue", "Статуя хранителя" },
        { "Milgongen", "Милгонген" },
        { "Unshadowed", "Бестеневой" },
        { "Fighter", "Боец" }
    };

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        ServicePointManager.DefaultConnectionLimit = 50;
        ServicePointManager.Expect100Continue = false;

        int maxTarget = 7000;
        int custom;
        if (args.Length > 0 && int.TryParse(args[0], out custom))
        {
            maxTarget = custom;
        }

        Console.WriteLine(string.Format("=== Запуск перевода Пакета №17 (Лимит: {0} уникальных строк) ===", maxTarget));

        // 1. Загрузка существующего словаря
        var existingRu = new Dictionary<string, string>(StringComparer.Ordinal);
        var orderedKeys = new List<string>();

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
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string v = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";
                        if (!existingRu.ContainsKey(k))
                        {
                            existingRu[k] = v;
                            orderedKeys.Add(k);
                        }
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Текущий размер русского словаря: {0} записей", existingRu.Count));

        // 2. Загрузка дискового кэша переводов (если был прерван)
        var diskCache = new Dictionary<string, string>(StringComparer.Ordinal);
        if (File.Exists(CachePath))
        {
            foreach (var line in File.ReadAllLines(CachePath, Encoding.UTF8))
            {
                string[] parts = line.Split('\t');
                if (parts.Length >= 2)
                {
                    diskCache[parts[0]] = parts[1];
                }
            }
            Console.WriteLine(string.Format("Загружено из кэша: {0} ранее переведенных строк", diskCache.Count));
        }

        // 3. Сбор кандидатов для Пакета №17 (Боевые механики, UI, квесты, экспедиции, дневники, лор)
        var seen = new HashSet<string>(StringComparer.Ordinal);

        var listStoryAndLore = new List<Tuple<string, string>>();
        var listCombatAndSkills = new List<Tuple<string, string>>();
        var listUINoticesAndPrompts = new List<Tuple<string, string>>();
        var listGameplayAndWorld = new List<Tuple<string, string>>();
        var listOtherUI = new List<Tuple<string, string>>();

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

                        if (existingRu.ContainsKey(cnKey) || existingRu.ContainsKey(enVal) || seen.Contains(cnKey) || seen.Contains(enVal))
                            continue;

                        seen.Add(cnKey);
                        seen.Add(enVal);

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        // Исключаем внутренний дебаг и технический шум движка/сервера
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

                        // 1. Боевые навыки, способности, гиперссылки, статусы боя, подземелья
                        if (enVal.Contains("HyperLink") || enVal.Contains("damage") || enVal.Contains("Damage") ||
                            enVal.Contains("Cooldown") || enVal.Contains("cooldown") || enVal.Contains("Slash") ||
                            enVal.Contains("Dusk") || enVal.Contains("Bleed") || enVal.Contains("Crit") ||
                            enVal.Contains("AOE") || enVal.Contains("Mutated") || enVal.Contains("mutated") ||
                            cnKey.Contains("技能") || cnKey.Contains("伤害") || cnKey.Contains("冷却"))
                        {
                            listCombatAndSkills.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 2. Системные предупреждения, промпты, диалоговые окна, купоны
                        else if (enVal.Contains("Warning") || enVal.Contains("Confirm") || enVal.Contains("Cancel") ||
                                 enVal.Contains("Notice") || enVal.Contains("Prompt") || enVal.Contains("Tips") ||
                                 enVal.Contains("Unlock") || enVal.Contains("Success") || enVal.Contains("Failed") ||
                                 enVal.Contains("Discount") || enVal.Contains("Voucher") || enVal.Contains("Pound") ||
                                 enVal.Contains("Sole") || enVal.Contains("Battlefield") || enVal.Contains("Queue") ||
                                 enVal.Contains("Level") || enVal.Contains("Reward"))
                        {
                            listUINoticesAndPrompts.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 3. Геймплейные шаги квестов, перемещения, взаимодействия
                        else if (enVal.Contains("Pretend") || enVal.Contains("Teleport") || enVal.Contains("Move") ||
                                 enVal.Contains("Arrive") || enVal.Contains("Find") || enVal.Contains("Search") ||
                                 enVal.Contains("Talk") || enVal.Contains("Interact") || enVal.Contains("Inspect") ||
                                 enVal.Contains("Open") || enVal.Contains("Enter") || enVal.Contains("Leave"))
                        {
                            listGameplayAndWorld.Add(Tuple.Create(cnKey, enVal));
                        }
                        // 4. Сюжет, диалоги, мысли, дневники, персонажи
                        else if (enVal.Contains("<h>") || enVal.Contains("</>") || enVal.Contains("May ") || enVal.Contains("June ") ||
                            enVal.Contains("July ") || enVal.Contains("August ") || enVal.Contains("September ") || enVal.Contains("October ") ||
                            enVal.Contains("November ") || enVal.Contains("December ") || enVal.Contains("Neil") || enVal.Contains("Daly") ||
                            enVal.Contains("Edwin") || enVal.Contains("Goddess") || enVal.Contains("Church") || enVal.Contains("Beyonder") ||
                            enVal.Contains("Moretti") || enVal.Contains("Mr.") || enVal.Contains("Miss") ||
                            (enVal.Length > 30 && (enVal.Contains("\"") || enVal.Contains("...") || enVal.Contains("?") || enVal.Contains("!"))))
                        {
                            listStoryAndLore.Add(Tuple.Create(cnKey, enVal));
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

        var candidates = new List<Tuple<string, string>>();
        candidates.AddRange(listCombatAndSkills);
        candidates.AddRange(listUINoticesAndPrompts);
        candidates.AddRange(listGameplayAndWorld);
        candidates.AddRange(listStoryAndLore);
        if (candidates.Count < maxTarget)
        {
            int diff = maxTarget - candidates.Count;
            candidates.AddRange(listOtherUI.GetRange(0, Math.Min(diff, listOtherUI.Count)));
        }

        if (candidates.Count > maxTarget)
        {
            candidates = candidates.GetRange(0, maxTarget);
        }

        Console.WriteLine(string.Format("Отобрано для Пакета №17: {0} уникальных строк", candidates.Count));

        // 4. Параллельный перевод с защитой разметки и кэшированием
        var translatedResults = new ConcurrentDictionary<string, string>();
        int processed = 0;
        int successes = 0;

        Parallel.ForEach(candidates, new ParallelOptions { MaxDegreeOfParallelism = 16 }, item =>
        {
            string cn = item.Item1;
            string en = item.Item2;

            string ru = null;
            if (diskCache.TryGetValue(en, out ru) && !string.IsNullOrWhiteSpace(ru))
            {
                // Из кэша
            }
            else
            {
                ru = TranslateSingleWithRetry(en);
                if (!string.IsNullOrWhiteSpace(ru))
                {
                    ru = ApplyCanonGlossary(ru);
                    ru = PostProcessText(ru);

                    lock (cacheLock)
                    {
                        try
                        {
                            File.AppendAllText(CachePath, en + "\t" + ru + "\n", Encoding.UTF8);
                        }
                        catch { }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(ru))
            {
                string escapedRu = CleanForLua(ru);
                string escapedCn = CleanForLua(cn);
                string escapedEn = CleanForLua(en);

                translatedResults[escapedCn] = escapedRu;
                translatedResults[escapedEn] = escapedRu;
                Interlocked.Increment(ref successes);
            }

            int p = Interlocked.Increment(ref processed);
            if (p % 100 == 0 || p == candidates.Count)
            {
                Console.Write(string.Format("\rПрогресс перевода: {0} / {1} (Успешно: {2})", p, candidates.Count, successes));
            }
            Thread.Sleep(10);
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

        // 6. Безопасное сохранение в RuntimeTextRussian.lua через атомарный временный файл
        Console.WriteLine(string.Format("Сохранение в {0} (всего {1} записей)...", RuPath, existingRu.Count));
        SaveLuaDictionary(RuPath, orderedKeys, existingRu);

        // 7. Синхронизация с data/RuntimeTextRussian.lua и клиентом игры
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
            if (Directory.Exists(Path.GetDirectoryName(GameTestPath)))
            {
                File.Copy(RuPath, GameTestPath, true);
                Console.WriteLine("✅ Синхронизировано с клиентом игры: " + GameTestPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Клиент игры не обновлен: " + ex.Message);
        }

        // Удаление временного кэша при успешном завершении
        try
        {
            if (File.Exists(CachePath))
            {
                File.Delete(CachePath);
            }
        }
        catch { }

        Console.WriteLine(string.Format("Пакет №17 успешно завершен! Новый размер словаря: {0} записей.", existingRu.Count));
    }

    static string ApplyCanonGlossary(string text)
    {
        string res = text;
        foreach (var pair in CanonExact)
        {
            res = Regex.Replace(res, @"\b" + Regex.Escape(pair.Key) + @"\b", pair.Value, RegexOptions.IgnoreCase);
        }

        // Канонизация и устранение машинных форм слова "Дурак" в отношении Шута
        res = Regex.Replace(res, @"\b(мистера|господина)\s+(Дурака|дурака)\b", "мистера Шута", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(мистеру|господину)\s+(Дураку|дураку)\b", "мистеру Шуту", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(мистером|господином)\s+(Дураком|дураком)\b", "мистером Шутом", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(мистере|господине)\s+(Дураке|дураке)\b", "мистере Шуте", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(мистер|господин)\s+(Дурак|дурак)\b", "Мистер Шут", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Церковь|Церкви|Церковью|Церквей)\s+(Дурака|дурака)\b", "$1 Шута", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(молитва|молитвы|молитве|молитвой|молитву)\s+(Дурака|дурака)\b", "$1 Шуту", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(вопрос|вопроса|вопросу)\s+(Дураку|дураку)\b", "$1 Шуту", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(путь|пути|путем)\s+(Дурака|дурака)\b", "$1 Шута", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(последовательность|последовательности)\s+(Дурака|дурака)\b", "$1 Шута", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(вера|веры|вере|веру)\s+в\s+(Дурака|дурака)\b", "$1 в Шута", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\bДурака\b", "Шута");
        res = Regex.Replace(res, @"\bДураку\b", "Шуту");
        res = Regex.Replace(res, @"\bДураком\b", "Шутом");
        res = Regex.Replace(res, @"\bДураке\b", "Шуте");
        res = Regex.Replace(res, @"\bДурак\b", "Шут");

        // Канонизация частых терминов
        res = Regex.Replace(res, @"\b(Потусторонняя характеристика|потусторонняя характеристика|потусторонней характеристики|потустороннюю характеристику|потусторонней характеристике)\b", "Потустороннее свойство", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Потусторонние характеристики|потусторонние характеристики|потусторонних характеристик)\b", "Потусторонние свойства", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Повелитель тайн|повелитель тайн)\b", "Повелитель Тайн", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Черный чертополох|Черный Терновник|Черный терновник|Блэкторн|Компания Терновник|компания Терновник)\b", "Чёрный Чертополох", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Старый Нил|старый Нил|старику Нилу|старика Нила)\b", "Старина Нил", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Кляйн|Кляйна|Кляйну|Кляйне|Кляйном)\b", "Клейн", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Антигонуса|Антигонус)\b", "Антигона", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Райел Бибер|Рейл Бибер|Риэль Бибер)\b", "Райэль Бибер", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(золотой фунт|золотых фунтов|золотых фунта)\b", "золотых фунтов", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(1 золотых фунтов)\b", "1 золотой фунт", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(пенсов|пенни|пенса)\b", "пенсов", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(1 пенсов)\b", "1 пенс", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(солей|соля|соли)\b", "суле", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(1 суле)\b", "1 суле", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(переваривание зелья|переваривание)\b", "усвоение зелья", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(метод действия)\b", "Метод Лицедейства", RegexOptions.IgnoreCase);

        // Боевые характеристики и термины
        res = Regex.Replace(res, @"\b(двойной атаки|Двойной атаки|двойную атаку|Двойную атаку)\b", "Двойная атака", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(скорости атаки|Скорости атаки)\b", "Скорость атаки", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(скорости бега|Скорости бега|скорость перемещения)\b", "Скорость бега", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(снижения урона|Снижения урона)\b", "Снижение урона", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(восстановления маны|Восстановления маны)\b", "Восстановление маны", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(тяжелое ранение|тяжёлого ранения|Тяжелое ранение)\b", "Тяжёлое ранение", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(суммируется (\d+) слоев|суммируется (\d+) слоев|может накапливаться (\d+) слоев)\b", "суммируется до $1 ур.", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(HP|Hp|hp)\b", "ОЗ");

        return res;
    }

    static string PostProcessText(string text)
    {
        string res = text;
        res = Regex.Replace(res, @"[a-f0-9]{32}", "");
        res = res.Replace(" ,", ",");
        res = res.Replace(" .", ".");
        res = res.Replace(" !", "!");
        res = res.Replace(" ?", "?");
        res = res.Replace(" :", ":");
        res = res.Replace(" ;", ";");
        res = res.Replace("( ", "(");
        res = res.Replace(" )", ")");
        res = res.Replace("« ", "«");
        res = res.Replace(" »", "»");
        res = res.Replace("“ ", "“");
        res = res.Replace(" ”", "”");
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
        string tempPath = filePath + ".tmp";
        using (var sw = new StreamWriter(tempPath, false, new UTF8Encoding(false)))
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

        for (int retry = 0; retry < 10; retry++)
        {
            try
            {
                File.Copy(tempPath, filePath, true);
                File.Delete(tempPath);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Предупреждение: файл занят, повтор сохранения ({0}/10): {1}", retry + 1, ex.Message));
                Thread.Sleep(1000);
            }
        }
    }

    static string TranslateSingleWithRetry(string text)
    {
        for (int retry = 0; retry < 3; retry++)
        {
            string res = TranslateSingle(text, retry);
            if (!string.IsNullOrWhiteSpace(res)) return res;
            Thread.Sleep(200 * (retry + 1));
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
                result = Regex.Replace(result, @"[a-f0-9]{32}", "");
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
