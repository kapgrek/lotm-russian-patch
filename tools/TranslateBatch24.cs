using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslateBatch24
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string DataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
    static string GameTestPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";
    static string CachePath = @"d:\gameDev\translate lotm\tools\batch24_cache.tsv";
    static object cacheLock = new object();

    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Боги и высшие сущности
        { "The Fool", "Шут" },
        { "Lord of Mysteries", "Повелитель Тайн" },
        { "Lord of the Mysteries", "Повелитель Тайн" },
        { "Great Mother", "Великая Мать" },
        { "The Great Mother", "Великая Мать" },
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
        { "Captain Dunn", "капитан Данн" },
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
        { "Trissy", "Трис" },
        { "Madame Sharon", "Мадам Шарон" },
        { "Divina", "Дивина" },
        { "Leo", "Лео" },
        { "Golden Wolf", "Золотой Волк" },

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
        { "Mandated Punishers", "Уполномоченные Каратели" },
        { "Machinery Hivemind", "Механический Разум" },
        { "MI9", "МИ-9" },
        { "Aurora Order", "Орден Авроры" },
        { "Iron and Blood Cross Order", "Орден Железного и Кровавого Креста" },
        { "Secret Order", "Тайный Орден" },
        { "Rose School of Thought", "Школа Мысли Розы" },
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
        { "Desi Bay", "Залив Деси" },
        { "Dixi", "Деси" },
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
        { "Battle Pass", "Боевой пропуск" },
        { "Auto Chess", "Автошахматы" },
        { "Auto-Chess", "Автошахматы" },
        { "Spirit Body Threads", "Нити духовного тела" },
        { "Spirit Line", "Духовная нить" },
        { "Astral Body", "Астральное тело" },
        { "Astral Projection", "Астральная проекция" },
        { "Spirit World", "Мир Духов" },
        { "Acting Method", "Метод Лицедейства" },
        { "Historical Projection", "Историческая проекция" },
        { "Paper Figurine Substitute", "Замена бумажным человечком" },
        { "Flame Controlling", "Управление пламенем" },
        { "Damage Reduction", "Снижение урона" },
        { "Super Armor", "Суперброня" },
        { "Cleanse", "Снятие контроля" },
        { "Cooldown Reduction", "Сокращение перезарядки" },
        { "Mind Fire", "Пламя разума" },
        { "Induction Mark", "Метка внушения" },
        { "Hypnosis", "Гипноз" },
        { "Imprisonment", "Заточение" },
        { "Grievous Injury", "Тяжёлое ранение" },
        { "Burn", "Горение" },
        { "Execution", "Казнь" },
        { "Gold Pound", "Золотой фунт" },
        { "Gold Pounds", "Золотых фунтов" },
        { "Gold Linals", "золотых линалов" },
        { "Linals", "линалов" },
        { "Wall of Spirituality", "Стена духовности" },
        { "Spirituality Wall", "Стена духовности" },
        { "Curly-Haired Baboon", "Курчавошерстный бабуин" },
        { "Curly-Haired Baboons", "Курчавошерстные бабуины" },
        { "Baboon", "Бабуин" },
        { "Baboons", "Бабуины" },
        { "Pressure Plate", "Нажимная плита" },
        { "Safe Zone", "Безопасная зона" },
        { "Minion Dummy", "Манекен миньона" },
        { "Memory Scene", "Сцена воспоминаний" },
        { "Mist Line", "Линия Тумана" },
        { "Keybinds", "Клавиши управления" },
        { "Collectibles", "Коллекции" },
        { "Signpost", "Указатель" },
        { "Floor Tile", "Напольная плитка" },
        { "Ground Tile", "Плитка пола" },
        { "Wisteria", "Глициния" },
        { "Chainmail", "Кольчуга" },
        { "Devourer", "Пожиратель" },
        { "Hunger Devour", "Голодное пожирание" }
    };

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
            enVal.StartsWith("===== ") || enVal.Contains("PlatformScalabilitySettings") ||
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

        // 6. Любой связный английский текст с пробелами
        if (enVal.Contains(" "))
        {
            score += 20;
        }

        // Бонус за нормальную длину текста
        if (enVal.Length >= 8 && enVal.Length <= 120 && !enVal.Contains("_"))
        {
            score += 15;
        }

        // Штраф за технические префиксы
        if (enVal.StartsWith("00_") || enVal.StartsWith("01_") || enVal.Contains("Test Scene"))
        {
            score -= 50;
        }

        if (enVal.Length < 3)
        {
            score -= 40;
        }

        return score;
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        ServicePointManager.DefaultConnectionLimit = 64;
        ServicePointManager.Expect100Continue = false;

        int maxTarget = 15000;
        int custom;
        if (args.Length > 0 && int.TryParse(args[0], out custom))
        {
            maxTarget = custom;
        }

        Console.WriteLine(string.Format("=== Запуск перевода Пакета №24 (Лимит: {0} уникальных строк) ===", maxTarget));

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

        // 3. Сбор кандидатов для Пакета №24
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var scoredList = new List<Tuple<int, string, string>>();

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

                        if (IsTechnicalCodeOrDebug(enVal, cnKey))
                            continue;

                        int score = GetStringPriorityScore(enVal, cnKey);
                        scoredList.Add(Tuple.Create(score, cnKey, enVal));
                    }
                }
            }
        }

        scoredList.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        var candidates = new List<Tuple<string, string>>();
        for (int i = 0; i < Math.Min(maxTarget, scoredList.Count); i++)
        {
            candidates.Add(Tuple.Create(scoredList[i].Item2, scoredList[i].Item3));
        }

        Console.WriteLine(string.Format("Отобрано для Пакета №24: {0} уникальных строк (из {1} кандидатов)", candidates.Count, scoredList.Count));

        // 4. Параллельный перевод с защитой разметки и кэшированием
        var translatedResults = new ConcurrentDictionary<string, string>();
        int processed = 0;
        int successes = 0;

        Parallel.ForEach(candidates, new ParallelOptions { MaxDegreeOfParallelism = 24 }, item =>
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
            Thread.Sleep(5);
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

        Console.WriteLine(string.Format("Пакет №24 успешно завершен! Новый размер словаря: {0} записей.", existingRu.Count));
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
        res = Regex.Replace(res, @"\b(ворот Чанис|ворота Чанис|врат Чанис|врата Чанис)\b", "Врат Чаниса", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Chanis Gate|Чанис Гейт)\b", "Врата Чаниса", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Стена Духовности)\b", "Стена духовности");
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
            sw.WriteLine("-- Lord of the Mysteries Russian Localization Runtime Dictionary");
            sw.WriteLine("-- Generated automatically with Canon Glossary preservation");
            sw.WriteLine("return {");

            foreach (var k in orderedKeys)
            {
                string val;
                if (dict.TryGetValue(k, out val))
                {
                    sw.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", k, val));
                }
            }

            sw.WriteLine("}");
        }

        for (int retry = 0; retry < 10; retry++)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(tempPath, filePath);
                break;
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
