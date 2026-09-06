using System;
using System.IO;
using System.Text;

class FindInvalidEscapes
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string[] files = new string[] {
            @"d:\gameDev\translate lotm\RuntimeTextRussian.lua",
            @"d:\gameDev\translate lotm\RussianLocalization.lua",
            @"d:\gameDev\translate lotm\data\Init.lua"
        };

        foreach (var file in files)
        {
            Console.WriteLine("Scanning " + Path.GetFileName(file) + "...");
            var lines = File.ReadAllLines(file, Encoding.UTF8);
            int invalidEscapes = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                bool inString = false;
                for (int j = 0; j < line.Length; j++)
                {
                    char c = line[j];
                    if (c == '"')
                    {
                        inString = !inString;
                    }
                    else if (inString && c == '\\')
                    {
                        if (j + 1 >= line.Length)
                        {
                            Console.WriteLine(string.Format("Line {0}:{1} - Trailing backslash", i + 1, j + 1));
                            invalidEscapes++;
                        }
                        else
                        {
                            char next = line[j + 1];
                            bool valid = next == 'a' || next == 'b' || next == 'f' || next == 'n' ||
                                         next == 'r' || next == 't' || next == 'v' || next == '\\' ||
                                         next == '"' || next == '\'' || next == '[' || next == ']' ||
                                         next == '0' || next == '1' || next == '2' || next == '3' ||
                                         next == '4' || next == '5' || next == '6' || next == '7' ||
                                         next == '8' || next == '9' || next == 'x' || next == 'z' ||
                                         next == 'u';

                            if (!valid)
                            {
                                Console.WriteLine(string.Format("Line {0}:{1} - Invalid escape '\\{2}' (Unicode: U+{3:X4}) in: {4}",
                                    i + 1, j + 1, next, (int)next, line.Substring(Math.Max(0, j - 20), Math.Min(60, line.Length - Math.Max(0, j - 20)))));
                                invalidEscapes++;
                            }
                            j++;
                        }
                    }
                }
            }

            Console.WriteLine(string.Format("Total lines: {0}, Invalid escapes: {1}\n", lines.Length, invalidEscapes));
        }
    }
}
