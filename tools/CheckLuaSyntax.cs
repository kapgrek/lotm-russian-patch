using System;
using System.IO;
using System.Text;

class CheckLuaSyntax
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string[] files = new string[] {
            @"d:\gameDev\translate lotm\RuntimeTextRussian.lua",
            @"d:\gameDev\translate lotm\RussianLocalization.lua"
        };

        bool allOk = true;

        foreach (var file in files)
        {
            Console.WriteLine("Проверка синтаксиса " + Path.GetFileName(file) + "...");
            var lines = File.ReadAllLines(file, Encoding.UTF8);
            int errors = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("[\""))
                {
                    int delim = line.IndexOf("\"] = \"");
                    if (delim < 0)
                    {
                        Console.WriteLine(string.Format("  Строка {0}: отсутствует разделитель \"] = \"", i + 1));
                        errors++;
                        continue;
                    }
                    if (!line.EndsWith("\",") && !line.EndsWith("\""))
                    {
                        Console.WriteLine(string.Format("  Строка {0}: строка не заканчивается на '\",'", i + 1));
                        errors++;
                    }
                }
            }

            if (errors == 0)
            {
                Console.WriteLine(string.Format("  ✅ {0} строк проверено. Ошибок не обнаружено!", lines.Length));
            }
            else
            {
                Console.WriteLine(string.Format("  ❌ Найдено ошибок: {0}", errors));
                allOk = false;
            }
        }

        if (!allOk) Environment.Exit(1);
    }
}
