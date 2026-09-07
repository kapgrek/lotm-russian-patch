using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class TranslateNonCyrillicBatch
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
        { "Klein", "Клейн" },
        { "Mr. Paul", "мистер Пол" },
        { "Amelia Fuller", "Амелия Фуллер" }
    };

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;

        string inTsv = @"d:\gameDev\translate lotm\tools\non_cyrillic_to_translate.tsv";
        string outTsv = @"d:\gameDev\translate lotm\tools\non_cyrillic_translated.tsv";

        var lines = File.ReadAllLines(inTsv, Encoding.UTF8);
        var entries = new List<Tuple<string, string, string, string>>(); // Id, CN, EN, RU

        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split('\t');
            if (parts.Length >= 4)
            {
                entries.Add(Tuple.Create(parts[0], parts[1], parts[2], parts[3]));
            }
        }

        Console.WriteLine(string.Format("Loaded {0} entries from {1}", entries.Count, inTsv));

        var results = new ConcurrentDictionary<string, string>(); // Id -> translated RU
        int completed = 0;
        int total = entries.Count;

        Parallel.ForEach(entries, new ParallelOptions { MaxDegreeOfParallelism = 10 }, item =>
        {
            string id = item.Item1;
            string cn = item.Item2;
            string en = item.Item3;
            string ru = item.Item4;

            string finalRu = TranslateEntry(cn, en, ru);
            results[id] = finalRu;

            int c = Interlocked.Increment(ref completed);
            if (c % 50 == 0 || c == total)
            {
                Console.WriteLine(string.Format("Progress: {0}/{1} ({2:F1}%)", c, total, (double)c / total * 100.0));
            }
        });

        using (var sw = new StreamWriter(outTsv, false, Encoding.UTF8))
        {
            sw.WriteLine("Id\tCN\tEN\tRU_Old\tRU_New");
            foreach (var item in entries)
            {
                string id = item.Item1;
                string newRu = results.ContainsKey(id) ? results[id] : item.Item4;
                sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", item.Item1, item.Item2, item.Item3, item.Item4, newRu));
            }
        }
        Console.WriteLine(string.Format("Saved all translated results to {0}", outTsv));
    }

    static string TranslateEntry(string cn, string en, string curRu)
    {
        if (string.IsNullOrWhiteSpace(en)) return cn;

        // 1. Check pure numbers/punctuation
        if (Regex.IsMatch(en.Trim(), @"^[\d\s\.,:%/\\~_\-\+\*\(\)\#\[\]\|]+$"))
        {
            return curRu;
        }

        // 2. Custom curated replacements
        if (cn == "歌曲名字歌曲名字" || en == "Song Name Song Name") return "Название песни Название песни";
        if (cn == "当前评分" || en == "Current rating") return "Текущий рейтинг";
        if (cn == "来来来" || en == "Come, come, come.") return "Идём, идём, скорее.";
        if (cn == "人物小传（一）" || en == "Character Biography (I)") return "Биография персонажа (I)";
        if (cn == "解锁条件解锁条件解锁") return "Условие открытия Условие открытия Открыть";
        if (cn == "奖励任务" || en == "Reward Quest") return "Задание с наградой";
        if (cn == "我的金榜" || en == "My Gold List") return "Мой золотой список";
        if (cn == "错帧动画" || en == "Staggered Frame Animation") return "Анимация ступенчатого кадра";
        if (cn == "前往觉醒系统" || en == "Go to Awakening system") return "Перейти к системе Пробуждения";
        if (cn == "成员权限" || en == "Member Permissions") return "Права участников";
        if (cn == "换攻击试一下" || en == "Change to Attack and try.") return "Переключитесь на атаку и попробуйте.";
        if (cn == "文本内容..." || en == "Text content...") return "Текстовое содержимое...";
        if (cn == "……该死。" || en == "...Damn it.") return "...Проклятье.";
        if (cn.Contains("有瓜吗")) return "Есть сплетни? Есть сплетни? Есть сплетни? Расскажите что-нибудь новенькое!";
        if (cn.Contains("组队通关了200次副本文本上限二十个字")) return "Пройдено 200 раз в группе. Лимит двадцать символов.";
        if (en.StartsWith("Amelia Fuller-")) return en.Replace("Amelia Fuller-", "Амелия Фуллер-");
        if (en.StartsWith("Lucky Cat  —  Lv")) return en.Replace("Lucky Cat  —  Lv", "Кот удачи — Ур.").Replace("Warrior", "Воин").Replace("Inspect", "Осмотреть");

        // 3. Online translate with tags protection
        var tagMap = new Dictionary<string, string>();
        int tagIdx = 0;
        string protectedText = Regex.Replace(en, @"<[^>]+>|\{[^{}]+\}|\\r\\n|\\n|\*d\*\*|\*d|\*f\*\*|\*f|#CanMove[^#]+#", m =>
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
            translated = en;
        }

        // 4. Apply Canon replacements
        foreach (var kvp in CanonMap)
        {
            translated = Regex.Replace(translated, @"\b" + Regex.Escape(kvp.Key) + @"\b", kvp.Value, RegexOptions.IgnoreCase);
        }

        // Strict Canon rules
        translated = Regex.Replace(translated, @"\bКляйн\b", "Клейн");
        translated = Regex.Replace(translated, @"\bКляйна\b", "Клейна");
        translated = Regex.Replace(translated, @"\bКляйну\b", "Клейну");
        translated = Regex.Replace(translated, @"\bКляйном\b", "Клейном");
        translated = Regex.Replace(translated, @"\bКляйне\b", "Клейне");

        translated = translated.Replace("Старый Нил", "Старина Нил");
        translated = translated.Replace("Старого Нила", "Старины Нила");
        translated = translated.Replace("Старому Нилу", "Старине Нилу");

        translated = Regex.Replace(translated, @"\bсолей\b", "суле");
        translated = Regex.Replace(translated, @"\bсоля\b", "суле");

        translated = translated.Replace("Потусторонняя характеристика", "Потустороннее свойство");
        translated = translated.Replace("Потусторонние характеристики", "Потусторонние свойства");
        translated = translated.Replace("Охранная компания Блэкторн", "Охранная компания «Чёрный Чертополох»");
        translated = translated.Replace("Охранная компания «Блэкторн»", "Охранная компания «Чёрный Чертополох»");
        translated = Regex.Replace(translated, @"\bБлэкторн\b", "Чёрный Чертополох");

        translated = translated.Replace("мой выход выше вашего", "мой нанесённый урон выше твоего");
        translated = translated.Replace("мой выход выше твоего", "мой нанесённый урон выше твоего");
        translated = translated.Replace("выход урона", "нанесённый урон");

        return translated;
    }
}
