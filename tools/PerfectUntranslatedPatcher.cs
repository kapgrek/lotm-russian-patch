using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class PerfectUntranslatedPatcher
{
    static readonly Dictionary<string, string> CanonMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "The Fool", "Шут" },
        { "Lord of Mysteries", "Повелитель Тайн" },
        { "Lord of the Mysteries", "Повелитель Тайн" },
        { "Beyonder", "Потусторонний" },
        { "Beyonders", "Потусторонние" },
        { "Beyonder Material", "Потусторонний материал" },
        { "Beyonder Materials", "Потусторонние материалы" },
        { "Beyonder Characteristic", "Потустороннее свойство" },
        { "Beyonder Characteristics", "Потусторонние свойства" },
        { "Spirit Body Threads", "Нити духовного тела" },
        { "Spirit Body Thread", "Нить духовного тела" },
        { "Spirit Body", "Духовное тело" },
        { "Astral Projection", "Астральная проекция" },
        { "Historical Projection", "Историческая проекция" },
        { "Historical Projections", "Исторические проекции" },
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "Paper Figurine Substitute", "Замена бумажным человечком" },
        { "Paper Figurine Substitutes", "Замена бумажным человечком" },
        { "Flame Controlling", "Управление пламенем" },
        { "Tarot Club", "Клуб Таро" },
        { "Nighthawks", "Ночные Ястребы" },
        { "Tingen", "Тинген" },
        { "Backlund", "Бэкланд" },
        { "Intis", "Интис" },
        { "Loen", "Лоэн" },
        { "Roselle", "Розель" },
        { "Klein", "Клейн" },
        { "Dunn Smith", "Данн Смит" },
        { "Old Neil", "Старина Нил" },
        { "Leonard", "Леонард" },
        { "Melissa", "Мелисса" },
        { "Benson", "Бенсон" },
        { "Blackthorn Security Company", "Охранная компания «Чёрный Чертополох»" },
        { "Blackthorn", "Чёрный Чертополох" },
        { "Gold Pound", "золотой фунт" },
        { "Gold Pounds", "золотых фунтов" },
        { "Soli", "суле" },
        { "Pence", "пенсов" },
        { "Damage Reduction", "Снижение урона" },
        { "Super Armor", "Суперброня" },
        { "Cleanse", "Снятие контроля" },
        { "Cooldown", "Перезарядка" },
        { "Cooldown Reduction", "Сокращение перезарядки" },
        { "Mind Fire", "Пламя разума" },
        { "Induction Mark", "Метка внушения" },
        { "Hypnosis", "Гипноз" },
        { "Imprisonment", "Заточение" },
        { "Spectator", "Зритель" },
        { "Telepathist", "Телепат" },
        { "Psychiatrist", "Психиатр" },
        { "Sleepless", "Бессонный" },
        { "Midnight Poet", "Полуночный Поэт" },
        { "Nightmare", "Кошмар" },
        { "Seer", "Провидец" },
        { "Clown", "Клоун" },
        { "Magician", "Фокусник" },
        { "Faceless", "Безликий" },
        { "Marionettist", "Марионеточник" },
        { "Bizarro Sorcerer", "Маг Непостижимого" },
        { "Scholar of Yore", "Учёный Прошлого" },
        { "Miracle Invoker", "Творец Чудес" },
        { "Attendant of Mysteries", "Служитель Тайн" },
        { "Marionette", "Марионетка" },
        { "Marionettes", "Марионетки" }
    };

    static readonly Dictionary<string, string> CanMoveMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "#CanMove不朽传奇#", "#CanMoveБессмертная легенда#" },
        { "#CanMoveImmortal Legend#", "#CanMoveБессмертная легенда#" },
        { "#CanMove你#", "#CanMoveТы#" },
        { "#CanMoveYou#", "#CanMoveТы#" },
        { "#CanMove修复#", "#CanMoveПочинить#" },
        { "#CanMoveRepair#", "#CanMoveПочинить#" },
        { "#CanMove窟窿#", "#CanMoveДыра#" },
        { "#CanMoveHole#", "#CanMoveДыра#" },
        { "#CanMove红色的#", "#CanMoveКрасный#" },
        { "#CanMoveRed#", "#CanMoveКрасный#" },
        { "#CanMove绳子#", "#CanMoveВерёвка#" },
        { "#CanMoveRope#", "#CanMoveВерёвка#" },
        { "#CanMove绿色的#", "#CanMoveЗелёный#" },
        { "#CanMoveGreen#", "#CanMoveЗелёный#" },
        { "#CanMove老人#", "#CanMoveСтарик#" },
        { "#CanMoveOldMan#", "#CanMoveСтарик#" },
        { "#CanMove老鼠#", "#CanMoveМышь#" },
        { "#CanMoveMouse#", "#CanMoveМышь#" },
        { "#CanMove船#", "#CanMoveКорабль#" },
        { "#CanMoveShip#", "#CanMoveКорабль#" },
        { "#CanMove苹果#", "#CanMoveЯблоко#" },
        { "#CanMoveApple#", "#CanMoveЯблоко#" },
        { "#CanMove荣耀#", "#CanMoveСлава#" },
        { "#CanMoveGlory#", "#CanMoveСлава#" },
        { "#CanMove蒸汽之子#", "#CanMoveДитя Пара#" },
        { "#CanMoveChildOfSteam#", "#CanMoveДитя Пара#" },
        { "#CanMove讨厌#", "#CanMoveНенависть#" },
        { "#CanMoveHate#", "#CanMoveНенависть#" },
        { "#CanMove财富#", "#CanMoveБогатство#" },
        { "#CanMoveWealth#", "#CanMoveБогатство#" },
        { "#CanMove资源#", "#CanMoveРесурсы#" },
        { "#CanMoveResources#", "#CanMoveРесурсы#" },
        { "#CanMove跳舞#", "#CanMoveТанец#" },
        { "#CanMoveDance#", "#CanMoveТанец#" },
        { "#CanMove释放#", "#CanMoveОсвобождение#" },
        { "#CanMoveRelease#", "#CanMoveОсвобождение#" },
        { "#CanMove金钱#", "#CanMoveДеньги#" },
        { "#CanMoveMoney#", "#CanMoveДеньги#" },
        { "#CanMove钥匙#", "#CanMoveКлюч#" },
        { "#CanMoveKey#", "#CanMoveКлюч#" },
        { "#CanMove问候#", "#CanMoveПриветствие#" },
        { "#CanMoveGreeting#", "#CanMoveПриветствие#" },
        { "#CanMove风车#", "#CanMoveМельница#" },
        { "#CanMoveWindmill#", "#CanMoveМельница#" },
        { "#CanMove驱赶#", "#CanMoveПрогнать#" },
        { "#CanMoveDriveAway#", "#CanMoveПрогнать#" },
        { "#CanMove魔女#", "#CanMoveВедьма#" },
        { "#CanMoveWitch#", "#CanMoveВедьма#" },
        { "#CanMove鸟窝#", "#CanMoveГнездо#" },
        { "#CanMoveBirdNest#", "#CanMoveГнездо#" },
        { "#CanMove黄色的#", "#CanMoveЖёлтый#" },
        { "#CanMoveYellow#", "#CanMoveЖёлтый#" },
        { "#CanMove三个字#", "#CanMoveТри слова#" },
        { "#CanMoveThree words#", "#CanMoveТри слова#" },
        { "#CanMove严打乱排废水#", "#CanMoveБорьба со сбросом сточных вод#" },
        { "#CanMoveStrictly crack down on illegal wastewater discharge#", "#CanMoveБорьба со сбросом сточных вод#" },
        { "#CanMove保卫环境治理污水#", "#CanMoveЗащита среды и очистка стоков#" },
        { "#CanMoveProtect the environment and treat sewage#", "#CanMoveЗащита среды и очистка стоков#" },
        { "#CanMove冷#空气下沉。", "#CanMoveХолодный# воздух опускается." },
        { "#CanMoveCold# air sinks.", "#CanMoveХолодный# воздух опускается." },
        { "#CanMove凯撒大帝#", "Император Цезарь" },
        { "#CanMoveCaesar the Great#", "Император Цезарь" },
        { "#CanMove击溃#", "Разгром" },
        { "#CanMoveDefeat#", "Разгром" },
        { "#CanMove利剑#", "Острый меч" },
        { "#CanMoveSharp Sword#", "Острый меч" },
        { "#CanMove武力#", "Сила" },
        { "#CanMoveMight#", "Сила" }
    };

    static readonly Dictionary<string, string> FormatMap = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        { "%.1f0,000", "%.1f0 000" },
        { "%.1f万", "%.1f0 000" },
        { "%.2f0,000", "%.2f0 000" },
        { "%.2f万", "%.2f0 000" },
        { "%d0,000", "%d0 000" },
        { "%d万", "%d0 000" },
        { "%dth", "%d-е место" },
        { "%d名", "%d-е место" },
        { "%d (Me)", "%d (я)" },
        { "%d（我）", "%d (я)" },
        { "%s - Advanced Dye", "%s — Высшая краска" },
        { "%s·高级染", "%s — Высшая краска" },
        { "%s·(%s Summon)", "%s·(Призыв: %s)" },
        { "%s·（%s召唤）", "%s·(Призыв: %s)" },
        { "%sRank %s", "%s Место: %s" },
        { "%s第%s名", "%s Место: %s" },
        { "%s %s Tycoon", "%s магнат: %s" },
        { "%s第%s大亨", "%s магнат: %s" },
        { "1", "1 шт." },
        { "1个", "1 шт." },
        { "3", "3 шт." },
        { "3个", "3 шт." },
        { "4", "4 шт." },
        { "4个", "4 шт." },
        { "5", "5 шт." },
        { "5个", "5 шт." }
    };

    static readonly Dictionary<string, string> MissingEnMap = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        { "Unlocked at Level %i", "Разблокируется на ур. %i" },
        { "%i级解锁", "Разблокируется на ур. %i" },
        { "Ageless", "Нестареющий" },
        { "不老", "Нестареющий" },
        { "Physician", "Врачеватель" },
        { "医师", "Врачеватель" },
        { "Scrolls Professor", "Профессор Свитков" },
        { "卷轴教授", "Профессор Свитков" },
        { "Fallen Earl", "Падший Граф" },
        { "堕落伯爵", "Падший Граф" },
        { "Astronomer", "Астроном" },
        { "天文学家", "Астроном" },
        { "Soul Assurer", "Успокоитель душ" },
        { "安魂师", "Успокоитель душ" },
        { "Parasite", "Паразит" },
        { "寄生者", "Паразит" },
        { "Warlock", "Колдун" },
        { "巫师", "Колдун" },
        { "Druid", "Друид" },
        { "德鲁伊", "Друид" },
        { "Demon", "Дьявол" },
        { "恶魔", "Дьявол" },
        { "Disciplinary Paladin", "Карающий Рыцарь" },
        { "惩戒骑士", "Карающий Рыцарь" },
        { "Raging Blow", "Яростный Бьющий" },
        { "暴怒之民", "Яростный Бьющий" },
        { "Doomsday", "Апокалипсис" },
        { "末日", "Апокалипсис" },
        { "Melee Scholar", "Учёный Рукопашного Боя" },
        { "格斗学者", "Учёный Рукопашного Боя" },
        { "Apostle of Desire", "Апостол Желания" },
        { "欲望使徒", "Апостол Желания" },
        { "Might", "Сила" },
        { "武力", "Сила" },
        { "Mercury Snake", "Змей Ртути" },
        { "水银之蛇", "Змей Ртути" },
        { "Discernor", "Прозревающий" },
        { "洞悉者", "Прозревающий" },
        { "Scholar of Crimson", "Учёный Багрянца" },
        { "深红学者", "Учёный Багрянца" },
        { "Mentor of Confusion", "Наставник Замешательства" },
        { "混乱导师", "Наставник Замешательства" },
        { "Calamity", "Бедствие" },
        { "灾难", "Бедствие" },
        { "Biologist", "Биолог" },
        { "生物学家", "Биолог" },
        { "Madman", "Безумец" },
        { "疯子", "Безумец" },
        { "Agony", "Страдание" },
        { "痛苦", "Страдание" },
        { "Dream Weaver", "Ткач Сновидений" },
        { "织梦人", "Ткач Сновидений" },
        { "Navigater", "Мореплаватель" },
        { "航海家", "Мореплаватель" },
        { "Briar Bishop", "Подкупатель" },
        { "贿赂者", "Подкупатель" },
        { "Serial Killer", "Серийный Убийца" },
        { "连环杀手", "Серийный Убийца" },
        { "Iron-Blooded Knight", "Железнокровный Рыцарь" },
        { "铁血骑士", "Железнокровный Рыцарь" },
        { "Grants 2-4 Random Affixes", "Даёт 2–4 случайных свойства" },
        { "随机获得2-4个词条", "Даёт 2–4 случайных свойства" },
        { "Potion Professor", "Профессор Зелий" },
        { "魔药教授", "Профессор Зелий" }
    };

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        string cnMissingPath = @"d:\gameDev\translate lotm\tools\untranslated_missing_cn_keys.tsv";
        string identicalPath = @"d:\gameDev\translate lotm\tools\untranslated_identical_to_en.tsv";

        Console.WriteLine("=================================================");
        Console.WriteLine(" PERFECT IN-PLACE UNTRANSLATED PATCHER FOR LOTM  ");
        Console.WriteLine("=================================================");

        // 1. Read RuntimeTextRussian.lua line by line
        Console.WriteLine("\n[1/5] Loading RuntimeTextRussian.lua lines...");
        var fileLines = new List<string>(File.ReadAllLines(ruPath, Encoding.UTF8));
        var keyToLineIndex = new Dictionary<string, int>(StringComparer.Ordinal);
        var existingDict = new Dictionary<string, string>(StringComparer.Ordinal);

        for (int i = 0; i < fileLines.Count; i++)
        {
            string t = fileLines[i].Trim();
            if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
            {
                int delim = t.IndexOf("\"] = \"");
                if (delim > 0)
                {
                    string k = t.Substring(2, delim - 2);
                    int valStart = delim + 6;
                    int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                    string v = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";
                    keyToLineIndex[k] = i;
                    existingDict[k] = v;
                }
            }
        }
        Console.WriteLine(string.Format("Loaded {0} lines, indexed {1} dictionary keys.", fileLines.Count, keyToLineIndex.Count));

        var pendingAppends = new Dictionary<string, string>(StringComparer.Ordinal);

        // 2. Apply 32 Missing Sequence keys
        Console.WriteLine("\n[2/5] Applying 32 missing Sequence keys & CN pairs...");
        int seqUpdated = 0;
        int seqAppended = 0;
        foreach (var kvp in MissingEnMap)
        {
            string cleanK = CleanLuaKey(kvp.Key);
            string cleanV = CleanLuaValue(kvp.Value);

            if (keyToLineIndex.ContainsKey(cleanK))
            {
                fileLines[keyToLineIndex[cleanK]] = string.Format("    [\"{0}\"] = \"{1}\",", cleanK, cleanV);
                existingDict[cleanK] = cleanV;
                seqUpdated++;
            }
            else
            {
                pendingAppends[cleanK] = cleanV;
                existingDict[cleanK] = cleanV;
                seqAppended++;
            }
        }
        Console.WriteLine(string.Format("Sequence keys: {0} updated in-place, {1} queued for append.", seqUpdated, seqAppended));

        // 3. Link Missing CN keys to existing Russian values
        Console.WriteLine("\n[3/5] Linking missing CN keys with existing Russian translations...");
        int cnLinked = 0;
        var cnLines = File.ReadAllLines(cnMissingPath, Encoding.UTF8);
        for (int i = 1; i < cnLines.Length; i++)
        {
            string l = cnLines[i];
            if (string.IsNullOrWhiteSpace(l)) continue;
            var parts = l.Split('\t');
            if (parts.Length >= 2)
            {
                string cn = parts[0];
                string en = parts[1];
                string cleanCn = CleanLuaKey(cn);
                string cleanEn = CleanLuaKey(en);

                if (!keyToLineIndex.ContainsKey(cleanCn) && !pendingAppends.ContainsKey(cleanCn))
                {
                    string ruVal = null;
                    if (existingDict.ContainsKey(cleanEn)) ruVal = existingDict[cleanEn];
                    else if (existingDict.ContainsKey(en)) ruVal = existingDict[en];

                    if (!string.IsNullOrEmpty(ruVal) && ruVal.Trim() != cleanEn.Trim() && ruVal.Trim() != en.Trim())
                    {
                        pendingAppends[cleanCn] = ruVal;
                        existingDict[cleanCn] = ruVal;
                        cnLinked++;
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Linked {0} missing CN keys to existing translations!", cnLinked));

        // 4. Translate 2,521 strings from untranslated_identical_to_en.tsv
        Console.WriteLine("\n[4/5] Translating 2,521 entries from untranslated_identical_to_en.tsv...");
        var idLines = File.ReadAllLines(identicalPath, Encoding.UTF8);
        var toTranslate = new List<Tuple<string, string>>();

        int staticHandled = 0;
        for (int i = 1; i < idLines.Length; i++)
        {
            string l = idLines[i];
            if (string.IsNullOrWhiteSpace(l)) continue;
            var parts = l.Split('\t');
            string cn = parts[0];
            string en = parts.Length > 1 ? parts[1] : "";

            string ru = null;
            if (CanMoveMap.ContainsKey(cn)) ru = CanMoveMap[cn];
            else if (CanMoveMap.ContainsKey(en)) ru = CanMoveMap[en];
            else if (FormatMap.ContainsKey(en)) ru = FormatMap[en];
            else if (FormatMap.ContainsKey(cn)) ru = FormatMap[cn];
            else if (MissingEnMap.ContainsKey(en)) ru = MissingEnMap[en];
            else if (MissingEnMap.ContainsKey(cn)) ru = MissingEnMap[cn];

            if (ru != null)
            {
                ApplyTranslation(fileLines, keyToLineIndex, pendingAppends, existingDict, cn, en, ru);
                staticHandled++;
            }
            else
            {
                toTranslate.Add(Tuple.Create(cn, en));
            }
        }
        Console.WriteLine(string.Format("Pre-handled static/format/tags: {0}. Queued for online translation: {1}", staticHandled, toTranslate.Count));

        // Parallel translation with 4 threads and 100ms delay to avoid rate limits
        int processed = 0;
        int failed = 0;
        var translatedBatch = new ConcurrentDictionary<string, string>();

        Parallel.ForEach(toTranslate, new ParallelOptions { MaxDegreeOfParallelism = 4 }, item =>
        {
            string cn = item.Item1;
            string en = item.Item2;

            string ru = TranslateMultiEndpoint(en);
            if (!string.IsNullOrWhiteSpace(ru) && ru.Trim() != en.Trim())
            {
                foreach (var pair in CanonMap)
                {
                    ru = Regex.Replace(ru, @"\b" + Regex.Escape(pair.Key) + @"\b", pair.Value, RegexOptions.IgnoreCase);
                }
                ru = ru.Replace("Кляйн", "Клейн");
                ru = ru.Replace("Блэкторн", "Чёрный Чертополох");
                ru = ru.Replace("солей", "суле");
                ru = ru.Replace("соля", "суле");

                translatedBatch[en] = ru;
            }
            else
            {
                Interlocked.Increment(ref failed);
            }

            int p = Interlocked.Increment(ref processed);
            if (p % 25 == 0 || p == toTranslate.Count)
            {
                Console.Write(string.Format("\rTranslation progress: {0} / {1} (Failed: {2})", p, toTranslate.Count, failed));
            }
            Thread.Sleep(100);
        });

        Console.WriteLine(string.Format("\nTranslation finished. Applying {0} translations...", translatedBatch.Count));

        foreach (var item in toTranslate)
        {
            string cn = item.Item1;
            string en = item.Item2;
            if (translatedBatch.ContainsKey(en))
            {
                ApplyTranslation(fileLines, keyToLineIndex, pendingAppends, existingDict, cn, en, translatedBatch[en]);
            }
        }

        // Re-check missing CN keys that matched translated batch
        for (int i = 1; i < cnLines.Length; i++)
        {
            string l = cnLines[i];
            if (string.IsNullOrWhiteSpace(l)) continue;
            var parts = l.Split('\t');
            if (parts.Length >= 2)
            {
                string cn = parts[0];
                string en = parts[1];
                string cleanCn = CleanLuaKey(cn);
                string cleanEn = CleanLuaKey(en);

                if (!keyToLineIndex.ContainsKey(cleanCn) && !pendingAppends.ContainsKey(cleanCn))
                {
                    if (existingDict.ContainsKey(cleanEn))
                    {
                        pendingAppends[cleanCn] = existingDict[cleanEn];
                    }
                }
            }
        }

        // 5. Append new keys right before final closing brace
        Console.WriteLine(string.Format("\n[5/5] Appending {0} new unique keys to file...", pendingAppends.Count));
        int lastBrace = fileLines.FindLastIndex(x => x.Trim() == "}");
        if (lastBrace < 0) lastBrace = fileLines.Count;

        var appendLines = new List<string>();
        foreach (var kvp in pendingAppends)
        {
            appendLines.Add(string.Format("    [\"{0}\"] = \"{1}\",", kvp.Key, kvp.Value));
        }
        fileLines.InsertRange(lastBrace, appendLines);

        // Update header count
        int totalDictCount = keyToLineIndex.Count + pendingAppends.Count;
        for (int i = 0; i < Math.Min(5, fileLines.Count); i++)
        {
            if (fileLines[i].StartsWith("-- Entries:"))
            {
                fileLines[i] = string.Format("-- Entries: {0}", totalDictCount);
                break;
            }
        }

        Console.WriteLine(string.Format("Writing {0} total lines to {1}...", fileLines.Count, ruPath));
        File.WriteAllLines(ruPath, fileLines, new UTF8Encoding(false));
        Console.WriteLine("Dictionary successfully updated and saved!");
    }

    static void ApplyTranslation(List<string> fileLines, Dictionary<string, int> keyToLineIndex, Dictionary<string, string> pendingAppends, Dictionary<string, string> existingDict, string cn, string en, string ru)
    {
        string cleanCn = CleanLuaKey(cn);
        string cleanEn = CleanLuaKey(en);
        string cleanRu = CleanLuaValue(ru);

        // Update EN key
        if (keyToLineIndex.ContainsKey(cleanEn))
        {
            fileLines[keyToLineIndex[cleanEn]] = string.Format("    [\"{0}\"] = \"{1}\",", cleanEn, cleanRu);
        }
        else
        {
            pendingAppends[cleanEn] = cleanRu;
        }
        existingDict[cleanEn] = cleanRu;

        // Update CN key
        if (keyToLineIndex.ContainsKey(cleanCn))
        {
            fileLines[keyToLineIndex[cleanCn]] = string.Format("    [\"{0}\"] = \"{1}\",", cleanCn, cleanRu);
        }
        else
        {
            pendingAppends[cleanCn] = cleanRu;
        }
        existingDict[cleanCn] = cleanRu;
    }

    static string CleanLuaKey(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\n").Replace("\r", "");
        r = r.Replace("\\n", "\n").Replace("\\t", "\t");
        r = r.Replace("\\\"", "\"");
        r = r.Replace("\"", "\\\"");
        r = r.Replace("\n", "\\n").Replace("\t", "\\t");
        return r;
    }

    static string CleanLuaValue(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\n").Replace("\r", "");
        r = r.Replace("\\n", "\n").Replace("\\t", "\t");
        r = r.Replace("\\\"", "\"");
        r = r.Replace("\"", "\\\"");
        r = r.Replace("\n", "\\n").Replace("\t", "\\t");
        return r;
    }

    static string TranslateMultiEndpoint(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        var tagMap = new Dictionary<string, string>();
        int tagIdx = 0;
        string protectedText = Regex.Replace(text, @"<[^>]+>|\{[^{}]+\}|\\n|\*d\*\*|\*d|\*f\*\*|\*f|#CanMove[^#]+#", m =>
        {
            string ph = "XTAG" + (tagIdx++) + "X";
            tagMap[ph] = m.Value;
            return ph;
        });

        string result = null;

        for (int attempt = 0; attempt < 3; attempt++)
        {
            // Attempt 1: clients5.google.com
            try
            {
                string url = "https://clients5.google.com/translate_a/t?client=dict-chrome-ex&sl=en&tl=ru&q=" + Uri.EscapeDataString(protectedText);
                using (var wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    string json = wc.DownloadString(url).Trim();
                    if (json.StartsWith("[\"") && json.EndsWith("\"]"))
                    {
                        result = json.Substring(2, json.Length - 4);
                        result = Regex.Unescape(result);
                    }
                }
            }
            catch { }

            // Attempt 2: translate.google.com/m mobile
            if (string.IsNullOrWhiteSpace(result) || result.Trim() == protectedText.Trim())
            {
                try
                {
                    string url = "https://translate.google.com/m?sl=en&tl=ru&hl=ru&q=" + Uri.EscapeDataString(protectedText);
                    using (var wc = new WebClient())
                    {
                        wc.Encoding = Encoding.UTF8;
                        wc.Headers.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X)");
                        string html = wc.DownloadString(url);
                        int idx = html.IndexOf("class=\"result-container\">");
                        if (idx > 0)
                        {
                            int end = html.IndexOf("</div>", idx);
                            string t = html.Substring(idx + 25, end - (idx + 25));
                            result = WebUtility.HtmlDecode(t);
                        }
                    }
                }
                catch { }
            }

            if (!string.IsNullOrWhiteSpace(result) && result.Trim() != protectedText.Trim())
            {
                break;
            }
            Thread.Sleep(200 * (attempt + 1));
        }

        if (string.IsNullOrWhiteSpace(result)) return text;

        result = Regex.Replace(result, @"[a-f0-9]{32}", "");
        foreach (var kvp in tagMap)
        {
            result = result.Replace(kvp.Key, kvp.Value);
        }

        return result.Trim();
    }
}
