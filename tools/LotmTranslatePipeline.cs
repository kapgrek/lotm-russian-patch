using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;

class LotmTranslatePipeline
{
    private static string projectRoot;
    private static string ruMasterPath;
    private static string geminiSourcePath;
    private static string shardsDir;
    private static string toolsDir;

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (!InitPaths())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ОШИБКА] Не удалось определить корневую директорию проекта D:\\gameDev\\NewBild.");
            Console.ResetColor();
            Environment.Exit(1);
        }

        if (args.Length == 0)
        {
            PrintUsage();
            return;
        }

        string cmd = args[0].ToLowerInvariant();
        switch (cmd)
        {
            case "status":
                RunStatus();
                break;

            case "extract":
                int count = 50;
                if (args.Length > 1)
                {
                    if (args[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        count = int.MaxValue;
                    }
                    else if (!int.TryParse(args[1], out count) || count <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("[ВНИМАНИЕ] Неверный параметр количества, используется значение по умолчанию 50.");
                        Console.ResetColor();
                        count = 50;
                    }
                }
                string extractTsv = Path.Combine(toolsDir, "batch_pending.tsv");
                if (args.Length > 2) extractTsv = args[2];
                RunExtract(count, extractTsv);
                break;

            case "apply":
                if (args.Length < 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ОШИБКА] Укажите путь к TSV-файлу для применения: apply <tsv-path>");
                    Console.ResetColor();
                    Environment.Exit(1);
                }
                RunApply(args[1]);
                break;

            case "build-shards":
                RunBuildShards();
                break;

            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ОШИБКА] Неизвестная команда: " + args[0]);
                Console.ResetColor();
                PrintUsage();
                Environment.Exit(1);
                break;
        }
    }

    static bool InitPaths()
    {
        string cur = Directory.GetCurrentDirectory();
        if (File.Exists(Path.Combine(cur, "RuntimeTextRussian.lua")))
            projectRoot = cur;
        else if (File.Exists(Path.Combine(cur, "..", "RuntimeTextRussian.lua")))
            projectRoot = Path.GetFullPath(Path.Combine(cur, ".."));
        else if (Directory.Exists(@"D:\gameDev\NewBild"))
            projectRoot = @"D:\gameDev\NewBild";
        else
            return false;

        ruMasterPath = Path.Combine(projectRoot, "RuntimeTextRussian.lua");
        geminiSourcePath = Path.Combine(projectRoot, "source_en", "RuntimeTextGemini.lua");
        shardsDir = Path.Combine(projectRoot, "data", "shards");
        toolsDir = Path.Combine(projectRoot, "tools");

        return File.Exists(ruMasterPath) && File.Exists(geminiSourcePath);
    }

    static void PrintUsage()
    {
        Console.WriteLine("==========================================================================");
        Console.WriteLine("      LOTM RUSSIAN LOCALIZATION — TRANSLATION PIPELINE 2.0");
        Console.WriteLine("==========================================================================");
        Console.WriteLine("Использование: LotmTranslatePipeline.exe <команда> [параметры]");
        Console.WriteLine();
        Console.WriteLine("Команды:");
        Console.WriteLine("  status                  Показать статус покрытия перевода и целостность шардов.");
        Console.WriteLine("  extract <count> [file]  Выгрузить непереведённые фразы в TSV-файл.");
        Console.WriteLine("                          Пример: extract 100");
        Console.WriteLine("                          Пример: extract all tools\\pending.tsv");
        Console.WriteLine("  apply <tsv-file>        Проверить TSV, добавить в мастер-словарь и пересобрать шарды.");
        Console.WriteLine("                          Пример: apply tools\\batch_pending.tsv");
        Console.WriteLine("  build-shards            Принудительно пересобрать 1 024 шарда (BuildPerfectRussianShards).");
        Console.WriteLine("==========================================================================");
    }

    static Dictionary<string, string> LoadRussianDictionary()
    {
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);
        using (var reader = new StreamReader(ruMasterPath, Encoding.UTF8))
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

                        dict[UnescapeLua(k)] = UnescapeLua(v);
                    }
                }
            }
        }
        return dict;
    }

    static List<KeyValuePair<string, string>> LoadGeminiSource()
    {
        var list = new List<KeyValuePair<string, string>>();
        using (var reader = new StreamReader(geminiSourcePath, Encoding.UTF8))
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

                        list.Add(new KeyValuePair<string, string>(UnescapeLua(k), UnescapeLua(v)));
                    }
                }
            }
        }
        return list;
    }

    static bool IsTranslated(string cnKey, string enVal, Dictionary<string, string> ruDict, out string ruVal)
    {
        ruVal = null;
        if (!ruDict.TryGetValue(cnKey, out ruVal) || string.IsNullOrWhiteSpace(ruVal))
        {
            if (!ruDict.TryGetValue(enVal, out ruVal) || string.IsNullOrWhiteSpace(ruVal))
            {
                return false;
            }
        }

        // Если содержит кириллицу — переведено
        if (Regex.IsMatch(ruVal, @"[\u0400-\u04FF]"))
            return true;

        // Если значение совпадает с английским оригиналом
        if (ruVal.Equals(enVal, StringComparison.Ordinal))
        {
            // Если в оригинале нет английских букв (числа, форматирование %d, знаки) — валидно
            if (!Regex.IsMatch(enVal, @"[a-zA-Z]"))
                return true;

            // Иначе строка оставлена на английском без перевода
            return false;
        }

        return true;
    }

    static void RunStatus()
    {
        Console.WriteLine("==========================================================================");
        Console.WriteLine("             LOTM LOCALIZATION STATUS — СТАТУС ПЕРЕВОДА");
        Console.WriteLine("==========================================================================");

        var ruDict = LoadRussianDictionary();
        var geminiList = LoadGeminiSource();

        int totalGemini = geminiList.Count;
        int translated = 0;
        int untranslated = 0;
        var pendingSamples = new List<KeyValuePair<string, string>>();

        foreach (var kvp in geminiList)
        {
            string ruVal;
            if (IsTranslated(kvp.Key, kvp.Value, ruDict, out ruVal))
            {
                translated++;
            }
            else
            {
                untranslated++;
                if (pendingSamples.Count < 5)
                    pendingSamples.Add(kvp);
            }
        }

        double coveragePercent = totalGemini > 0 ? ((double)translated / totalGemini) * 100.0 : 100.0;

        Console.WriteLine(string.Format("• Исходных фраз (Gemini Source):  {0:N0}", totalGemini));
        Console.WriteLine(string.Format("• Записей в мастер-словаре:       {0:N0} (CN + EN ключи)", ruDict.Count));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(string.Format("• Переведено фраз:                {0:N0} ({1:F3}%)", translated, coveragePercent));
        Console.ResetColor();

        if (untranslated > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format("• Ожидает перевода:               {0:N0}", untranslated));
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Примеры непереведённых строк:");
            foreach (var p in pendingSamples)
            {
                string previewCn = p.Key.Replace("\r\n", " ").Replace("\n", " ");
                string previewEn = p.Value.Replace("\r\n", " ").Replace("\n", " ");
                if (previewCn.Length > 45) previewCn = previewCn.Substring(0, 42) + "...";
                if (previewEn.Length > 45) previewEn = previewEn.Substring(0, 42) + "...";
                Console.WriteLine(string.Format("  [CN] {0}  ==>  [EN] {1}", previewCn, previewEn));
            }
            Console.WriteLine();
            Console.WriteLine(string.Format("Для выгрузки выполните: .\\tools\\LotmTranslatePipeline.exe extract {0}", Math.Min(50, untranslated)));
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("• Все фразы из Gemini Source переведены на 100%!");
            Console.ResetColor();
        }

        // Проверка шардов
        int shardCount = Directory.Exists(shardsDir) ? Directory.GetFiles(shardsDir, "RuntimeTextGemini_*.lua").Length : 0;
        Console.WriteLine();
        Console.WriteLine("Целостность шардов (Архитектура 2.0):");
        if (shardCount == 1024)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ✔ Все 1 024 шарда присутствуют в data/shards/ (Двухиндексная система активна)");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format("  ✖ Обнаружено {0} шардов из 1 024! Запустите: build-shards", shardCount));
            Console.ResetColor();
        }
        Console.WriteLine("==========================================================================");
    }

    static void RunExtract(int count, string outTsvPath)
    {
        Console.WriteLine("=== ЭКСТРАКЦИЯ НЕПЕРЕВЕДЁННЫХ СТРОК ===");
        var ruDict = LoadRussianDictionary();
        var geminiList = LoadGeminiSource();

        var pending = new List<KeyValuePair<string, string>>();
        foreach (var kvp in geminiList)
        {
            string ruVal;
            if (!IsTranslated(kvp.Key, kvp.Value, ruDict, out ruVal))
            {
                pending.Add(kvp);
                if (pending.Count >= count) break;
            }
        }

        if (pending.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Непереведённых строк не найдено! Все фразы уже переведены.");
            Console.ResetColor();
            return;
        }

        string outDir = Path.GetDirectoryName(Path.GetFullPath(outTsvPath));
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

        using (var writer = new StreamWriter(outTsvPath, false, Encoding.UTF8))
        {
            writer.WriteLine("CN\tEN\tRU");
            foreach (var kvp in pending)
            {
                string cn = EscapeTsv(kvp.Key);
                string en = EscapeTsv(kvp.Value);
                writer.WriteLine(string.Format("{0}\t{1}\t", cn, en));
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(string.Format("Успешно выгружено {0} строк в файл:", pending.Count));
        Console.WriteLine("  " + Path.GetFullPath(outTsvPath));
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Инструкция:");
        Console.WriteLine("1. Откройте TSV в редакторе (VS Code / Excel) и заполните третью колонку (RU).");
        Console.WriteLine(string.Format("2. Примените перевод командой: .\\tools\\LotmTranslatePipeline.exe apply \"{0}\"", outTsvPath));
    }

    static void RunApply(string tsvPath)
    {
        if (!File.Exists(tsvPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ОШИБКА] TSV-файл не найден: " + tsvPath);
            Console.ResetColor();
            return;
        }

        Console.WriteLine("=== ВАЛИДАЦИЯ И ПРИМЕНЕНИЕ ПЕРЕВОДА ИЗ TSV ===");
        string[] lines = File.ReadAllLines(tsvPath, Encoding.UTF8);

        var newEntries = new List<Tuple<string, string, string>>(); // CN, EN, RU
        int lineNum = 0;
        int warnings = 0;

        foreach (var rawLine in lines)
        {
            lineNum++;
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("//"))
                continue;

            if (lineNum == 1 && line.StartsWith("CN\tEN", StringComparison.OrdinalIgnoreCase))
                continue;

            string[] cols = rawLine.Split('\t');
            if (cols.Length < 3)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(string.Format("[ПРЕДУПРЕЖДЕНИЕ] Строка {0}: Недостаточно колонок (ожидалось 3, найдено {1}). Пропуск.", lineNum, cols.Length));
                Console.ResetColor();
                warnings++;
                continue;
            }

            string cn = UnescapeTsv(cols[0]);
            string en = UnescapeTsv(cols[1]);
            string ru = UnescapeTsv(cols[2]);

            if (string.IsNullOrWhiteSpace(ru))
            {
                continue; // Пустой перевод игнорируем
            }

            // Валидация тегов и спецификаторов
            string tagWarn;
            if (!ValidateTags(en, ru, out tagWarn))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(string.Format("[ТЕГИ] Строка {0}: {1}", lineNum, tagWarn));
                Console.ResetColor();
                warnings++;
            }

            newEntries.Add(Tuple.Create(cn, en, ru));
        }

        if (newEntries.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Не найдено заполненных строк перевода для добавления.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine(string.Format("Считано {0} валидных записей (предупреждений: {1}). Добавление в мастер-словарь...", newEntries.Count, warnings));

        // Читаем текущий RuntimeTextRussian.lua
        var ruDict = LoadRussianDictionary();
        int addedKeys = 0;

        foreach (var entry in newEntries)
        {
            string cn = entry.Item1;
            string en = entry.Item2;
            string ru = entry.Item3;

            if (!string.IsNullOrEmpty(cn))
            {
                ruDict[cn] = ru;
                addedKeys++;
            }
            if (!string.IsNullOrEmpty(en))
            {
                ruDict[en] = ru;
                addedKeys++;
            }
        }

        // Записываем обновлённый RuntimeTextRussian.lua
        Console.WriteLine("Обновление мастер-файла " + ruMasterPath + "...");
        using (var writer = new StreamWriter(ruMasterPath, false, Encoding.UTF8))
        {
            writer.WriteLine("-- Russian Master Translation Dictionary");
            writer.WriteLine(string.Format("-- Total unique dual-indexed entries: {0}", ruDict.Count));
            writer.WriteLine("return {");
            foreach (var kvp in ruDict)
            {
                writer.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", CleanForLua(kvp.Key), CleanForLua(kvp.Value)));
            }
            writer.WriteLine("}");
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(string.Format("Мастер-словарь успешно обновлён! Новых ключей: {0}, всего записей: {1:N0}.", addedKeys, ruDict.Count));
        Console.ResetColor();

        // Автоматическая сборка шардов
        RunBuildShards();
    }

    static void RunBuildShards()
    {
        Console.WriteLine();
        Console.WriteLine("=== АВТОМАТИЧЕСКАЯ СБОРКА 1 024 ШАРДОВ ===");
        string shardBuilderExe = Path.Combine(toolsDir, "BuildPerfectRussianShards.exe");
        string shardBuilderCs = Path.Combine(toolsDir, "BuildPerfectRussianShards.cs");

        if (!File.Exists(shardBuilderExe))
        {
            if (File.Exists(shardBuilderCs))
            {
                Console.WriteLine("Компиляция BuildPerfectRussianShards.exe...");
                string csc = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe";
                var cscProc = Process.Start(new ProcessStartInfo
                {
                    FileName = csc,
                    Arguments = string.Format("/nologo /optimize+ /out:\"{0}\" \"{1}\"", shardBuilderExe, shardBuilderCs),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                });
                cscProc.WaitForExit();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ОШИБКА] Исходный код BuildPerfectRussianShards.cs не найден!");
                Console.ResetColor();
                return;
            }
        }

        var proc = Process.Start(new ProcessStartInfo
        {
            FileName = shardBuilderExe,
            WorkingDirectory = projectRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        });

        while (!proc.StandardOutput.EndOfStream)
        {
            string line = proc.StandardOutput.ReadLine();
            Console.WriteLine("  [SHARDS] " + line);
        }
        proc.WaitForExit();

        if (proc.ExitCode == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ Все 1 024 шарда успешно обновлены и синхронизированы!");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✖ Ошибка при сборке шардов! Код завершения: " + proc.ExitCode);
            Console.ResetColor();
        }
    }

    static bool ValidateTags(string original, string translated, out string warning)
    {
        warning = null;
        var tagPattern = new Regex(@"<[^>]+>");
        var origTags = tagPattern.Matches(original);
        var transTags = tagPattern.Matches(translated);

        if (origTags.Count != transTags.Count)
        {
            warning = string.Format("Несоответствие количества тегов: в оригинале {0}, в переводе {1}", origTags.Count, transTags.Count);
            return false;
        }

        // Проверка параметров %s, %d, %i
        var specPattern = new Regex(@"%[0-9\.]*[sdiMf]");
        var origSpecs = specPattern.Matches(original);
        var transSpecs = specPattern.Matches(translated);

        if (origSpecs.Count != transSpecs.Count)
        {
            warning = string.Format("Несоответствие спецификаторов форматирования: в оригинале {0}, в переводе {1}", origSpecs.Count, transSpecs.Count);
            return false;
        }

        return true;
    }

    static string EscapeTsv(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "\\r");
    }

    static string UnescapeTsv(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        var sb = new StringBuilder();
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '\\' && i + 1 < s.Length)
            {
                char c = s[i + 1];
                if (c == 't') { sb.Append('\t'); i++; }
                else if (c == 'n') { sb.Append('\n'); i++; }
                else if (c == 'r') { sb.Append('\r'); i++; }
                else if (c == '\\') { sb.Append('\\'); i++; }
                else sb.Append(s[i]);
            }
            else
            {
                sb.Append(s[i]);
            }
        }
        return sb.ToString();
    }

    static string UnescapeLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return Regex.Unescape(s);
    }

    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\\\r\n", "\\\\\\n").Replace("\\\n", "\\\\\\n");
        r = r.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "");
        if (r.Contains("\\\"") && (r.Contains("guildTaskHelp") || r.Contains("Clickable")))
        {
            return r.Replace("\\\"", "\\\\\\\"");
        }
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
    }
}
