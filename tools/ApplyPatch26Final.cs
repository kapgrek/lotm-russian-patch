using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class ApplyPatch26Final
{
    static readonly Dictionary<string, string> CanonExact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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
        string rootDir = @"d:\gameDev\translate lotm";
        string ruPath = Path.Combine(rootDir, "RuntimeTextRussian.lua");
        string tsvPath = Path.Combine(rootDir, "tools", "non_cyrillic_translated.tsv");
        string mod35Path = Path.Combine(rootDir, "tools", "modified_35.txt");

        Console.WriteLine("========================================================================");
        Console.WriteLine("    APPLYING LOTM PATCH v2.6.0 FINAL INTEGRATION & LOCALIZATION        ");
        Console.WriteLine("========================================================================");

        // 1. Create a dictionary of updates (raw key -> raw val)
        var updates = new Dictionary<string, string>(StringComparer.Ordinal);

        // A. Load non_cyrillic_translated.tsv
        Console.WriteLine("\n[1/6] Loading refined non-cyrillic translations...");
        var tsvLines = File.ReadAllLines(tsvPath, Encoding.UTF8);
        int tsvCount = 0;
        for (int i = 1; i < tsvLines.Length; i++)
        {
            var p = tsvLines[i].Split('\t');
            if (p.Length >= 5)
            {
                string cn = Unescape(p[1]);
                string en = Unescape(p[2]);
                string ruNew = Unescape(p[4]);

                ruNew = CanonizeText(ruNew);

                updates[cn] = ruNew;
                updates[en] = ruNew;
                tsvCount++;
            }
        }
        Console.WriteLine(string.Format("Loaded {0} entries from {1}.", tsvCount, Path.GetFileName(tsvPath)));

        // B. Load 35 modified entries
        Console.WriteLine("\n[2/6] Loading 35 modified entries (gender macros & tags)...");
        var modLines = File.ReadAllLines(mod35Path, Encoding.UTF8);
        string curCn = "", curOldEn = "", curNewEn = "", curRu = "";
        int modCount = 0;

        Action applyMod = () =>
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

            bestRu = CanonizeText(bestRu);

            updates[curCn] = bestRu;
            updates[curNewEn] = bestRu;
            if (!string.IsNullOrEmpty(curOldEn)) updates[curOldEn] = bestRu;
            modCount++;
        };

        foreach (var l in modLines)
        {
            if (l.StartsWith("=== ["))
            {
                applyMod();
                curCn = ""; curOldEn = ""; curNewEn = ""; curRu = "";
                continue;
            }
            if (l.StartsWith("CN:     ")) curCn = Unescape(l.Substring(8).Trim());
            else if (l.StartsWith("Old EN: ")) curOldEn = Unescape(l.Substring(8).Trim());
            else if (l.StartsWith("New EN: ")) curNewEn = Unescape(l.Substring(8).Trim());
            else if (l.StartsWith("Cur RU: ")) curRu = Unescape(l.Substring(8).Trim());
        }
        applyMod();
        Console.WriteLine(string.Format("Loaded and prepared {0} modified entries.", modCount));

        // C. Escape fixes:
        // Key with literal backslash in key:
        string k1_cn_slash = "<HyperLink stylename=\\\"Chat_Task\\\" u=\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\">点击帮助</>";
        string k1_en_slash = "<HyperLink stylename=\\\"Chat_Task\\\" u=\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\"> Click for help </>";
        string v1_slash = "<HyperLink stylename=\\\"Chat_Task\\\" u=\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\"> Нажмите для помощи </>";

        string k2_cn_slash = "击杀<HyperLink stylename=\\\"Clickable\\\" u=\\\"\\\">守卫者</>概率掉落";
        string k2_en_slash = "Killing <HyperLink stylename=\\\"Clickable\\\" u=\\\"\\\"> guardian </> has a chance to drop";
        string v2_slash = "Шанс выпадения при убийстве <HyperLink stylename=\\\"Clickable\\\" u=\\\"\\\">Стража</>";

        // Key with normal quote in key:
        string k1_cn_norm = "<HyperLink stylename=\"Chat_Task\" u=\"guildTaskHelp=%s,%d,%s,%s,%s,%s\">点击帮助</>";
        string k1_en_norm = "<HyperLink stylename=\"Chat_Task\" u=\"guildTaskHelp=%s,%d,%s,%s,%s,%s\"> Click for help </>";
        string v1_norm = "<HyperLink stylename=\"Chat_Task\" u=\"guildTaskHelp=%s,%d,%s,%s,%s,%s\"> Нажмите для помощи </>";

        string k2_cn_norm = "击杀<HyperLink stylename=\"Clickable\" u=\"\">守卫者</>概率掉落";
        string k2_en_norm = "Killing <HyperLink stylename=\"Clickable\" u=\"\"> guardian </> has a chance to drop";
        string v2_norm = "Шанс выпадения при убийстве <HyperLink stylename=\"Clickable\" u=\"\">Стража</>";

        updates[k1_cn_slash] = v1_slash;
        updates[k1_en_slash] = v1_slash;
        updates[k2_cn_slash] = v2_slash;
        updates[k2_en_slash] = v2_slash;

        updates[k1_cn_norm] = v1_norm;
        updates[k1_en_norm] = v1_norm;
        updates[k2_cn_norm] = v2_norm;
        updates[k2_en_norm] = v2_norm;

        Console.WriteLine(string.Format("Total unique keys in update batch: {0}", updates.Count));

        // 2. Read existing RuntimeTextRussian.lua, update in-place
        Console.WriteLine("\n[3/6] Applying updates to RuntimeTextRussian.lua...");
        string bakRu = ruPath + ".bak_v1331";
        if (!File.Exists(bakRu))
        {
            File.Copy(ruPath, bakRu);
            Console.WriteLine("Created backup at " + bakRu);
        }

        var existingLines = File.ReadAllLines(ruPath, Encoding.UTF8);
        var newLines = new List<string>(existingLines.Length + 100);
        var appliedKeys = new HashSet<string>(StringComparer.Ordinal);
        int linesUpdated = 0;

        for (int i = 0; i < existingLines.Length; i++)
        {
            string line = existingLines[i];
            string t = line.Trim();

            if (t == "}" || t == "};")
            {
                // Before closing bracket, append any new keys from updates that weren't present
                int appended = 0;
                foreach (var kvp in updates)
                {
                    if (!appliedKeys.Contains(kvp.Key))
                    {
                        string serK = Serialize(kvp.Key);
                        string serV = Serialize(kvp.Value);
                        newLines.Add(string.Format("    [\"{0}\"] = \"{1}\",", serK, serV));
                        appliedKeys.Add(kvp.Key);
                        appended++;
                    }
                }
                Console.WriteLine(string.Format("Appended {0} new entries before table closure.", appended));
                newLines.Add(line);
                continue;
            }

            if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
            {
                int delim = t.IndexOf("\"] = \"");
                if (delim > 0)
                {
                    string kEsc = t.Substring(2, delim - 2);
                    int valStart = delim + 6;
                    int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                    string vEsc = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                    string rawK = Unescape(kEsc);
                    string rawV = Unescape(vEsc);

                    appliedKeys.Add(rawK);

                    string newV;
                    if (updates.TryGetValue(rawK, out newV))
                    {
                        newLines.Add(string.Format("    [\"{0}\"] = \"{1}\",", kEsc, Serialize(newV)));
                        linesUpdated++;
                        continue;
                    }
                    else
                    {
                        // Check canon cleanup on existing line
                        string canonV = CanonizeText(rawV);
                        if (canonV != rawV)
                        {
                            newLines.Add(string.Format("    [\"{0}\"] = \"{1}\",", kEsc, Serialize(canonV)));
                            linesUpdated++;
                            continue;
                        }
                    }
                }
            }

            newLines.Add(line);
        }

        Console.WriteLine(string.Format("Updated {0} lines in RuntimeTextRussian.lua.", linesUpdated));
        File.WriteAllLines(ruPath, newLines, Encoding.UTF8);
        Console.WriteLine(string.Format("Successfully saved {0} lines to {1}.", newLines.Count, ruPath));
    }

    static string CanonizeText(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        // Strictly Клейн
        s = Regex.Replace(s, @"\bКляйн\b", "Клейн");
        s = Regex.Replace(s, @"\bКляйна\b", "Клейна");
        s = Regex.Replace(s, @"\bКляйну\b", "Клейну");
        s = Regex.Replace(s, @"\bКляйном\b", "Клейном");
        s = Regex.Replace(s, @"\bКляйне\b", "Клейне");

        // Strictly Старина Нил
        s = s.Replace("Старый Нил", "Старина Нил");
        s = s.Replace("Старого Нила", "Старины Нила");
        s = s.Replace("Старому Нилу", "Старине Нилу");
        s = s.Replace("Старым Нилом", "Стариной Нилом");

        // Strictly суле
        s = Regex.Replace(s, @"\bсолей\b", "суле");
        s = Regex.Replace(s, @"\bсоля\b", "суле");

        // Strictly Потустороннее свойство
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

        // Slang cleanup
        s = s.Replace("мой выход выше вашего", "мой нанесённый урон выше твоего");
        s = s.Replace("мой выход выше твоего", "мой нанесённый урон выше твоего");
        s = s.Replace("выход урона", "нанесённый урон");
        s = s.Replace("skewering", "сбор группы");

        s = s.Replace("\\п", "\\n").Replace("\\т", "\\t");

        return s;
    }

    static string Unescape(string s)
    {
        return s.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\\", "\\");
    }

    static string Serialize(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }
}
