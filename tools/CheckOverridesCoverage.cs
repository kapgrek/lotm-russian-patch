using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class CheckOverridesCoverage
{
    static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."));
        if (!Directory.Exists(Path.Combine(projectRoot, "data")))
        {
            projectRoot = @"D:\gameDev\NewBild";
        }

        string initPath = Path.Combine(projectRoot, "data", "Init.lua");
        string ruLocPath = Path.Combine(projectRoot, "RussianLocalization.lua");

        if (!File.Exists(initPath))
        {
            Console.WriteLine("[ОШИБКА] Файл не найден: " + initPath);
            return 1;
        }
        if (!File.Exists(ruLocPath))
        {
            Console.WriteLine("[ОШИБКА] Файл не найден: " + ruLocPath);
            return 1;
        }

        Console.WriteLine("==========================================================");
        Console.WriteLine("   Валидация покрытия системных оверрайдов и констант   ");
        Console.WriteLine("==========================================================");

        // Read stringConstOverrides from Init.lua
        var initStringConsts = new Dictionary<string, string>();
        var initExactOverrides = new Dictionary<string, string>();

        string[] lines = File.ReadAllLines(initPath, Encoding.UTF8);
        bool inConst = false;
        bool inExact = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("local stringConstOverrides = {")) { inConst = true; continue; }
            if (inConst && line == "}") { inConst = false; continue; }
            if (line.StartsWith("local visibleTextExactOverrides = {")) { inExact = true; continue; }
            if (inExact && line == "}") { inExact = false; continue; }

            if (inConst)
            {
                int eq = line.IndexOf(" = \"");
                if (eq > 0)
                {
                    string key = line.Substring(0, eq).Trim();
                    int valEnd = line.LastIndexOf("\"");
                    string val = line.Substring(eq + 4, valEnd - (eq + 4));
                    initStringConsts[key] = val;
                }
            }

            if (inExact)
            {
                if (line.StartsWith("[\""))
                {
                    int delim = line.IndexOf("\"] =");
                    if (delim > 0)
                    {
                        string key = line.Substring(2, delim - 2);
                        initExactOverrides[key] = "";
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Init.lua stringConstOverrides: {0}", initStringConsts.Count));
        Console.WriteLine(string.Format("Init.lua visibleTextExactOverrides: {0}", initExactOverrides.Count));

        // Read RussianLocalization.lua stringConstOverrides and exact overrides
        string ruLoc = File.ReadAllText(ruLocPath, Encoding.UTF8);

        int missingConsts = 0;
        foreach (var k in initStringConsts.Keys)
        {
            if (!ruLoc.Contains(k + " = \""))
            {
                Console.WriteLine("Missing stringConstOverride in RussianLocalization: " + k + " (EN: " + initStringConsts[k] + ")");
                missingConsts++;
            }
        }
        Console.WriteLine(string.Format("Пропущено stringConstOverrides: {0}", missingConsts));

        int missingExact = 0;
        foreach (var k in initExactOverrides.Keys)
        {
            if (!ruLoc.Contains("[\"" + k + "\"]"))
            {
                missingExact++;
                if (missingExact <= 15)
                {
                    Console.WriteLine("Missing visibleTextExactOverride: " + k);
                }
            }
        }
        Console.WriteLine(string.Format("Пропущено visibleTextExactOverrides: {0}", missingExact));

        if (missingConsts == 0 && missingExact == 0)
        {
            Console.WriteLine("\n🎉 ВСЕ ТЕСТЫ ПРОЙДЕНЫ! Покрытие оверрайдов 100% (0 missing)!");
            return 0;
        }
        else
        {
            Console.WriteLine(string.Format("\n⚠️ ВНИМАНИЕ: Найдено непокрытых оверрайдов: {0}", missingConsts + missingExact));
            return 1;
        }
    }
}
