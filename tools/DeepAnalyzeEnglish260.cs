using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class DeepAnalyzeEnglish260
{
    static bool HasCyrillic(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        foreach (char c in s)
        {
            if ((c >= 0x0400 && c <= 0x04FF) || c == 0x0500 || c == 0x0501)
                return true;
        }
        return false;
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string shardsDir = @"temp_en_2.6.0\extracted\payload\bridge\game\Saved\Mods\lua\mods\cpdd_runtime_fixes";
        string oldSourceEnFile = @"source_en\RuntimeTextGemini.lua";
        string masterRuFile = @"RuntimeTextRussian.lua";
        string reportFile = @"temp_en_2.6.0\full_analysis_report.txt";

        using (var outWriter = new StreamWriter(reportFile, false, Encoding.UTF8))
        {
            Action<string> log = (msg) => {
                Console.WriteLine(msg);
                outWriter.WriteLine(msg);
            };

            log("================================================================================");
            log("           ПОЛНЫЙ АНАЛИЗ ОБНОВЛЕНИЯ АНГЛИЙСКОГО ПАТЧА v2.6.0");
            log("================================================================================");
            log("Дата анализа: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            // 1. Load Master Russian Dictionary
            log("\n[1] Загрузка мастер-словаря русской локализации (RuntimeTextRussian.lua)...");
            var ruDict = new Dictionary<string, string>(StringComparer.Ordinal);
            using (var reader = new StreamReader(masterRuFile, Encoding.UTF8))
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
                            ruDict[k] = v;
                        }
                    }
                }
            }
            log(string.Format("Всего записей в RuntimeTextRussian.lua: {0:N0}", ruDict.Count));

            // 2. Load Old Source EN
            log("\n[2] Загрузка старой английской базы (source_en/RuntimeTextGemini.lua)...");
            var oldEnDict = new Dictionary<string, string>(StringComparer.Ordinal);
            if (File.Exists(oldSourceEnFile))
            {
                using (var reader = new StreamReader(oldSourceEnFile, Encoding.UTF8))
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
                                oldEnDict[k] = v;
                            }
                        }
                    }
                }
            }
            log(string.Format("Всего записей в source_en/RuntimeTextGemini.lua: {0:N0}", oldEnDict.Count));

            // 3. Load New English 2.6.0 Shards
            log("\n[3] Загрузка 1 024 шардов нового английского патча v2.6.0...");
            var newEnShards = new Dictionary<string, string>(StringComparer.Ordinal);
            var shardFiles = Directory.GetFiles(shardsDir, "RuntimeTextGemini_*.lua");
            log(string.Format("Найдено файлов шардов: {0}", shardFiles.Length));

            foreach (var file in shardFiles)
            {
                using (var reader = new StreamReader(file, Encoding.UTF8))
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
                                newEnShards[k] = v;
                            }
                        }
                    }
                }
            }
            log(string.Format("Всего уникальных китайских ключей в новых шардах EN: {0:N0}", newEnShards.Count));

            // 4. Comparison: New EN Shards vs Old source_en
            log("\n[4] Сравнение новой английской базы с предыдущей...");
            var brandNewCnKeys = new List<string>();
            var modifiedEnTranslations = new List<string>();
            var removedCnKeys = new List<string>();

            foreach (var kvp in newEnShards)
            {
                string oldVal;
                if (!oldEnDict.TryGetValue(kvp.Key, out oldVal))
                {
                    brandNewCnKeys.Add(kvp.Key);
                }
                else if (oldVal != kvp.Value)
                {
                    modifiedEnTranslations.Add(kvp.Key);
                }
            }

            foreach (var k in oldEnDict.Keys)
            {
                if (!newEnShards.ContainsKey(k))
                {
                    removedCnKeys.Add(k);
                }
            }

            log(string.Format(" - Совершенно новых строк (отсутствовали в старой базе): {0:N0}", brandNewCnKeys.Count));
            log(string.Format(" - Измененных / исправленных переводов Lani27: {0:N0}", modifiedEnTranslations.Count));
            log(string.Format(" - Удаленных строк из старой базы: {0:N0}", removedCnKeys.Count));

            // 5. Check Russian Coverage for New EN Shards
            log("\n[5] Анализ покрытия новой английской базы в текущей русской локализации...");

            int cnInRuWithCyrillic = 0;
            int cnInRuWithoutCyrillic = 0;
            int cnNotInRu = 0;

            var missingFromRuList = new List<KeyValuePair<string, string>>();
            var nonCyrillicInRuList = new List<KeyValuePair<string, string>>();

            foreach (var kvp in newEnShards)
            {
                string ruVal;
                if (ruDict.TryGetValue(kvp.Key, out ruVal))
                {
                    if (HasCyrillic(ruVal))
                    {
                        cnInRuWithCyrillic++;
                    }
                    else
                    {
                        cnInRuWithoutCyrillic++;
                        nonCyrillicInRuList.Add(new KeyValuePair<string, string>(kvp.Key, ruVal));
                    }
                }
                else
                {
                    cnNotInRu++;
                    missingFromRuList.Add(kvp);
                }
            }

            log(string.Format(" - Китайских ключей, имеющих полноценный перевод на русский (с кириллицей): {0:N0} ({1:P2})", 
                cnInRuWithCyrillic, (double)cnInRuWithCyrillic / newEnShards.Count));
            log(string.Format(" - Китайских ключей в русском словаре БЕЗ кириллицы (латиница/числа/заглушки): {0:N0}", cnInRuWithoutCyrillic));
            log(string.Format(" - Китайских ключей, ВООБЩЕ ОТСУТСТВУЮЩИХ в русском словаре: {0:N0}", cnNotInRu));

            // 6. Check EN keys coverage in Russian Dictionary
            int enKeyInRu = 0;
            int enKeyNotInRu = 0;
            foreach (var kvp in newEnShards)
            {
                if (ruDict.ContainsKey(kvp.Value))
                    enKeyInRu++;
                else
                    enKeyNotInRu++;
            }
            log(string.Format(" - Английских значений как ключей в RU словаре: найдено {0:N0}, отсутствует {1:N0}", enKeyInRu, enKeyNotInRu));

            // 7. Analysis of what brand new strings are
            log("\n[6] Категоризация новых строк (Brand New Keys: " + brandNewCnKeys.Count + ")...");
            var categories = new Dictionary<string, int>();
            foreach (var k in brandNewCnKeys)
            {
                string enVal = newEnShards[k];
                string cat = "Прочее (диалоги/текст)";
                if (enVal.StartsWith("<") || k.StartsWith("<")) cat = "RichText / UI теги";
                else if (enVal.Contains("http") || enVal.Contains("www.")) cat = "Ссылки / Веб";
                else if (Regex.IsMatch(enVal, @"^[0-9\.\-\+\:\s\%]+$")) cat = "Числовые / Параметры";
                else if (k.Length <= 4) cat = "Короткие фразы / Названия";
                else if (enVal.ToLower().Contains("skill") || enVal.ToLower().Contains("damage") || enVal.ToLower().Contains("cooldown")) cat = "Боевые механики / Навыки";
                
                int c;
                categories.TryGetValue(cat, out c);
                categories[cat] = c + 1;
            }

            foreach (var cat in categories)
            {
                log(string.Format("   * {0}: {1} строк", cat.Key, cat.Value));
            }

            // 8. Detailed Samples
            log("\n[7] Примеры совершенно новых строк в патче 2.6.0:");
            int showCount = Math.Min(15, brandNewCnKeys.Count);
            for (int i = 0; i < showCount; i++)
            {
                string k = brandNewCnKeys[i];
                string en = newEnShards[k];
                string ru;
                ruDict.TryGetValue(k, out ru);
                log(string.Format("  [{0}] CN: {1}", i + 1, k));
                log(string.Format("      EN: {0}", en));
                log(string.Format("      RU в словаре: {0}", ru != null ? ru : "<НЕТ В СЛОВАРЕ>"));
            }

            log("\n[8] Анализ измененных переводов (Modified Translations: " + modifiedEnTranslations.Count + "):");
            showCount = Math.Min(15, modifiedEnTranslations.Count);
            for (int i = 0; i < showCount; i++)
            {
                string k = modifiedEnTranslations[i];
                string oldEn = oldEnDict[k];
                string newEn = newEnShards[k];
                string ru;
                ruDict.TryGetValue(k, out ru);
                log(string.Format("  [{0}] CN: {1}", i + 1, k));
                log(string.Format("      Старый EN: {0}", oldEn));
                log(string.Format("      Новый  EN: {0}", newEn));
                log(string.Format("      Текущий RU: {0}", ru != null ? ru : "<НЕТ В СЛОВАРЕ>"));
            }

            // 9. Analysis of Other Files in English Patch 2.6.0
            log("\n[9] Анализ дополнительных компонентов English Patch 2.6.0:");
            string overridesLua = @"temp_en_2.6.0\extracted\payload\bridge\game\Saved\Mods\translation-overrides.lua";
            if (File.Exists(overridesLua))
            {
                var lines = File.ReadAllLines(overridesLua, Encoding.UTF8);
                log(string.Format(" - translation-overrides.lua: {0} строк", lines.Length));
            }

            string stateJson = @"temp_en_2.6.0\extracted\payload\bridge\game\Saved\Mods\translation-overrides.state.json";
            if (File.Exists(stateJson))
            {
                var fi = new FileInfo(stateJson);
                log(string.Format(" - translation-overrides.state.json: {0:N0} байт", fi.Length));
            }

            string bakedBin = @"temp_en_2.6.0\extracted\payload\bridge\game\Saved\Mods\BakedText\blocks.bin";
            if (File.Exists(bakedBin))
            {
                var fi = new FileInfo(bakedBin);
                log(string.Format(" - BakedText/blocks.bin: {0:N0} байт", fi.Length));
            }

            string bakedManifest = @"temp_en_2.6.0\extracted\payload\bridge\game\Saved\Mods\BakedText\manifest.json";
            if (File.Exists(bakedManifest))
            {
                var fi = new FileInfo(bakedManifest);
                log(string.Format(" - BakedText/manifest.json: {0:N0} байт", fi.Length));
            }

            string cpddTranslationDir = @"temp_en_2.6.0\extracted\payload\bridge\game\Saved\Mods\lua\cpdd_translation";
            if (Directory.Exists(cpddTranslationDir))
            {
                var files = Directory.GetFiles(cpddTranslationDir, "*.*", SearchOption.AllDirectories);
                log(string.Format(" - cpdd_translation: {0} файлов (Excel, Config, Gameplay)", files.Length));
            }

            string enInit = Path.Combine(shardsDir, "Init.lua");
            if (File.Exists(enInit))
            {
                var lines = File.ReadAllLines(enInit, Encoding.UTF8);
                log(string.Format(" - Init.lua английского патча: {0} строк", lines.Length));
            }

            log("\n================================================================================");
            log("                             ВЫВОДЫ И ИТОГИ");
            log("================================================================================");
            log(string.Format("1. Английская база расширилась со 126 617 до {0:N0} строк (+{1:N0} новых строк).", newEnShards.Count, brandNewCnKeys.Count));
            log(string.Format("2. Текущее покрытие русской локализации:"));
            log(string.Format("   - С кириллическим переводом: {0:N0} строк ({1:P2} от новой базы)", cnInRuWithCyrillic, (double)cnInRuWithCyrillic / newEnShards.Count));
            log(string.Format("   - Требует внимания / доперевода:"));
            log(string.Format("     * Полностью отсутствуют в RU словаре: {0:N0} строк", cnNotInRu));
            log(string.Format("     * В словаре RU без кириллицы (на англ/числа/теги): {0:N0} строк", cnInRuWithoutCyrillic));
            log(string.Format("     * Изменено английских формулировок/тегов в 2.6.0: {0:N0} строк", modifiedEnTranslations.Count));
            log("================================================================================");
        }
    }
}
