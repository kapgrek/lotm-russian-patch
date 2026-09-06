using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class VerifySkillCoverage
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string geminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        string dataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
        string gameRuPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";

        Console.WriteLine("==========================================================");
        Console.WriteLine("     Валидация покрытия боевых навыков и формул игры     ");
        Console.WriteLine("==========================================================");

        if (!File.Exists(ruPath))
        {
            Console.WriteLine("[ОШИБКА] Файл не найден: " + ruPath);
            Environment.Exit(1);
        }

        var existingRu = new HashSet<string>(StringComparer.Ordinal);
        int totalRuEntries = 0;

        string shardsDir = @"d:\gameDev\translate lotm\data\shards";
        if (Directory.Exists(shardsDir) && Directory.GetFiles(shardsDir, "RuntimeTextGemini_*.lua").Length > 0)
        {
            var shardFiles = Directory.GetFiles(shardsDir, "RuntimeTextGemini_*.lua");
            Console.WriteLine(string.Format("Загрузка русских шардов из data/shards/ ({0} файлов)...", shardFiles.Length));
            foreach (var sf in shardFiles)
            {
                foreach (var line in File.ReadAllLines(sf, Encoding.UTF8))
                {
                    string t = line.Trim();
                    if (t.StartsWith("[\""))
                    {
                        int delim = t.IndexOf("\"] = \"");
                        if (delim > 2)
                        {
                            existingRu.Add(t.Substring(2, delim - 2));
                            totalRuEntries++;
                        }
                    }
                }
            }
            Console.WriteLine(string.Format("Всего записей в шардах: {0} (уникальных ключей: {1})", totalRuEntries, existingRu.Count));
        }
        else if (File.Exists(ruPath))
        {
            foreach (var line in File.ReadAllLines(ruPath, Encoding.UTF8))
            {
                string t = line.Trim();
                if (t.StartsWith("[\""))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 2)
                    {
                        existingRu.Add(t.Substring(2, delim - 2));
                        totalRuEntries++;
                    }
                }
            }
            Console.WriteLine(string.Format("Всего записей в RuntimeTextRussian.lua: {0}", totalRuEntries));
        }

        // 1. Проверка покрытия формульных навыков
        var formulaRegex = new Regex(@"(\*d|\*f|spellfielddisc|bulletdisc|buffdisc|mul\(|CheckStar\()", RegexOptions.IgnoreCase);
        var stanceRegex = new Regex(@"Stance.*switches to|switches to.*Stance|Only available in.*Stance|仅.*姿态可用", RegexOptions.IgnoreCase);

        int totalFormulas = 0;
        int translatedFormulas = 0;
        int missingFormulas = 0;
        var missingSamples = new List<string>();

        using (var reader = new StreamReader(geminiPath, Encoding.UTF8))
        {
            string line;
            int lineNum = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineNum++;
                string t = line.Trim();
                if (t.StartsWith("[\""))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 2)
                    {
                        string cnKey = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : (t.EndsWith("\"") ? t.Length - 1 : -1);
                        if (valEnd >= valStart)
                        {
                            string enVal = t.Substring(valStart, valEnd - valStart);
                            if (formulaRegex.IsMatch(enVal) || stanceRegex.IsMatch(enVal))
                            {
                                totalFormulas++;
                                if (existingRu.Contains(cnKey) || existingRu.Contains(enVal))
                                {
                                    translatedFormulas++;
                                }
                                else
                                {
                                    missingFormulas++;
                                    if (missingSamples.Count < 5)
                                    {
                                        missingSamples.Add(string.Format("Строка {0}: {1}", lineNum, enVal.Substring(0, Math.Min(80, enVal.Length))));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Всего формульных/детальных навыков в Gemini: {0}", totalFormulas));
        Console.WriteLine(string.Format("Переведено на русский: {0} ({1:P1})", translatedFormulas, (double)translatedFormulas / totalFormulas));
        Console.WriteLine(string.Format("Отсутствует: {0}", missingFormulas));

        if (missingFormulas > 0)
        {
            Console.WriteLine("[ВНИМАНИЕ] Примеры пропущенных навыков:");
            foreach (var s in missingSamples) Console.WriteLine("  - " + s);
        }

        // 2. Проверка ключевых навыков
        Console.WriteLine("\nПроверка контрольных навыков:");
        bool mindFireDetailed = false;
        bool mindFirePassive = false;

        foreach (var k in existingRu)
        {
            if (k.Contains("Releases Mind Fire, with a *d** base probability") || k.Contains("释放心灵之火，以*d**的基础概率"))
                mindFireDetailed = true;
            if (k.Contains("86023010") && (k.Contains("心灵之火") || k.Contains("Mind Fire")))
                mindFirePassive = true;
        }

        Console.WriteLine(string.Format("  - Навык «Пламя разума» (Детальное описание, строка 118131): {0}", mindFireDetailed ? "✅ НАЙДЕН" : "❌ ОТСУТСТВУЕТ"));
        Console.WriteLine(string.Format("  - Талант «Пламя разума» (Пассивный узел Horror, строка 5256): {0}", mindFirePassive ? "✅ НАЙДЕН" : "❌ ОТСУТСТВУЕТ"));

        // 3. Проверка синхронизации шардов и файлов
        Console.WriteLine("\nПроверка синхронизации архитектуры шардов:");
        int shardCount = Directory.Exists(shardsDir) ? Directory.GetFiles(shardsDir, "RuntimeTextGemini_*.lua").Length : 0;
        Console.WriteLine(string.Format("  - Русских шардов в data/shards/: {0}/1024 {1}", shardCount, shardCount == 1024 ? "✅ ПОЛНЫЙ НАБОР" : "❌ НЕПОЛНЫЙ"));

        string modBaseShardsDir = @"d:\gameDev\translate lotm\mod_base\Saved\Mods\lua\mods\cpdd_runtime_fixes";
        int modBaseShardCount = Directory.Exists(modBaseShardsDir) ? Directory.GetFiles(modBaseShardsDir, "RuntimeTextGemini_*.lua").Length : 0;
        Console.WriteLine(string.Format("  - Русских шардов в mod_base/: {0}/1024 {1}", modBaseShardCount, modBaseShardCount == 1024 ? "✅ ПОЛНЫЙ НАБОР" : "❌ НЕПОЛНЫЙ"));

        if (File.Exists(dataRuPath))
        {
            long dataSize = new FileInfo(dataRuPath).Length;
            Console.WriteLine(string.Format("  - Стаб RuntimeTextRussian.lua в data/: {0} байт {1}", dataSize, dataSize < 5000 ? "✅ БЕЗОПАСЕН (LJ_MAX_CONSTS защищён)" : "⚠️ ПРЕВЫШАЕТ ЛИМИТ"));
        }

        if (missingFormulas == 0 && mindFireDetailed && mindFirePassive && shardCount == 1024)
        {
            Console.WriteLine("\n🎉 ВСЕ ТЕСТЫ ПРОЙДЕНЫ! Покрытие детальных навыков 100%! Шардовая архитектура полностью готова!");
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("\n⚠️ ВНИМАНИЕ: Требуется завершить перевод или синхронизацию!");
            Environment.Exit(missingFormulas > 0 ? 1 : 0);
        }
    }
}
