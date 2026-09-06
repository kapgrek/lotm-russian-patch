using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslateBatch25Final
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string DataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
    static string GameTestPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";
    static string CachePath = @"d:\gameDev\translate lotm\tools\batch25_cache.tsv";
    static object cacheLock = new object();

    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
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
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "Klein Moretti", "Клейн Моретти" },
        { "Klein", "Клейн" },
        { "Captain Dunn", "капитан Данн" },
        { "Dunn Smith", "Данн Смит" },
        { "Old Neil", "Старина Нил" },
        { "Leonard Mitchell", "Леонард Митчелл" },
        { "Leonard", "Леонард" },
        { "Captain Frye", "капитан Фрай" },
        { "Frye", "Фрай" },
        { "Daly Simone", "Дейли Симон" },
        { "Audrey Hall", "Одри Холл" },
        { "Audrey", "Одри" },
        { "Alger Wilson", "Алджер Уилсон" },
        { "Alger", "Алджер" },
        { "Derrick Berg", "Деррик Берг" },
        { "Derrick", "Деррик" },
        { "Roselle Gustav", "Розель Густав" },
        { "Emperor Roselle", "Император Розель" },
        { "Roselle", "Розель" },
        { "Trissy", "Трис" },
        { "Rylbir", "Райэль Бибер" },
        { "Ruerbibo", "Райэль Бибер" },
        { "Tingen", "Тинген" },
        { "Backlund", "Бэкланд" },
        { "Beckland", "Бэкланд" },
        { "Desi Bay", "Залив Деси" },
        { "Desi", "Деси" },
        { "Dixi", "Деси" },
        { "Hornacis", "Хорнакис" },
        { "Bravehearts Bar", "бар «Храброе Сердце»" },
        { "Dragon Bar", "бар «Злой Дракон»" },
        { "Demon Bar", "бар «Демон»" },
        { "Iron Dragon Bar", "бар «Железный Дракон»" },
        { "Golden Wolf", "Золотой Волк" },
        { "Golden Wolf Cathedral", "Церковь Золотого Волка" },
        { "Golden Wolf Church", "Церковь Золотого Волка" },
        { "Rose School of Thought", "Школа Розы" },
        { "Rose School", "Школа Розы" },
        { "Beyonder", "Потусторонний" },
        { "Beyonders", "Потусторонние" },
        { "Beyonder Characteristic", "Потустороннее свойство" },
        { "Gold Pound", "Золотой фунт" },
        { "Gold Pounds", "Золотых фунтов" },
        { "Sule", "суле" },
        { "Pence", "пенсов" }
    };

    static string TranslateLoreOrKnown(string cn, string en)
    {
        // 1. Дневник гадателя
        if (en.Contains("Divining for others is really profitable"))
        {
            return "Гадать другим действительно очень прибыльно — большинству людей просто нужен кто-то, чтобы придать им храбрости. \\n\\n --------------------------------------------- \\n\\n Сегодня гадал человеку, результат четко указывал на это, но оказался неверным. Как такое возможно? В гадании не бывает ошибок, возможно, я неправильно истолковал знаки. \\n\\n --------------------------------------------- \\n\\n (Средние страницы залиты кровью, разобрать текст почти невозможно) \\n\\n --------------------------------------------- \\n\\n Я видел слишком много людей, которые принимали откровения за предначертанную судьбу, а затем погибали.";
        }

        // 2. Системные и UI сообщения подбора матчей и ИИ
        if (en.StartsWith("Your team's average Beyonder rating does not meet the recommended rating"))
        {
            return "Средний рейтинг Потустороннего вашей группы не достиг рекомендуемого. В данный момент подбор невозможен. Рекомендуется сражаться против ИИ или искать матч в одиночку.";
        }
        if (en.StartsWith("Your team's average Beyonder rating does not meet the recommended rating. Current matchmaking has been automatically canceled"))
        {
            return "Средний рейтинг Потустороннего вашей группы не достиг рекомендуемого. Текущий подбор автоматически отменен. Рекомендуется сражаться против ИИ или искать матч в одиночку.";
        }
        if (en.StartsWith("A member of your team does not meet the level requirements for this dungeon"))
        {
            return "Один из участников вашей группы не соответствует требованиям по уровню для этого подземелья. Текущий подбор автоматически отменен. Рекомендуется сражаться против ИИ или искать матч в одиночку.";
        }
        if (en == "Combat Attributes") return "Боевые характеристики";
        if (en == "Combat Attribute Bonus") return "Бонус боевых характеристик";
        if (en == "AI Mode") return "Режим ИИ";
        if (en == "AI Dance Battle Mode") return "Битва танцев: режим ИИ";
        if (en == "Dungeon · AI Mode") return "Подземелье · Режим ИИ";
        if (en == "Too many AI dungeons currently active, automatically switched to party mode.")
            return "В данный момент активно слишком много подземелий с ИИ, автоматически переключено в режим группы.";
        if (en == "Too many AI dungeons currently active, please try again later.")
            return "В данный момент активно слишком много подземелий с ИИ, пожалуйста, повторите попытку позже.";
        if (en == "Cannot use AI motion capture actions in the current scene.")
            return "В текущей сцене невозможно использовать захват движений ИИ.";
        if (en == "Enable AI Face Sculpting") return "Включить создание лица с помощью ИИ";
        if (en == "Upload the prepared video to the Art Academy; the AI will analyze and review the content.")
            return "Загрузите подготовленное видео в Академию Искусств; алгоритм проанализирует и проверит содержимое.";
        if (en == "Stop AI for All Bots") return "Остановить ИИ для всех ботов";
        if (en == "Resume AI for All Bots") return "Возобновить ИИ для всех ботов";
        if (en == "Enable AI Chat?") return "Включить чат с ИИ?";
        if (en == "Enable AI action video generation") return "Включить генерацию видео действий с ИИ";
        if (en == "Whether to enable AI action video playback") return "Включить воспроизведение видео действий с ИИ";
        if (en == "Whether to enable AI chat sensitive word check") return "Включить проверку запрещенных слов в чате с ИИ";
        if (en == "Enable AI Video Queuing Function?") return "Включить функцию очереди видео с ИИ?";
        if (en.StartsWith("This dungeon supports a maximum of %d players in AI mode"))
            return "Это подземелье в режиме ИИ поддерживает не более %d игроков в группе";
        if (en == "Resume AI for all bots in this scene") return "Возобновить ИИ для всех ботов в этой сцене";
        if (en == "Pause AI for all bots in this scene") return "Приостановить ИИ для всех ботов в этой сцене";
        if (en == "Simple AI Test Boss") return "Простой тестовый босс ИИ";
        if (en == "Simple AI Test Minion 2") return "Простой тестовый миньон ИИ 2";
        if (en == "For AI Positioning Mechanism Solving") return "Для решения механики позиционирования ИИ";
        if (en == "Set Free AI Video Generation Count") return "Задать число бесплатных генераций видео с ИИ";
        if (en == "Idle Animation to enter after special animation ends") return "Анимация покоя после завершения особого действия";
        if (en == "Chat AI Check Function") return "Функция проверки чата через ИИ";
        if (en == "Video creation AI play animation") return "Воспроизведение анимации создания видео ИИ";
        if (en == "Select file AI play animation") return "Выбор файла для анимации воспроизведения ИИ";

        // 3. Короткие системные переключатели
        if (en == "0: Disable, 1: Enable.") return "0: Выключить, 1: Включить";
        if (en == "0: Hide, 1: Enable.") return "0: Скрыть, 1: Включить";
        if (en == "0: Override \\n 1: Additive" || en == "0: Override \n 1: Additive") return "0: Перезапись \\n 1: Сложение";
        if (en == "1: Enable, 0: Disable.") return "1: Включить, 0: Выключить";
        if (en == "1: Enable 0: Disable") return "1: Включить, 0: Выключить";
        if (en == "1-Enable, other values-Disable") return "1 — Включить, другие значения — Выключить";
        if (en == "1-Correct") return "1 — Верно";
        if (en == "3-Correct") return "3 — Верно";
        if (en == "1: Start, 0: End.") return "1: Старт, 0: Конец";
        if (en == "1: Disable, 0: Enable.") return "1: Отключить, 0: Включить";
        if (en == "1") return "1";
        if (en == "3") return "3";
        if (en == "4") return "4";
        if (en == "5") return "5";
        if (en == ".p4config and ") return ".p4config и ";
        if (en.StartsWith("-----------------------------End Output")) return "-----------------------------Конец вывода-----------------------------";

        // 4. Подземелья и измерения с префиксами чисел
        var matchInst = Regex.Match(en, @"^(\d+)\s+(.+?)\s+(Instance|Dungeon|Open World|Plane|Copy|Solo Instance|Single-player Instance|Single-Player Instance|Multiplayer Scene)$");
        if (matchInst.Success)
        {
            string id = matchInst.Groups[1].Value;
            string loc = matchInst.Groups[2].Value.Trim();
            string type = matchInst.Groups[3].Value;

            string ruType = "Подземелье";
            if (type == "Open World") ruType = "Открытый мир";
            else if (type == "Plane") ruType = "Измерение";
            else if (type == "Copy") ruType = "Копия";
            else if (type.Contains("Solo") || type.Contains("Single")) ruType = "Одиночное измерение";
            else if (type.Contains("Multiplayer")) ruType = "Многопользовательская сцена";

            string ruLoc = loc;
            foreach (var kvp in CanonExact)
            {
                ruLoc = Regex.Replace(ruLoc, @"\b" + Regex.Escape(kvp.Key) + @"\b", kvp.Value, RegexOptions.IgnoreCase);
            }
            ruLoc = ruLoc.Replace("Lake", "Озеро").Replace("Factory", "Фабрика").Replace("Docks", "Причалы")
                         .Replace("Wharf", "Причал").Replace("Pier", "Пирс").Replace("Sewers", "Канализация")
                         .Replace("Sewer", "Канализация").Replace("Village", "Деревня").Replace("Manor", "Поместье")
                         .Replace("Dream", "Сон").Replace("Joker", "Клоун").Replace("Clown", "Клоун")
                         .Replace("Snow Mountain", "Снежная гора").Replace("Snowfield", "Снежные поля")
                         .Replace("Tournament", "Турнир").Replace("Apple Knight", "Яблочный рыцарь")
                         .Replace("Banquet Hall", "Банкетный зал").Replace("Fate Cafe", "Кафе Судьбы")
                         .Replace("Destiny Cafe", "Кафе Судьбы").Replace("Shadow Cult", "Теневой культ")
                         .Replace("Eroded Corner", "Изъеденный угол").Replace("Mountain Path", "Горная тропа")
                         .Replace("Ruined Village", "Разрушенная деревня").Replace("Gloomy Manor", "Мрачное поместье")
                         .Replace("Church Fields", "Церковные поля").Replace("Frozen Ruins", "Ледяные руины")
                         .Replace("Penal Factory", "Фабрика наказаний").Replace("Ferlanqi Flats", "Квартира Франки")
                         .Replace("Flats", "Квартиры").Replace("Cafe", "Кафе").Replace("Warehouse", "Склад")
                         .Replace("Welch's Home", "Дом Уэлча").Replace("Welch's House", "Дом Уэлча")
                         .Replace("Old House", "Старый дом").Replace("Raphael Cemetery", "Кладбище Рафаэля")
                         .Replace("Suburban Villa", "Загородная вилла").Replace("Outskirts", "Окраины")
                         .Replace("Suburbs", "Пригород").Replace("Room of Memories", "Зал Воспоминаний")
                         .Replace("Chamber of Memories", "Зал Воспоминаний").Replace("Loop Space", "Зацикленное пространство")
                         .Replace("Hidden Room", "Тайная комната").Replace("Secret Space", "Тайное пространство")
                         .Replace("Theater Office", "Офис театра").Replace("Theatre Office", "Офис театра")
                         .Replace("Illusion", "Иллюзия").Replace("Irene's Home", "Дом Ирины")
                         .Replace("Irene's House", "Дом Ирины").Replace("QA Test Village", "Тестовая деревня QA")
                         .Replace("Kitchen", "Кухня").Replace("Joyce's Dream", "Сон Джойса")
                         .Replace("Consciousness", "Сознание").Replace("Lamud Town", "Городок Ламуд")
                         .Replace("Apartment", "Квартира").Replace("Blackboard Space", "Пространство доски")
                         .Replace("Generic Sewer", "Городская канализация").Replace("Bicycle Chase", "Погоня на велосипеде")
                         .Replace("Artisan's Hut", "Хижина ремесленника").Replace("Roadside", "Обочина дороги")
                         .Replace("City Wonder", "Чудо города").Replace("Grey Mist Space", "Пространство Серого Тумана")
                         .Replace("Memory Space", "Пространство памяти").Replace("Selina's Home", "Дом Селены")
                         .Replace("Hainas's Dream", "Сон Хайнаса").Replace("Torrent of Knowledge", "Поток Знаний")
                         .Replace("Bard Performance", "Выступление барда").Replace("Text Game", "Текстовая игра")
                         .Replace("Divination Space", "Пространство гадания").Replace("Grand Theatre", "Большой театр")
                         .Replace("Safe House", "Убежище").Replace("Machine Room", "Машинный зал")
                         .Replace("Tea Party Venue", "Зал чаепития").Replace("Inner Chamber of the Venue", "Внутренний зал")
                         .Replace("Dragon Hunter City", "Город Охотников на Драконов").Replace("City of Dragon Hunters", "Город Охотников на Драконов")
                         .Replace("Forest Cabin", "Лесная хижина").Replace("Golden Autumn Lake", "Озеро Золотой Осени")
                         .Replace("Sinful Tingen", "Грешный Тинген").Replace("Corner of Spring", "Весенний уголок")
                         .Replace("A Corner of Spring", "Весенний уголок").Replace("Clinic", "Клиника")
                         .Replace("Tingen in the Mirror", "Тинген в зеркале").Replace("Gate of the Underworld", "Врата Подземного Мира")
                         .Replace("Charity Hospital", "Благотворительная больница").Replace("Ripper Theatre", "Театр Потрошителя")
                         .Replace("Corner of an Alley", "Уголок переулка").Replace("Old Street Cottage", "Домик на Старой улице")
                         .Replace("Lake of Chaos and Order", "Озеро Хаоса и Порядка").Replace("Ballroom", "Бальный зал")
                         .Replace("Ancestral Home", "Родовое поместье").Replace("Water Lily Town", "Городок Кувшинок")
                         .Replace("Sea of Flowers No. 2", "Море цветов №2").Replace("Sea of Flowers", "Море цветов")
                         .Replace("Ayla's Residence", "Обитель Айлы").Replace("Remnants of the Divine War", "Руины Войны Богов")
                         .Replace("Explore Puzzle Space", "Исследование пространства загадок").Replace("Golden Indus Theater", "Театр Золотого Платана")
                         .Replace("Library", "Библиотека").Replace("Suburban Wilderness", "Загородная глушь")
                         .Replace("University Town", "Университетский городок").Replace("Witch Promotion", "Возвышение Ведьмы")
                         .Replace("Rose Paradise", "Рай Роз").Replace("Book of Disorder", "Книга Беспорядка")
                         .Replace("Unknown Space", "Неведомое пространство").Replace("Flo's Home", "Дом Фло")
                         .Replace("Mrs. Miriam's Home", "Дом госпожи Мириам");

            return string.Format("{0} {1}: {2}", id, ruType, ruLoc.Trim(' ', '-'));
        }

        return null;
    }

    static string ApplyCanon(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        foreach (var pair in CanonExact)
        {
            text = Regex.Replace(text, @"\b" + Regex.Escape(pair.Key) + @"\b", pair.Value, RegexOptions.IgnoreCase);
        }
        return text;
    }

    static string TranslateGoogle(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        var tagMap = new Dictionary<string, string>();
        int tagIdx = 0;
        string protectedText = Regex.Replace(text, @"<[^>]+>|\{[^{}]+\}|\\n|\*d\*\*|\*d|\*f\*\*|\*f|mul\([^)]+\)|spellfielddisc\([^)]+\)|buffdisc\([^)]+\)|bulletdisc\([^)]+\)|buffappear\([^)]+\)|CheckStar\([^)]+\)|%s|%d|%i|%f", m =>
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
        string r = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "");
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        ServicePointManager.DefaultConnectionLimit = 64;
        ServicePointManager.Expect100Continue = false;

        Console.WriteLine("=====================================================");
        Console.WriteLine("  Lord of Mysteries - Final Batch #25 (100% Final)   ");
        Console.WriteLine("=====================================================");

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
        Console.WriteLine("Существующих записей в словаре: " + existingRu.Count);

        // 2. Сбор абсолютно ВСЕХ оставшихся непереведенных строк
        var untranslated = new List<Tuple<string, string>>();
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

                        if (!existingRu.ContainsKey(cnKey) && !existingRu.ContainsKey(enVal))
                        {
                            untranslated.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine("Осталось перевести непереведенных строк: " + untranslated.Count);
        if (untranslated.Count == 0)
        {
            Console.WriteLine("Все строки уже переведены на 100%!");
            return;
        }

        // 3. Перевод всех строк
        var translatedResults = new ConcurrentDictionary<string, string>();
        int processed = 0;

        Parallel.ForEach(untranslated, new ParallelOptions { MaxDegreeOfParallelism = 16 }, item =>
        {
            string cn = item.Item1;
            string en = item.Item2;

            string ru = TranslateLoreOrKnown(cn, en);
            if (string.IsNullOrWhiteSpace(ru))
            {
                ru = TranslateGoogle(en);
                ru = ApplyCanon(ru);
            }

            if (!string.IsNullOrWhiteSpace(ru))
            {
                translatedResults[cn] = ru;
                translatedResults[en] = ru;
            }

            int p = Interlocked.Increment(ref processed);
            if (p % 20 == 0 || p == untranslated.Count)
            {
                Console.Write("\rПрогресс перевода: " + p + " / " + untranslated.Count);
            }
        });

        Console.WriteLine("\nПеревод завершен! Получено ключей: " + translatedResults.Count);

        // 4. Слияние со словарем
        foreach (var kvp in translatedResults)
        {
            if (!existingRu.ContainsKey(kvp.Key))
            {
                orderedKeys.Add(kvp.Key);
            }
            existingRu[kvp.Key] = kvp.Value;
        }

        // 5. Сохранение файла словаря
        using (var sw = new StreamWriter(RuPath, false, new UTF8Encoding(false)))
        {
            sw.WriteLine("-- Generated Russian Translation Dictionary for Lord of Mysteries");
            sw.WriteLine("-- Entries: " + existingRu.Count);
            sw.WriteLine("return {");
            foreach (var k in orderedKeys)
            {
                string v = existingRu[k];
                sw.WriteLine("    [\"" + CleanLua(k) + "\"] = \"" + CleanLua(v) + "\",");
            }
            sw.WriteLine("}");
        }
        Console.WriteLine("Словарь записан: " + RuPath + " (" + existingRu.Count + " записей)");

        // 6. Синхронизация с data и игрой
        File.Copy(RuPath, DataRuPath, true);
        Console.WriteLine("Скопировано в " + DataRuPath);
        if (File.Exists(GameTestPath) || Directory.Exists(Path.GetDirectoryName(GameTestPath)))
        {
            try
            {
                File.Copy(RuPath, GameTestPath, true);
                Console.WriteLine("Скопировано в " + GameTestPath);
            }
            catch { }
        }
    }
}
