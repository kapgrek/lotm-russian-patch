using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslatePatch26All
{
    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "The Fool", "Шут" },
        { "Lord of Mysteries", "Повелитель Тайн" },
        { "Lord of the Mysteries", "Повелитель Тайн" },
        { "Great Mother", "Великая Мать" },
        { "The Great Mother", "Великая Мать" },
        { "Evernight Goddess", "Богиня Вечной Ночи" },
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
        { "Great Old Ones", "Великие Древние" },
        { "Above the Grey Fog", "Над Серым Туманом" },
        { "Above the Gray Fog", "Над Серым Туманом" },
        { "Grey Fog", "Серый Туман" },
        { "Gray Fog", "Серый Туман" },
        { "Tarot Club", "Клуб Таро" },
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "Beyonder", "Потусторонний" },
        { "Beyonders", "Потусторонние" },
        { "Beyonder Characteristic", "Потустороннее свойство" },
        { "Beyonder Characteristics", "Потусторонние свойства" },
        { "Beyonder Material", "Потусторонний материал" },
        { "Beyonder Materials", "Потусторонние материалы" },
        { "Spirit Body Threads", "Нити духовного тела" },
        { "Spirit Body Thread", "Нить духовного тела" },
        { "Spirit Body", "Духовное тело" },
        { "Historical Projection", "Историческая проекция" },
        { "Historical Projections", "Исторические проекции" },
        { "Paper Figurine Substitute", "Замена бумажным человечком" },
        { "Paper Figurine Substitutes", "Замена бумажным человечком" },
        { "Flame Controlling", "Управление пламенем" },
        { "Acting Method", "Метод Лицедейства" },
        { "Digestion", "Усвоение" },
        { "Lose Control", "Потерять контроль" },
        { "Loss of Control", "Потеря контроля" },
        { "Madness", "Безумие" },
        { "Sanity", "Рассудок" },
        { "Corruption", "Искажение" },
        { "Spirituality", "Духовность" },
        { "Astral Projection", "Астральная проекция" },
        { "Mystical Item", "Мистический предмет" },
        { "Mystical Items", "Мистические предметы" },
        { "Charm", "Оберег" },
        { "Charms", "Обереги" },
        { "Talisman", "Амулет" },
        { "Talismans", "Амулеты" },
        { "Divination", "Гадание" },
        { "Spirit World", "Мир Духов" },
        { "Marionette", "Марионетка" },
        { "Marionettes", "Марионетки" },
        { "Marionettist", "Марионеточник" },
        { "Seer", "Провидец" },
        { "Clown", "Клоун" },
        { "Magician", "Фокусник" },
        { "Faceless", "Безликий" },
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
        { "Provoker", "Провокатор" },
        { "Pyromaniac", "Пироман" },
        { "Reaper", "Жнец" },
        { "Apprentice", "Ученик" },
        { "Trickmaster", "Мастер Трикстер" },
        { "Astrologer", "Астролог" },
        { "Arbiter", "Арбитр" },
        { "Sheriff", "Шериф" },
        { "Judge", "Судья" },
        { "Nighthawks", "Ночные Ястребы" },
        { "Mandated Punishers", "Уполномоченные Каратели" },
        { "Machinery Hivemind", "Механический Разум" },
        { "Aurora Order", "Орден Авроры" },
        { "Iron and Blood Cross Order", "Орден Железного и Кровавого Креста" },
        { "Rose School of Thought", "Школа Мысли Розы" },
        { "Secret Order", "Тайный Орден" },
        { "Tingen", "Тинген" },
        { "Backlund", "Бэкланд" },
        { "Intis", "Интис" },
        { "Loen", "Лоэн" },
        { "Klein Moretti", "Клейн Моретти" },
        { "Klein", "Клейн" },
        { "Dunn Smith", "Данн Смит" },
        { "Captain Dunn", "капитан Данн" },
        { "Old Neil", "Старина Нил" },
        { "Leonard Mitchell", "Леонард Митчелл" },
        { "Leonard", "Леонард" },
        { "Melissa Moretti", "Мелисса Моретти" },
        { "Melissa", "Мелисса" },
        { "Benson Moretti", "Бенсон Моретти" },
        { "Benson", "Бенсон" },
        { "Daly Simone", "Дейли Симон" },
        { "Captain Frye", "капитан Фрай" },
        { "Frye", "Фрай" },
        { "Audrey Hall", "Одри Холл" },
        { "Audrey", "Одри" },
        { "Alger Wilson", "Алджер Уилсон" },
        { "Alger", "Алджер" },
        { "Derrick Berg", "Деррик Берг" },
        { "Derrick", "Деррик" },
        { "Roselle Gustav", "Розель Густав" },
        { "Emperor Roselle", "Император Розель" },
        { "Roselle", "Розель" },
        { "Blackthorn Security Company", "Охранная компания «Чёрный Чертополох»" },
        { "Blackthorn", "Чёрный Чертополох" },
        { "Bravehearts Bar", "бар «Храброе Сердце»" },
        { "Dragon Bar", "бар «Злой Дракон»" },
        { "Gold Pound", "золотой фунт" },
        { "Gold Pounds", "золотых фунтов" },
        { "Soli", "суле" },
        { "Sule", "суле" },
        { "Pence", "пенсов" },
        { "Damage Reduction", "Снижение урона" },
        { "Super Armor", "Суперброня" },
        { "Cleanse", "Снятие контроля" },
        { "Cooldown Reduction", "Сокращение перезарядки" },
        { "Cooldown", "Перезарядка" },
        { "Mind Fire", "Пламя разума" },
        { "Induction Mark", "Метка внушения" },
        { "Hypnosis", "Гипноз" },
        { "Imprisonment", "Заточение" },
        { "Susie", "Сузи" }
    };

    static string CachePath = @"d:\gameDev\translate lotm\tools\patch26_translation_cache.tsv";
    static object cacheLock = new object();
    static ConcurrentDictionary<string, string> translationCache = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        // Enable TLS 1.2
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;

        string rootDir = @"d:\gameDev\translate lotm";
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");
        string ruMasterFile = Path.Combine(rootDir, @"RuntimeTextRussian.lua");

        Console.WriteLine("========================================================================");
        Console.WriteLine("       AUTOMATED TRANSLATION & INTEGRATION OF ENGLISH PATCH 2.6.0       ");
        Console.WriteLine("========================================================================");

        // Load Cache
        LoadCache();

        // Load 2.6 Shards
        Console.WriteLine("\n[1/6] Loading all 2.6 shards...");
        var en26Entries = new Dictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shards26Dir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);
        foreach (var f in shardFiles) ParseLuaTable(f, en26Entries);
        Console.WriteLine("Loaded " + en26Entries.Count + " entries from 2.6 shards.");

        // Load RU master dictionary
        Console.WriteLine("\n[2/6] Loading RuntimeTextRussian.lua...");
        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(ruMasterFile, ruDict);
        Console.WriteLine("Loaded " + ruDict.Count + " existing entries in RuntimeTextRussian.lua.");

        // Apply Step 1: Sync 123 Missing CN keys
        Console.WriteLine("\n[3/6] Synchronizing missing CN keys...");
        int syncedCn = 0;
        foreach (var kvp in en26Entries)
        {
            string cn = kvp.Key;
            string en = kvp.Value;
            if (!ruDict.ContainsKey(cn) && ruDict.ContainsKey(en))
            {
                ruDict[cn] = ruDict[en];
                syncedCn++;
            }
        }
        Console.WriteLine("Synchronized " + syncedCn + " missing CN keys.");

        // Apply Step 2: Fix 35 modified strings and Sealed Artifact formula
        Console.WriteLine("\n[4/6] Applying fixes for 35 modified strings and combat formula...");
        ApplyModifiedFixes(ruDict);

        // Step 3: Find all remaining untranslated strings
        var missingList = new List<KeyValuePair<string, string>>();
        foreach (var kvp in en26Entries)
        {
            if (!ruDict.ContainsKey(kvp.Key) && !ruDict.ContainsKey(kvp.Value))
            {
                missingList.Add(kvp);
            }
        }
        Console.WriteLine(string.Format("Found {0} untranslated entries to process.", missingList.Count));

        // Step 4: Multithreaded translation
        Console.WriteLine("\n[5/6] Translating untranslated entries in parallel...");
        int total = missingList.Count;
        int completed = 0;

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 8 };
        Parallel.ForEach(missingList, parallelOptions, item =>
        {
            string cn = item.Key;
            string en = item.Value;
            string ru = TranslateEntry(cn, en);

            lock (ruDict)
            {
                ruDict[cn] = ru;
                ruDict[en] = ru;
            }

            SaveToCache(en, ru);

            int done = Interlocked.Increment(ref completed);
            if (done % 200 == 0 || done == total)
            {
                Console.WriteLine(string.Format("Progress: {0} / {1} ({2:F1}%)", done, total, (double)done / total * 100.0));
            }
        });

        // Step 5: Save updated RuntimeTextRussian.lua
        Console.WriteLine("\n[6/6] Writing updated RuntimeTextRussian.lua...");
        // Backup
        string bakRu = ruMasterFile + ".bak_pre26";
        if (!File.Exists(bakRu)) File.Copy(ruMasterFile, bakRu);

        using (var writer = new StreamWriter(ruMasterFile, false, Encoding.UTF8))
        {
            writer.WriteLine("-- Russian Master Translation Dictionary");
            writer.WriteLine("-- Total unique dual-indexed entries: " + ruDict.Count);
            writer.WriteLine("return {");
            foreach (var kvp in ruDict)
            {
                writer.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", CleanForLua(kvp.Key), CleanForLua(kvp.Value)));
            }
            writer.WriteLine("}");
        }
        Console.WriteLine("Saved RuntimeTextRussian.lua with " + ruDict.Count + " entries.");
        Console.WriteLine("\nALL TRANSLATIONS COMPLETE!");
    }

    static string TranslateEntry(string cn, string en)
    {
        // 1. Check cache
        string cached;
        if (translationCache.TryGetValue(en, out cached)) return cached;

        // 2. Dev placeholders
        if (cn.Contains("占位") || en.IndexOf("placeholder", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "[Заглушка]";
        }
        if (cn.Contains("文本文本") || en.IndexOf("text text text", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "Текст текст текст текст текст";
        }
        if (cn.Contains("一二三四五六七八九十"))
        {
            return "Один Два Три Четыре Пять Шесть Семь Восемь Девять Десять";
        }
        if (cn.Contains("七个字") && en.Contains("Seven Chars"))
        {
            return "[Семь символов]";
        }

        // 3. Known patterns
        if (en.StartsWith("When an ordinary skill enters cooldown, the cooldown is immediately returned"))
        {
            return "Когда обычный навык уходит на перезарядку, перезарядка немедленно сбрасывается. Если это заряжаемый навык, восстанавливаются все заряды. Сброс может сработать максимум 1 раз для одного навыка. {CheckStar(Type=\"sealed\",ID=2085021)=1?Сброшенный навык наносит на <Yellow>*f**</> меньше урона и исцеления. }{CheckStar(Type=\"sealed\",ID=2085021)=3?Сброшенный навык дополнительно наносит на <Yellow>*f**</> больше урона и исцеления. }";
        }

        // 4. Protect tokens and translate
        string translated = TranslateGoogle(en);
        translated = ApplyCanon(translated);
        translated = PostProcessCleanup(translated);
        return translated;
    }

    static void ApplyModifiedFixes(Dictionary<string, string> ruDict)
    {
        // Add Sealed Artifact formula
        string cnFormula = "普通技能进入冷却时立刻返还冷却，如果是充能技能则返还全部充能次数，单个技能最多触发1次返还。{CheckStar(Type=\"sealed\",ID=2085021)=1?返还的技能降低<Yellow>*f**</>伤害和治疗。}{CheckStar(Type=\"sealed\",ID=2085021)=3?返还的技能额外提高<Yellow>*f**</>伤害和治疗。}";
        string enFormula = "When an ordinary skill enters cooldown, the cooldown is immediately returned. If it is a charge skill, all charge counts are returned. A single skill can trigger this return at most once. {CheckStar(Type=\"sealed\",ID=2085021)=1?The returned skill deals <Yellow>*f**</> less damage and healing. }{CheckStar(Type=\"sealed\",ID=2085021)=3?The returned skill deals <Yellow>*f**</> more damage and healing. }";
        string ruFormula = "Когда обычный навык уходит на перезарядку, перезарядка немедленно сбрасывается. Если это заряжаемый навык, восстанавливаются все заряды. Сброс может сработать максимум 1 раз для одного навыка. {CheckStar(Type=\"sealed\",ID=2085021)=1?Сброшенный навык наносит на <Yellow>*f**</> меньше урона и исцеления. }{CheckStar(Type=\"sealed\",ID=2085021)=3?Сброшенный навык дополнительно наносит на <Yellow>*f**</> больше урона и исцеления. }";
        ruDict[cnFormula] = ruFormula;
        ruDict[enFormula] = ruFormula;

        // Fix gender tokens and malformed tags
        var keysToFix = new List<string>(ruDict.Keys);
        foreach (var k in keysToFix)
        {
            string val = ruDict[k];
            bool changed = false;

            if (val.Contains("{Mr.{ Ms.|}}") || val.Contains("{Mr.{Ms.|}}") || val.Contains("{Mr.{ Ms. |}}"))
            {
                val = val.Replace("{Mr.{ Ms.|}}", "{{мистер|мисс}}")
                         .Replace("{Mr.{Ms.|}}", "{{мистер|мисс}}")
                         .Replace("{Mr.{ Ms. |}}", "{{мистер|мисс}}");
                changed = true;
            }
            if (val.Contains("</h>"))
            {
                val = val.Replace("</h>", "</>");
                changed = true;
            }
            if (val.Contains("</mark>"))
            {
                val = val.Replace("</mark>", "</>");
                changed = true;
            }

            if (changed)
            {
                ruDict[k] = val;
            }
        }
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

    static string PostProcessCleanup(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        text = text.Replace("Кляйн", "Клейн")
                   .Replace("Блэкторн", "Чёрный Чертополох")
                   .Replace("Старый Нил", "Старина Нил")
                   .Replace("солей", "суле")
                   .Replace("соля", "суле")
                   .Replace("характеристика", "свойство")
                   .Replace("Характеристика", "Свойство")
                   .Replace("Потусторонняя характеристика", "Потустороннее свойство")
                   .Replace("потусторонняя характеристика", "потустороннее свойство")
                   .Replace("Дурак", "Шут")
                   .Replace("Глупец", "Шут")
                   .Replace("{{Mr.|Ms.}}", "{{мистер|мисс}}");
        return text;
    }

    static string TranslateGoogle(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        var tagMap = new Dictionary<string, string>();
        int tagIdx = 0;
        string protectedText = Regex.Replace(text, @"<[^>]+>|\{\{[^}]+\}\}|\{[^{}]+\}|\\n|\\t|\*d\*\*|\*d|\*f\*\*|\*f|mul\([^)]+\)|spellfielddisc\([^)]+\)|buffdisc\([^)]+\)|bulletdisc\([^)]+\)|buffappear\([^)]+\)|CheckStar\([^)]+\)|%s|%d|%i|%f", m =>
        {
            string ph = "ZZTAG" + (tagIdx++) + "ZZ";
            tagMap[ph] = m.Value;
            return ph;
        });

        int retries = 3;
        while (retries-- > 0)
        {
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
                    if (string.IsNullOrWhiteSpace(result)) result = text;

                    foreach (var kvp in tagMap)
                    {
                        result = result.Replace(kvp.Key, kvp.Value);
                        result = result.Replace(kvp.Key.ToLower(), kvp.Value);
                    }
                    return result;
                }
            }
            catch (Exception)
            {
                Thread.Sleep(500);
            }
        }

        return text;
    }

    static void LoadCache()
    {
        if (!File.Exists(CachePath)) return;
        using (var reader = new StreamReader(CachePath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split('\t');
                if (parts.Length >= 2)
                {
                    translationCache[parts[0]] = parts[1];
                }
            }
        }
        Console.WriteLine("Loaded " + translationCache.Count + " entries from cache.");
    }

    static void SaveToCache(string en, string ru)
    {
        translationCache[en] = ru;
        lock (cacheLock)
        {
            using (var writer = new StreamWriter(CachePath, true, Encoding.UTF8))
            {
                writer.WriteLine(Escape(en) + "\t" + Escape(ru));
            }
        }
    }

    static void ParseLuaTable(string filePath, Dictionary<string, string> dict)
    {
        using (var reader = new StreamReader(filePath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (!line.StartsWith("[\"")) continue;
                int firstQuote = 2;
                int keyEndQuote = FindClosingQuote(line, firstQuote);
                if (keyEndQuote == -1) continue;
                string key = Unescape(line.Substring(firstQuote, keyEndQuote - firstQuote));

                int eqIdx = line.IndexOf('=', keyEndQuote);
                if (eqIdx == -1) continue;
                int valStartQuote = line.IndexOf('"', eqIdx);
                if (valStartQuote == -1) continue;
                valStartQuote++;
                int valEndQuote = FindClosingQuote(line, valStartQuote);
                if (valEndQuote == -1) continue;
                string val = Unescape(line.Substring(valStartQuote, valEndQuote - valStartQuote));
                dict[key] = val;
            }
        }
    }

    static int FindClosingQuote(string s, int start)
    {
        for (int i = start; i < s.Length; i++)
        {
            if (s[i] == '"' && (i == 0 || s[i - 1] != '\\')) return i;
        }
        return -1;
    }

    static string Unescape(string s)
    {
        return s.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\\", "\\");
    }

    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "");
        r = r.Replace("\\", "\\\\");
        r = r.Replace("\"", "\\\"");
        return r;
    }

    static string Escape(string s)
    {
        if (s == null) return "";
        return s.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }
}
