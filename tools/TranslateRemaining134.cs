using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

class TranslateRemaining134
{
    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "The Fool", "Шут" },
        { "Lord of Mysteries", "Повелитель Тайн" },
        { "Lord of the Mysteries", "Повелитель Тайн" },
        { "Evernight Goddess", "Богиня Вечной Ночи" },
        { "Lord of Storms", "Владыка Шторма" },
        { "God of Steam and Machinery", "Бог Пара и Машин" },
        { "Mother Earth", "Мать-Земля" },
        { "True Creator", "Истинный Творец" },
        { "Original Creator", "Изначальный Творец" },
        { "Great Old Ones", "Великие Древние" },
        { "Above the Grey Fog", "Над Серым Туманом" },
        { "Grey Fog", "Серый Туман" },
        { "Tarot Club", "Клуб Таро" },
        { "Sealed Artifact", "Запечатанный артефакт" },
        { "Sealed Artifacts", "Запечатанные артефакты" },
        { "Beyonder", "Потусторонний" },
        { "Beyonders", "Потусторонние" },
        { "Beyonder Characteristic", "Потустороннее свойство" },
        { "Beyonder Characteristics", "Потусторонние свойства" },
        { "Spirit Body Threads", "Нити духовного тела" },
        { "Historical Projection", "Историческая проекция" },
        { "Paper Figurine Substitute", "Замена бумажным человечком" },
        { "Flame Controlling", "Управление пламенем" },
        { "Acting Method", "Метод Лицедейства" },
        { "Digestion", "Усвоение" },
        { "Lose Control", "Потерять контроль" },
        { "Loss of Control", "Потеря контроля" },
        { "Madness", "Безумие" },
        { "Sanity", "Рассудок" },
        { "Spirituality", "Духовность" },
        { "Spirit Body", "Духовное тело" },
        { "Divination", "Гадание" },
        { "Marionette", "Марионетка" },
        { "Marionettes", "Марионетки" },
        { "Nighthawks", "Ночные Ястребы" },
        { "Tingen", "Тинген" },
        { "Backlund", "Бэкланд" },
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
        { "Gold Pound", "золотой фунт" },
        { "Gold Pounds", "золотых фунтов" },
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
        { "Susie", "Сузи" },
        { "Howes Street", "улица Хоуэс" },
        { "Howls Street", "улица Хоуэс" },
        { "Daffodil Street", "улица Нарциссов" },
        { "Iron Cross Street", "улица Железного Креста" },
        { "Zouteland Street", "улица Зотланд" }
    };

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;

        string rootDir = @"d:\gameDev\translate lotm";
        string shards26Dir = Path.Combine(rootDir, @"source_en_2.6\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes");
        string ruMasterFile = Path.Combine(rootDir, @"RuntimeTextRussian.lua");

        Console.WriteLine("Loading all 1,024 shards from 2.6...");
        var en26Entries = new Dictionary<string, string>(StringComparer.Ordinal);
        var shardFiles = Directory.GetFiles(shards26Dir, "RuntimeTextGemini_*.lua");
        Array.Sort(shardFiles);
        foreach (var f in shardFiles) ParseLuaTable(f, en26Entries);
        Console.WriteLine("Loaded " + en26Entries.Count + " entries from 2.6 shards.");

        Console.WriteLine("Loading RuntimeTextRussian.lua...");
        var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
        ParseLuaTable(ruMasterFile, ruDict);
        Console.WriteLine("Loaded " + ruDict.Count + " existing entries.");

        int syncedCn = 0;
        int syncedEn = 0;
        int translatedNew = 0;

        foreach (var kvp in en26Entries)
        {
            string cn = kvp.Key;
            string en = kvp.Value;

            bool hasCn = ruDict.ContainsKey(cn);
            bool hasEn = ruDict.ContainsKey(en);

            if (hasCn && hasEn) continue;

            if (hasCn && !hasEn)
            {
                ruDict[en] = ruDict[cn];
                syncedEn++;
                continue;
            }

            if (!hasCn && hasEn)
            {
                ruDict[cn] = ruDict[en];
                syncedCn++;
                continue;
            }

            // Neither is present: translate
            string ru = TranslateEntry(cn, en);
            ruDict[cn] = ru;
            ruDict[en] = ru;
            translatedNew++;
            Console.WriteLine(string.Format("[{0}] Translated: {1} -> {2}", translatedNew, Escape(en), Escape(ru)));
        }

        Console.WriteLine(string.Format("Synced missing CN: {0}, Synced missing EN: {1}, Translated newly: {2}", syncedCn, syncedEn, translatedNew));

        // Write back to RuntimeTextRussian.lua
        Console.WriteLine("Writing updated RuntimeTextRussian.lua...");
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
    }

    static string TranslateEntry(string cn, string en)
    {
        // Check for dev text
        if (cn.Contains("占位") || en.IndexOf("placeholder", StringComparison.OrdinalIgnoreCase) >= 0) return "[Заглушка]";
        if (cn.Contains("文本文本")) return "Текст текст текст";

        string trans = TranslateGoogle(en);
        trans = ApplyCanon(trans);
        trans = PostProcess(trans);
        return trans;
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

    static string PostProcess(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        return text.Replace("Кляйн", "Клейн")
                   .Replace("Блэкторн", "Чёрный Чертополох")
                   .Replace("Старый Нил", "Старина Нил")
                   .Replace("солей", "суле")
                   .Replace("соля", "суле")
                   .Replace("характеристика", "свойство")
                   .Replace("Характеристика", "Свойство")
                   .Replace("{{Mr.|Ms.}}", "{{мистер|мисс}}")
                   .Replace("{Mr.{ Ms.|}}", "{{мистер|мисс}}")
                   .Replace("{Mr.{Ms.|}}", "{{мистер|мисс}}");
    }

    static string TranslateGoogle(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        var tagMap = new Dictionary<string, string>();
        int tagIdx = 0;
        string protectedText = Regex.Replace(text, @"<[^>]+>|\{\{[^}]+\}\}|\{[^{}]+\}|\r\n|\n|\r|\\n|\\r|\\t|\*d\*\*|\*d|\*f\*\*|\*f|mul\([^)]+\)|spellfielddisc\([^)]+\)|buffdisc\([^)]+\)|bulletdisc\([^)]+\)|buffappear\([^)]+\)|CheckStar\([^)]+\)|%s|%d|%i|%f", m =>
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
                    result = Regex.Replace(result, @"[a-f0-9]{32}", "").Trim();
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
        string r = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "").Replace("\t", "\\t");
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
    }

    static string Escape(string s)
    {
        if (s == null) return "";
        return s.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }
}
