using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        string dataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
        string gameTestPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";

        var lines = new List<string>(File.ReadAllLines(ruPath, Encoding.UTF8));

        // Find the last line "}"
        int lastBrace = -1;
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            if (lines[i].Trim() == "}")
            {
                lastBrace = i;
                break;
            }
        }

        if (lastBrace >= 0)
        {
            var additions = new List<string>
            {
                "    [\"<HyperLink stylename=\\\\\\\"Chat_Task\\\\\\\" u=\\\\\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\\\\\">点击帮助</>\"] = \"<HyperLink stylename=\\\\\\\"Chat_Task\\\\\\\" u=\\\\\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\\\\\">Нажмите для помощи</>\",",
                "    [\"<HyperLink stylename=\\\\\\\"Chat_Task\\\\\\\" u=\\\\\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\\\\\"> Click for help </>\"] = \"<HyperLink stylename=\\\\\\\"Chat_Task\\\\\\\" u=\\\\\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\\\\\">Нажмите для помощи</>\",",
                "    [\"击杀<HyperLink stylename=\\\\\\\"Clickable\\\\\\\" u=\\\\\\\"\\\\\\\">守卫者</>概率掉落\"] = \"Шанс выпадения при убийстве <HyperLink stylename=\\\\\\\"Clickable\\\\\\\" u=\\\\\\\"\\\\\\\">Стража</>\",",
                "    [\"Killing <HyperLink stylename=\\\\\\\"Clickable\\\\\\\" u=\\\\\\\"\\\\\\\"> guardian </> has a chance to drop\"] = \"Шанс выпадения при убийстве <HyperLink stylename=\\\\\\\"Clickable\\\\\\\" u=\\\\\\\"\\\\\\\">Стража</>\",",
            };

            lines.InsertRange(lastBrace, additions);

            // Update entries count comment
            for (int i = 0; i < 5; i++)
            {
                if (lines[i].StartsWith("-- Entries:"))
                {
                    lines[i] = "-- Entries: " + (lines.Count - 3);
                    break;
                }
            }

            File.WriteAllLines(ruPath, lines, new UTF8Encoding(false));
            File.Copy(ruPath, dataRuPath, true);
            if (File.Exists(gameTestPath) || Directory.Exists(Path.GetDirectoryName(gameTestPath)))
            {
                try { File.Copy(ruPath, gameTestPath, true); } catch { }
            }
            Console.WriteLine("Added 4 exact literal keys to RuntimeTextRussian.lua and synchronized!");
        }
    }
}
