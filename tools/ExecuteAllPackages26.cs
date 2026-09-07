using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class ExecuteAllPackages26
{
    static readonly Dictionary<string, string> CanonMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "The Fool", "Шут" },
        { "Lord of Mysteries", "Повелитель Тайн" },
        { "Lord of the Mysteries", "Повелитель Тайн" },
        { "Beyonder Characteristic", "Потустороннее свойство" },
        { "Beyonder Characteristics", "Потусторонние свойства" },
        { "Beyonder Characteristic:", "Потустороннее свойство:" },
        { "Beyonder Material", "Потусторонний материал" },
        { "Beyonder Materials", "Потусторонние материалы" },
        { "Beyonder", "Потусторонний" },
        { "Beyonders", "Потусторонние" },
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "Spirit Body Threads", "Нити духовного тела" },
        { "Spirit Body Thread", "Нить духовного тела" },
        { "Spirit Body", "Духовное тело" },
        { "Historical Projection", "Историческая проекция" },
        { "Historical Projections", "Исторические проекции" },
        { "Paper Figurine Substitute", "Замена бумажным человечком" },
        { "Paper Figurine Substitutes", "Замена бумажным человечком" },
        { "Flame Controlling", "Управление пламенем" },
        { "Tarot Club", "Клуб Таро" },
        { "Nighthawks", "Ночные Ястребы" },
        { "Blackthorn Security Company", "Охранная компания «Чёрный Чертополох»" },
        { "Blackthorn", "Чёрный Чертополох" },
        { "Gold Pound", "золотой фунт" },
        { "Gold Pounds", "золотых фунтов" },
        { "Soli", "суле" },
        { "Sule", "суле" },
        { "Pence", "пенсов" },
        { "Tingen", "Тинген" },
        { "Backlund", "Бэкланд" },
        { "Damage Reduction", "Снижение урона" },
        { "Super Armor", "Суперброня" },
        { "Cleanse", "Снятие контроля" },
        { "Cooldown Reduction", "Сокращение перезарядки" },
        { "Cooldown", "Перезарядка" },
        { "Mind Fire", "Пламя разума" },
        { "Induction Mark", "Метка внушения" },
        { "Hypnosis", "Гипноз" },
        { "Imprisonment", "Заточение" },
        { "Old Neil", "Старина Нил" },
        { "Dunn Smith", "Данн Смит" },
        { "Leonard Mitchell", "Леонард Митчелл" },
        { "Melissa Moretti", "Мелисса Моретти" },
        { "Benson Moretti", "Бенсон Моретти" },
        { "Daly Simone", "Дейли Симон" },
        { "Audrey Hall", "Одри Холл" },
        { "Alger Wilson", "Алджер Уилсон" },
        { "Derrick Berg", "Деррик Берг" },
        { "Roselle Gustav", "Розель Густав" },
        { "Emperor Roselle", "Император Розель" },
        { "Klein Moretti", "Клейн Моретти" },
        { "Klein", "Клейн" }
    };

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;

        string rootDir = @"d:\gameDev\translate lotm";
        string ruPath = Path.Combine(rootDir, "RuntimeTextRussian.lua");
        string nonCyrPath = Path.Combine(rootDir, "tools", "non_cyrillic_ru_strings.txt");
        string mod35Path = Path.Combine(rootDir, "tools", "modified_35.txt");
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");

        Console.WriteLine("========================================================================");
        Console.WriteLine("     EXECUTING LOTM PATCH v2.6.0 TRANSLATION & INTEGRATION ROADMAP      ");
        Console.WriteLine("========================================================================");

        // 1. Load current RuntimeTextRussian.lua
        Console.WriteLine("\n[1/5] Loading RuntimeTextRussian.lua...");
        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        using (var reader = new StreamReader(ruPath, Encoding.UTF8))
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
                        string k = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string v = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";
                        string unescapedKey = Unescape(k);
                        string unescapedVal = Unescape(v);
                        ruDict[unescapedKey] = unescapedVal;
                    }
                }
            }
        }
        Console.WriteLine(string.Format("Loaded {0} existing entries in RuntimeTextRussian.lua.", ruDict.Count));

        // 2. Package 2.6-A: Process 496 non-cyrillic strings
        Console.WriteLine("\n[2/5] Package 2.6-A: Translating non-cyrillic entries...");
        ProcessPackage26A(nonCyrPath, ruDict);

        // Fix the 4 escaping strings (with literal backslashes and double-escapes)
        FixEscapingEntries(ruDict);

        // 3. Package 2.6-B: Apply 35 modified strings and canonicalize dialogues
        Console.WriteLine("\n[3/5] Package 2.6-B: Applying 35 modified strings & gender macros...");
        Apply35Modifications(mod35Path, ruDict);

        // 4. Package 2.6-C: Canonize items, combat terms, and fix slangs
        Console.WriteLine("\n[4/5] Package 2.6-C: Canonical post-processing of all entries...");
        CanonicalPostProcess(ruDict);

        // 5. Save updated RuntimeTextRussian.lua
        Console.WriteLine("\n[5/5] Saving updated RuntimeTextRussian.lua...");
        SaveDictionary(ruPath, ruDict);

        Console.WriteLine("\nExecution completed successfully! Proceeding to validation and shard generation.");
    }

    static void ProcessPackage26A(string nonCyrPath, Dictionary<string, string> ruDict)
    {
        var lines = File.ReadAllLines(nonCyrPath, Encoding.UTF8);
        var entriesToTranslate = new List<Tuple<string, string, string>>(); // CN, EN, RU

        string curCn = "", curEn = "", curRu = "", curIdx = "";
        Action addEntry = () =>
        {
            if (string.IsNullOrEmpty(curIdx)) return;
            if (!Regex.IsMatch(curRu, @"[\u0400-\u04FF]"))
            {
                entriesToTranslate.Add(Tuple.Create(curCn, curEn, curRu));
            }
        };

        foreach (var l in lines)
        {
            var mIdx = Regex.Match(l, @"^\[(\d+)\]\s*CN:\s*(.*)");
            if (mIdx.Success)
            {
                addEntry();
                curIdx = mIdx.Groups[1].Value;
                curCn = mIdx.Groups[2].Value;
                curEn = "";
                curRu = "";
                continue;
            }
            var mEn = Regex.Match(l, @"^\s*EN:\s*(.*)");
            if (mEn.Success) { curEn = mEn.Groups[1].Value; continue; }
            var mRu = Regex.Match(l, @"^\s*RU:\s*(.*)");
            if (mRu.Success) { curRu = mRu.Groups[1].Value; continue; }
        }
        addEntry();

        Console.WriteLine(string.Format("Found {0} non-cyrillic entries in {1}", entriesToTranslate.Count, Path.GetFileName(nonCyrPath)));

        var translated = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        int total = entriesToTranslate.Count;
        int completed = 0;

        Parallel.ForEach(entriesToTranslate, new ParallelOptions { MaxDegreeOfParallelism = 10 }, item =>
        {
            string cn = item.Item1;
            string en = item.Item2;
            string ru = TranslateSingleWithCanon(en, cn);

            translated[cn] = ru;
            translated[en] = ru;

            int c = Interlocked.Increment(ref completed);
            if (c % 50 == 0 || c == total)
            {
                Console.WriteLine(string.Format("  Package 2.6-A progress: {0}/{1} ({2:F1}%)", c, total, (double)c / total * 100.0));
            }
        });

        int updatedCount = 0;
        foreach (var kvp in translated)
        {
            ruDict[kvp.Key] = kvp.Value;
            updatedCount++;
        }
        Console.WriteLine(string.Format("Updated {0} keys (CN + EN) in dictionary from Package 2.6-A.", updatedCount));
    }

    static void FixEscapingEntries(Dictionary<string, string> ruDict)
    {
        Console.WriteLine("Fixing HyperLink/Clickable escaping entries...");

        string k1_cn = "<HyperLink stylename=\\\"Chat_Task\\\" u=\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\">点击帮助</>";
        string k1_en = "<HyperLink stylename=\\\"Chat_Task\\\" u=\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\"> Click for help </>";
        string val1 = "<HyperLink stylename=\\\"Chat_Task\\\" u=\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\"> Нажмите для помощи </>";

        string k2_cn = "击杀<HyperLink stylename=\\\"Clickable\\\" u=\\\"\\\">守卫者</>概率掉落";
        string k2_en = "Killing <HyperLink stylename=\\\"Clickable\\\" u=\\\"\\\"> guardian </> has a chance to drop";
        string val2 = "Шанс выпадения при убийстве <HyperLink stylename=\\\"Clickable\\\" u=\\\"\\\">Стража</>";

        ruDict[k1_cn] = val1;
        ruDict[k1_en] = val1;
        ruDict[k2_cn] = val2;
        ruDict[k2_en] = val2;

        // Also add standard unescaped quote versions
        string k1_cn_clean = "<HyperLink stylename=\"Chat_Task\" u=\"guildTaskHelp=%s,%d,%s,%s,%s,%s\">点击帮助</>";
        string k1_en_clean = "<HyperLink stylename=\"Chat_Task\" u=\"guildTaskHelp=%s,%d,%s,%s,%s,%s\"> Click for help </>";
        string val1_clean = "<HyperLink stylename=\"Chat_Task\" u=\"guildTaskHelp=%s,%d,%s,%s,%s,%s\"> Нажмите для помощи </>";

        string k2_cn_clean = "击杀<HyperLink stylename=\"Clickable\" u=\"\">守卫者</>概率掉落";
        string k2_en_clean = "Killing <HyperLink stylename=\"Clickable\" u=\"\"> guardian </> has a chance to drop";
        string val2_clean = "Шанс выпадения при убийстве <HyperLink stylename=\"Clickable\" u=\"\">Стража</>";

        ruDict[k1_cn_clean] = val1_clean;
        ruDict[k1_en_clean] = val1_clean;
        ruDict[k2_cn_clean] = val2_clean;
        ruDict[k2_en_clean] = val2_clean;

        Console.WriteLine("Escaping keys successfully registered for both \\\" and \" variants.");
    }

    static void Apply35Modifications(string mod35Path, Dictionary<string, string> ruDict)
    {
        var lines = File.ReadAllLines(mod35Path, Encoding.UTF8);
        string curCn = "", curOldEn = "", curNewEn = "", curRu = "";
        int applied = 0;

        Action applyEntry = () =>
        {
            if (string.IsNullOrEmpty(curCn)) return;
            string bestRu = curRu;

            // Fix broken gender macros {Mr.{ Ms.|}} or {Mr.{Ms.|}} into {{мистер|мисс}}
            bestRu = Regex.Replace(bestRu, @"\{Mr\.\{\s*Ms\.\|\}\}", "{{мистер|мисс}}");
            bestRu = Regex.Replace(bestRu, @"\{Mr\.\{Ms\.\|\}\}", "{{мистер|мисс}}");
            bestRu = Regex.Replace(bestRu, @"\{\{Mr\.\s*\|\s*Ms\.\}\}", "{{мистер|мисс}}");

            // Fix tags
            bestRu = bestRu.Replace("<h>Lise Evans</h>", "<h>Лиз Эванс</>");
            bestRu = bestRu.Replace("<Mark id=\"#159\">flute </mark>", "<Mark id=\"#159\">флейты </>");

            // Apply Canon corrections
            bestRu = CanonizeText(bestRu);

            ruDict[curCn] = bestRu;
            ruDict[curNewEn] = bestRu;
            if (!string.IsNullOrEmpty(curOldEn)) ruDict[curOldEn] = bestRu;
            applied++;
        };

        foreach (var l in lines)
        {
            if (l.StartsWith("=== ["))
            {
                applyEntry();
                curCn = ""; curOldEn = ""; curNewEn = ""; curRu = "";
                continue;
            }
            if (l.StartsWith("CN:     ")) curCn = Unescape(l.Substring(8).Trim());
            else if (l.StartsWith("Old EN: ")) curOldEn = Unescape(l.Substring(8).Trim());
            else if (l.StartsWith("New EN: ")) curNewEn = Unescape(l.Substring(8).Trim());
            else if (l.StartsWith("Cur RU: ")) curRu = Unescape(l.Substring(8).Trim());
        }
        applyEntry();

        Console.WriteLine(string.Format("Successfully updated {0} modified entries with canonical gender macros & tags.", applied));
    }

    static void CanonicalPostProcess(Dictionary<string, string> ruDict)
    {
        var keys = new List<string>(ruDict.Keys);
        int canonized = 0;

        foreach (var k in keys)
        {
            string val = ruDict[k];
            string newVal = CanonizeText(val);
            if (newVal != val)
            {
                ruDict[k] = newVal;
                canonized++;
            }
        }
        Console.WriteLine(string.Format("Canonicalized {0} entries across the entire dictionary.", canonized));
    }

    static string CanonizeText(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        // Strictly канон: Клейн, never Кляйн
        s = Regex.Replace(s, @"\bКляйн\b", "Клейн");
        s = Regex.Replace(s, @"\bКляйна\b", "Клейна");
        s = Regex.Replace(s, @"\bКляйну\b", "Клейну");
        s = Regex.Replace(s, @"\bКляйном\b", "Клейном");
        s = Regex.Replace(s, @"\bКляйне\b", "Клейне");

        // Strictly Старина Нил, never Старый Нил
        s = s.Replace("Старый Нил", "Старина Нил");
        s = s.Replace("Старого Нила", "Старины Нила");
        s = s.Replace("Старому Нилу", "Старине Нилу");
        s = s.Replace("Старым Нилом", "Стариной Нилом");

        // Strictly суле, never солей/соля
        s = Regex.Replace(s, @"\bсолей\b", "суле");
        s = Regex.Replace(s, @"\bсоля\b", "суле");

        // Strictly Потустороннее свойство, never Потусторонняя характеристика
        s = s.Replace("Потусторонняя характеристика", "Потустороннее свойство");
        s = s.Replace("Потусторонние характеристики", "Потусторонние свойства");
        s = s.Replace("Потусторонней характеристики", "Потустороннего свойства");
        s = s.Replace("Потусторонних характеристик", "Потусторонних свойств");
        s = s.Replace("Потустороннюю характеристику", "Потустороннее свойство");

        // Strictly Охранная компания «Чёрный Чертополох»
        s = s.Replace("Охранная компания Блэкторн", "Охранная компания «Чёрный Чертополох»");
        s = s.Replace("Охранная компания «Блэкторн»", "Охранная компания «Чёрный Чертополох»");
        s = s.Replace("Охранной компании Блэкторн", "Охранной компании «Чёрный Чертополох»");
        s = s.Replace("Охранную компанию Блэкторн", "Охранную компанию «Чёрный Чертополох»");
        s = Regex.Replace(s, @"\bБлэкторн\b", "Чёрный Чертополох");

        // Gaming slang machine fixes
        s = s.Replace("мой выход выше вашего", "мой нанесённый урон выше твоего");
        s = s.Replace("мой выход выше твоего", "мой нанесённый урон выше твоего");
        s = s.Replace("выход урона", "нанесённый урон");
        s = s.Replace("skewering", "сбор группы");

        // Clean broken escapes like \п -> \n
        s = s.Replace("\\п", "\\n").Replace("\\т", "\\t");

        return s;
    }

    static string TranslateSingleWithCanon(string text, string cnText)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        if (Regex.IsMatch(text.Trim(), @"^[\d\s\.,:%/\\~_\-\+\*\(\)\#\[\]\|]+$"))
        {
            // Pure punctuation/numbers/formatting: return as-is
            return text;
        }

        // Check specific known patterns
        if (cnText == "歌曲名字歌曲名字" || text == "Song Name Song Name") return "Название песни Название песни";
        if (cnText == "当前评分" || text == "Current rating") return "Текущий рейтинг";
        if (cnText == "来来来" || text == "Come, come, come.") return "Идём, идём, скорее.";
        if (cnText == "人物小传（一）" || text == "Character Biography (I)") return "Биография персонажа (I)";
        if (cnText == "解锁条件解锁条件解锁") return "Условие открытия Условие открытия Открыть";
        if (cnText == "奖励任务" || text == "Reward Quest") return "Задание с наградой";
        if (cnText == "我的金榜" || text == "My Gold List") return "Мой золотой список";
        if (cnText == "错帧动画" || text == "Staggered Frame Animation") return "Анимация ступенчатого кадра";
        if (cnText == "前往觉醒系统" || text == "Go to Awakening system") return "Перейти к системе Пробуждения";
        if (cnText == "成员权限" || text == "Member Permissions") return "Права участников";
        if (cnText == "换攻击试一下" || text == "Change to Attack and try.") return "Переключитесь на атаку и попробуйте.";
        if (cnText == "文本内容..." || text == "Text content...") return "Текстовое содержимое...";
        if (cnText == "……该死。" || text == "...Damn it.") return "...Проклятье.";

        var tagMap = new Dictionary<string, string>();
        int tagIdx = 0;
        string protectedText = Regex.Replace(text, @"<[^>]+>|\{[^{}]+\}|\\r\\n|\\n|\*d\*\*|\*d|\*f\*\*|\*f|#CanMove[^#]+#", m =>
        {
            string ph = "XTAG" + (tagIdx++) + "X";
            tagMap[ph] = m.Value;
            return ph;
        });

        string translated = null;
        for (int attempt = 0; attempt < 3; attempt++)
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

                    string res = sb.ToString();
                    res = Regex.Replace(res, @"[a-f0-9]{32}", "");
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        foreach (var kvp in tagMap) res = res.Replace(kvp.Key, kvp.Value);
                        translated = res.Trim();
                        break;
                    }
                }
            }
            catch
            {
                Thread.Sleep(200 * (attempt + 1));
            }
        }

        if (string.IsNullOrEmpty(translated) || translated == protectedText)
        {
            translated = text;
        }

        // Apply Canon Replacements
        foreach (var kvp in CanonMap)
        {
            translated = Regex.Replace(translated, @"\b" + Regex.Escape(kvp.Key) + @"\b", kvp.Value, RegexOptions.IgnoreCase);
        }

        translated = CanonizeText(translated);
        return translated;
    }

    static void SaveDictionary(string path, Dictionary<string, string> dict)
    {
        string bakPath = path + ".bak_26";
        if (!File.Exists(bakPath)) File.Copy(path, bakPath);

        using (var sw = new StreamWriter(path, false, Encoding.UTF8))
        {
            sw.WriteLine("-- Generated Russian Translation Dictionary for Lord of Mysteries");
            sw.WriteLine(string.Format("-- Total entries: {0}", dict.Count));
            sw.WriteLine("return {");
            foreach (var kvp in dict)
            {
                string key = CleanForLua(kvp.Key);
                string val = CleanForLua(kvp.Value);
                sw.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", key, val));
            }
            sw.WriteLine("}");
        }
        Console.WriteLine(string.Format("Successfully saved {0} entries to {1}.", dict.Count, path));
    }

    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "");
        // If s contains literal \" from source, preserve it, but prevent invalid unescaped quotes
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
    }

    static string Unescape(string s)
    {
        return s.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\\", "\\");
    }
}
