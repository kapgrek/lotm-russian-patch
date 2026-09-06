using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslateBatch14
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
    static string DataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
    static string LsiDir = @"d:\gameDev\translate lotm\mod_base\Saved\Mods\lua\mods\cpdd_runtime_fixes";

    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Высшие сущности и боги
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
        { "Tarot Club", "Клуб Таро" },
        { "Angel of Fate", "Ангел Судьбы" },
        { "King of Angels", "Король Ангелов" },
        { "Kings of Angels", "Короли Ангелов" },

        // Персонажи и личности
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
        { "Miss Judgment", "Мисс Судья" },
        { "Mr. Moon", "Мистер Луна" },
        { "Mr. Sun", "Мистер Солнце" },
        { "Mr. Star", "Мистер Звезда" },
        { "Ma'am Hermit", "Мадам Отшельник" },
        { "Antigonus Family", "Семья Антигона" },
        { "Antigonus", "Антигон" },
        { "Riel Bieber", "Райэль Бибер" },
        { "Ray Bieber", "Райэль Бибер" },
        { "Welch McGovern", "Уэлч Макговерн" },
        { "Welch", "Уэлч" },
        { "Naya", "Ная" },
        { "Captain", "Капитан" },
        { "Mrs. Cesir", "миссис Сесир" },
        { "Cesir", "Сесир" },
        { "Father Utravsky", "отец Утравски" },
        { "Utravsky", "Утравски" },
        { "Doctor Aaron", "доктор Аарон" },
        { "Aaron Ceres", "Аарон Церес" },
        { "Will Auceptin", "Уилл Оцептин" },
        { "Will Ceres", "Уилл Церес" },
        { "Snake of Mercury", "Змей Ртути" },
        { "Snake of Fate", "Змей Судьбы" },
        { "Lamud", "Ламуд" },
        { "Dream Catcher", "Ловец снов" },
        { "Arthur", "Артур" },
        { "Cook", "Кук" },
        { "Thomas", "Томас" },
        { "Linda", "Линда" },
        { "Jack", "Джек" },
        { "Madam Haley", "мадам Хейли" },
        { "Madam Hali", "мадам Хейли" },
        { "Haley", "Хейли" },
        { "Hali", "Хейли" },
        { "Xiga", "Сига" },
        { "Collette", "Колетт" },
        { "Brown Bear", "Бурый Медведь" },
        { "Edmond", "Эдмонд" },
        { "Aiden", "Эйден" },
        { "Eric", "Эрик" },
        { "Ms. Fula", "мисс Фула" },
        { "Fula", "Фула" },

        // Пути и Последовательности
        { "The Fool Pathway", "Путь Шута" },
        { "Fool Pathway", "Путь Шута" },
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
        { "Hypnotist", "Гипнотизёр" },
        { "Manipulator", "Манипулятор" },
        { "Sleepless", "Бессонный" },
        { "Midnight Poet", "Полуночный Поэт" },
        { "Nightmare", "Кошмар" },
        { "Soul Assurer", "Успокоитель Душ" },
        { "Spirit Warlock", "Духовный Колдун" },
        { "Hunter", "Охотник" },
        { "Provoker", "Провокатор" },
        { "Pyromaniac", "Пироман" },
        { "Reaper", "Жнец" },
        { "Apprentice", "Ученик" },
        { "Trickmaster", "Мастер Трикстер" },
        { "Astrologer", "Астролог" },
        { "Scribe", "Писец" },
        { "Traveler", "Путешественник" },
        { "Secrets Sorcerer", "Маг Секретов" },
        { "Wanderer", "Скиталец" },
        { "Planeswalker", "Мироходец" },
        { "Key of Stars", "Ключ Звёзд" },
        { "Door", "Дверь" },
        { "Sheriff", "Шериф" },
        { "Judge", "Судья" },
        { "Arbiter", "Арбитр" },

        // Механики и концепции
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
        { "Chanis Gate", "Врата Чаниса" },
        { "City of Silver", "Город Серебра" },
        { "Moon City", "Город Луны" },
        { "Forsaken Land of the Gods", "Земля, Покинутая Богами" },
        { "Land of the Forsaken Gods", "Земля, Покинутая Богами" },
        { "Dowsing Rod Navigation", "Лозоходство" },
        { "Dowsing Rod", "Лоза для гадания" },
        { "Tingen in the Mirror", "Тинген в зеркале" },
        { "Mirror Tingen", "Зеркальный Тинген" },
        { "Giant King's Court", "Двор Короля Гигантов" },
        { "Requiem Poetry Society", "Поэтическое общество Заупокойной службы" },

        // Локации, заведения и районы
        { "Backlund", "Бэкланд" },
        { "Tingen", "Тинген" },
        { "Sinful Tingen", "«Грешный Тинген»" },
        { "Evil Dragon Bar", "бар «Злой Дракон»" },
        { "Eskerson Island", "остров Эскерсон" },
        { "South District", "Южный район" },
        { "Pritz Harbor", "гавань Притц" },
        { "East Borough", "Восточный район" },
        { "West Borough", "Западный район" },
        { "North Borough", "Северный район" },
        { "Hillston Borough", "район Хиллстон" },
        { "Cherwood Borough", "район Червуд" },
        { "Bachellor Street", "улица Бачелор" },
        { "Williams Street", "улица Уильямс" },
        { "Minsk Street", "улица Минск" },
        { "Boklund Street", "улица Боклунд" },
        { "Bravehearts Bar", "бар «Храброе Сердце»" },
        { "Harvest Church", "Церковь Урожая" },
        { "Church of the Harvest", "Церковь Урожая" },
        { "Nighthawks", "Ночные Ястребы" },
        { "Nighthawk", "Ночной Ястреб" },
        { "Mandated Punishers", "Уполномоченные Каратели" },
        { "Mandated Punisher", "Уполномоченный Каратель" },
        { "Machinery Hivemind", "Механический Разум" },
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
        { "Gold Pound", "Золотой фунт" },
        { "Gold Pounds", "Золотых фунтов" },
        { "Pound", "Фунт" },

        // Подземелья, экипировка, предметы и титулы (Пакет 14)
        { "Tree of Abundance (Hard)", "Древо Изобилия (Сложно)" },
        { "Tree of Abundance (Normal)", "Древо Изобилия (Обычно)" },
        { "Tree of Abundance", "Древо Изобилия" },
        { "May Manor · Castle (Hard)", "Поместье Мэй · Замок (Сложно)" },
        { "May Manor · Castle (Normal)", "Поместье Мэй · Замок (Обычно)" },
        { "May Manor", "Поместье Мэй" },
        { "Orange Quality Imprinted Equipment", "Оранжевая экипировка с клеймом" },
        { "Gold Quality Imprinted Equipment", "Золотая экипировка с клеймом" },
        { "Imprinted Equipment", "Экипировка с клеймом" },
        { "Mythical weapon", "Мифическое оружие" },
        { "Mythical weapons", "Мифическое оружие" },
        { "Mythical Cocoon", "Мифический кокон" },
        { "Cocoon Silk", "Шёлк кокона" },
        { "Embrace of Concealment", "«Объятия сокрытия»" },
        { "World Calamity", "Мировое бедствие" },
        { "Spectator Kill Chest", "Сундук убийства Зрителя" },
        { "Parliament Security Force", "Служба безопасности парламента" },

        // Боевые навыки монстров и боссов
        { "Blasphemous Touch", "Кощунственное касание" },
        { "Soul Impact", "Удар неупокоенных душ" },
        { "Breath of Abundance", "Дыхание Изобилия" },
        { "Triple Water Blade", "Тройное водное лезвие" },
        { "Undying Witness", "Неувядающий свидетель" },
        { "Harsh Discipline", "Суровое наставление" },
        { "Music Starts", "Музыка начинается" },
        { "Green of All Things", "Зелень всего сущего" },
        { "Triple Slash", "Тройной разрез" },
        { "Double Slash", "Двойной разрез" },
        { "Cut in Two", "Рассечение надвое" },
        { "One Pot Sweep", "Очищающий вихрь" },
        { "Ten Thousand Pounds Pressure", "Давление в десять тысяч фунтов" }
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
        int targetCount = 4000;
        int customCount;
        if (args.Length > 0 && int.TryParse(args[0], out customCount))
        {
            targetCount = customCount;
        }

        Console.WriteLine(string.Format("=== Запуск перевода Пакета №14 (Цель: {0} уникальных строк) ===", targetCount));

        // 1. Чтение существующего словаря
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

        // 2. Чтение тегов LSI
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
        Console.WriteLine(string.Format("Загружено тегов LSI: {0}", lsiTagMap.Count));

        // 3. Отбор кандидатов для Пакета №14
        var pItemNormal = new List<Tuple<string, string>>();
        var pTalkOther = new List<Tuple<string, string>>();
        var pItemGift = new List<Tuple<string, string>>();
        var pGossip = new List<Tuple<string, string>>();
        var pOldTalk = new List<Tuple<string, string>>();
        var pAsideTalk = new List<Tuple<string, string>>();
        var pOtherTalk = new List<Tuple<string, string>>();
        var pTingen = new List<Tuple<string, string>>();
        var pItemOutlook = new List<Tuple<string, string>>();
        var pMonsterSkill = new List<Tuple<string, string>>();
        var pOthers = new List<Tuple<string, string>>();

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

                        if (existingRu.ContainsKey(cnKey) || existingRu.ContainsKey(enVal) || seen.Contains(cnKey) || seen.Contains(enVal))
                            continue;

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") || enVal.StartsWith("[Discarded]") || enVal.StartsWith("dj"))
                            continue;

                        string sk = SourceKey(cnKey);
                        List<string> tags;
                        if (lsiTagMap.TryGetValue(sk, out tags))
                        {
                            if (tags.Contains("itemnormal")) pItemNormal.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("talkother")) pTalkOther.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("itemgift")) pItemGift.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("gossip")) pGossip.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("oldtalk")) pOldTalk.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("asidetalk")) pAsideTalk.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("othertalk")) pOtherTalk.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("tingentalk") || tags.Contains("tingen")) pTingen.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("itemoutlook")) pItemOutlook.Add(Tuple.Create(cnKey, enVal));
                            else if (tags.Contains("monsterskill")) pMonsterSkill.Add(Tuple.Create(cnKey, enVal));
                            else pOthers.Add(Tuple.Create(cnKey, enVal));

                            seen.Add(cnKey);
                            seen.Add(enVal);
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Кандидаты: ItemNormal={0}, TalkOther={1}, ItemGift={2}, Gossip={3}, OldTalk={4}, AsideTalk={5}, OtherTalk={6}, Tingen={7}, ItemOutlook={8}, MonsterSkill={9}",
            pItemNormal.Count, pTalkOther.Count, pItemGift.Count, pGossip.Count, pOldTalk.Count, pAsideTalk.Count, pOtherTalk.Count, pTingen.Count, pItemOutlook.Count, pMonsterSkill.Count));

        var candidates = new List<Tuple<string, string>>();

        // Добавляем все нарративные категории полностью
        foreach (var x in pItemNormal) candidates.Add(x);
        Console.WriteLine(string.Format("Добавлено ItemNormal: {0}", pItemNormal.Count));

        foreach (var x in pTalkOther) candidates.Add(x);
        Console.WriteLine(string.Format("Добавлено TalkOther: {0}", pTalkOther.Count));

        foreach (var x in pItemGift) candidates.Add(x);
        Console.WriteLine(string.Format("Добавлено ItemGift: {0}", pItemGift.Count));

        foreach (var x in pGossip) candidates.Add(x);
        Console.WriteLine(string.Format("Добавлено Gossip: {0}", pGossip.Count));

        foreach (var x in pOldTalk) candidates.Add(x);
        Console.WriteLine(string.Format("Добавлено OldTalk: {0}", pOldTalk.Count));

        foreach (var x in pAsideTalk) candidates.Add(x);
        Console.WriteLine(string.Format("Добавлено AsideTalk: {0}", pAsideTalk.Count));

        foreach (var x in pOtherTalk) candidates.Add(x);
        Console.WriteLine(string.Format("Добавлено OtherTalk: {0}", pOtherTalk.Count));

        foreach (var x in pTingen) candidates.Add(x);
        Console.WriteLine(string.Format("Добавлено Tingen: {0}", pTingen.Count));

        foreach (var x in pItemOutlook) candidates.Add(x);
        Console.WriteLine(string.Format("Добавлено ItemOutlook: {0}", pItemOutlook.Count));

        // Добираем боевыми навыками монстров
        int monsterTarget = targetCount - candidates.Count;
        int monsterAdded = 0;
        for (int i = 0; i < pMonsterSkill.Count && candidates.Count < targetCount; i++)
        {
            candidates.Add(pMonsterSkill[i]);
            monsterAdded++;
        }
        Console.WriteLine(string.Format("Добавлено MonsterSkill: {0}", monsterAdded));

        Console.WriteLine(string.Format("Итого отобрано в Пакет №14: {0} уникальных строк", candidates.Count));

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
                ru = PostProcessDialogue(ru);

                // Экранирование для Lua
                string escapedRu = CleanForLua(ru);
                string escapedCn = CleanForLua(cn);
                string escapedEn = CleanForLua(en);

                translatedResults[escapedCn] = escapedRu;
                translatedResults[escapedEn] = escapedRu;
                Interlocked.Increment(ref successes);
            }

            int p = Interlocked.Increment(ref processed);
            if (p % 50 == 0 || p == candidates.Count)
            {
                Console.Write(string.Format("\rПрогресс перевода: {0} / {1} (Успешно: {2})", p, candidates.Count, successes));
            }
            Thread.Sleep(25);
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

        // 7. Синхронизация с data/RuntimeTextRussian.lua
        try
        {
            File.Copy(RuPath, DataRuPath, true);
            Console.WriteLine("✅ Синхронизировано с data/RuntimeTextRussian.lua");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка data: " + ex.Message);
        }

        Console.WriteLine("Пакет №14 успешно интегрирован!");
    }

    static string ApplyCanonGlossary(string text)
    {
        string res = text;
        foreach (var pair in CanonExact)
        {
            res = Regex.Replace(res, @"\b" + Regex.Escape(pair.Key) + @"\b", pair.Value, RegexOptions.IgnoreCase);
        }

        // Канонизация и устранение машинных ошибок
        res = Regex.Replace(res, @"\b(Потусторонняя характеристика|потусторонняя характеристика|потусторонней характеристики|потустороннюю характеристику|потусторонней характеристике)\b", "Потустороннее свойство", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Потусторонние характеристики|потусторонние характеристики|потусторонних характеристик)\b", "Потусторонние свойства", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Мистер Дурак|господин Дурак|Господин Дурак|мистер Дурак)\b", "Мистер Шут", RegexOptions.IgnoreCase);
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

        // Специфика диалогов Тингена, сплетен и клубных партий
        res = Regex.Replace(res, @"\b(маленькая Дейли|маленькой Дейли|юная Дейли)\b", "малышка Дейли", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Тинген в зеркале|Тингена в зеркале)\b", "Тинген в зеркале", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(стрит от (\d+) до ([A-Za-zА-Яа-я]+))\b", "стрит от $1 до $2", RegexOptions.IgnoreCase);

        // Локации, заведения и районы
        res = Regex.Replace(res, @"\b(улице Зутланд|улица Зутланд|Зутланд)\b", "улица Зотланд", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(улице Нарцисс|улица Нарцисс)\b", "улица Нарциссов", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(улице Железного креста|улица Железного креста)\b", "улица Железного Креста", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Врата Чаниса|ворота Чаниса|Врата Шанис|Ворота Чанис)\b", "Врата Чаниса", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Город Серебра|Серебряный город)\b", "Город Серебра", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Город Луны|Лунный город)\b", "Город Луны", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Древний Бог Солнца|Древний бог солнца)\b", "Древний Бог Солнца", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Земля, покинутая богами|Покинутая богами земля)\b", "Земля, Покинутая Богами", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Бэклунд|Баклунд|Бекланд)\b", "Бэкланд", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(гавани Притц|Порт Притц|порт Притц|Притц-Харбор)\b", "гавань Притц", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Храбрые сердца|Храбрых сердец|Храброе сердце)\b", "«Храброе Сердце»", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Злой Дракон|Злого Дракона|Злому Дракону)\b", "«Злой Дракон»", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Двор короля гигантов|Двор Короля гигантов)\b", "Двор Короля Гигантов", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(Поэтическое общество Заупокойной службы|Общество реквиема|Общество Реквиема)\b", "Поэтическое общество Заупокойной службы", RegexOptions.IgnoreCase);
        res = Regex.Replace(res, @"\b(универмаг Гарольд|универмага Гарольд)\b", "универмаг Гарольда", RegexOptions.IgnoreCase);

        return res;
    }

    static string PostProcessDialogue(string text)
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
