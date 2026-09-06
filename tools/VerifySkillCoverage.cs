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

        // 3. Проверка синхронизации файлов
        Console.WriteLine("\nПроверка синхронизации:");
        long rootSize = new FileInfo(ruPath).Length;
        Console.WriteLine(string.Format("  - Размер RuntimeTextRussian.lua в корне: {0:N0} байт", rootSize));

        if (File.Exists(dataRuPath))
        {
            long dataSize = new FileInfo(dataRuPath).Length;
            Console.WriteLine(string.Format("  - Размер в data/: {0:N0} байт {1}", dataSize, dataSize == rootSize ? "✅ Синхронизирован" : "⚠️ Отличается"));
        }

        if (File.Exists(gameRuPath))
        {
            long gameSize = new FileInfo(gameRuPath).Length;
            Console.WriteLine(string.Format("  - Размер в игре: {0:N0} байт {1}", gameSize, gameSize == rootSize ? "✅ Синхронизирован" : "⚠️ Отличается"));
        }

        if (missingFormulas == 0 && mindFireDetailed && mindFirePassive)
        {
            Console.WriteLine("\n🎉 ВСЕ ТЕСТЫ ПРОЙДЕНЫ! Покрытие детальных навыков 100%!");
            Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("\n⚠️ ВНИМАНИЕ: Требуется завершить перевод или синхронизацию!");
            Environment.Exit(missingFormulas > 0 ? 1 : 0);
        }
    }
}
