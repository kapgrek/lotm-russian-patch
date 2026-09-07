using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class CleanCache
{
    static void Main()
    {
        string cachePath = @"d:\gameDev\translate lotm\tools\patch26_translation_cache.tsv";
        if (!File.Exists(cachePath)) return;

        var lines = File.ReadAllLines(cachePath, Encoding.UTF8);
        using (var writer = new StreamWriter(cachePath, false, Encoding.UTF8))
        {
            foreach (var line in lines)
            {
                var parts = line.Split('\t');
                if (parts.Length >= 2)
                {
                    string en = parts[0];
                    string ru = parts[1];
                    ru = Regex.Replace(ru, @"[a-f0-9]{32}", "").Trim();
                    ru = ru.Replace("Кляйн", "Клейн")
                           .Replace("Блэкторн", "Чёрный Чертополох")
                           .Replace("Старый Нил", "Старина Нил")
                           .Replace("солей", "суле")
                           .Replace("соля", "суле")
                           .Replace("характеристика", "свойство")
                           .Replace("Характеристика", "Свойство")
                           .Replace("Потусторонняя характеристика", "Потустороннее свойство")
                           .Replace("потусторонняя характеристика", "потустороннее свойство")
                           .Replace("{{Mr.|Ms.}}", "{{мистер|мисс}}")
                           .Replace("{Mr.{ Ms.|}}", "{{мистер|мисс}}")
                           .Replace("{Mr.{Ms.|}}", "{{мистер|мисс}}");
                    writer.WriteLine(en + "\t" + ru);
                }
            }
        }
        Console.WriteLine("Cleaned " + lines.Length + " cache lines.");
    }
}
