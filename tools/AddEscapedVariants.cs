using System;
using System.IO;
using System.Text;

class AddEscapedVariants
{
    static void Main()
    {
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        var lines = File.ReadAllLines(ruPath, Encoding.UTF8);
        using (var sw = new StreamWriter(ruPath + ".tmp", false, new UTF8Encoding(false)))
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == lines.Length - 1 && lines[i].Trim() == "}")
                {
                    sw.WriteLine("    [\"<HyperLink stylename=\\\\\\\"Chat_Task\\\\\\\" u=\\\\\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\\\\\">点击帮助</>\"] = \"<HyperLink stylename=\\\\\\\"Chat_Task\\\\\\\" u=\\\\\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\\\\\"> Нажмите, чтобы получить помощь </>\"," );
                    sw.WriteLine("    [\"<HyperLink stylename=\\\\\\\"Chat_Task\\\\\\\" u=\\\\\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\\\\\"> Click for help </>\"] = \"<HyperLink stylename=\\\\\\\"Chat_Task\\\\\\\" u=\\\\\\\"guildTaskHelp=%s,%d,%s,%s,%s,%s\\\\\\\"> Нажмите, чтобы получить помощь </>\"," );
                    sw.WriteLine("    [\"击杀<HyperLink stylename=\\\\\\\"Clickable\\\\\\\" u=\\\\\\\"\\\\\\\">守卫者</>概率掉落\"] = \"Убив стража <HyperLink stylename=\\\\\\\"Clickable\\\\\\\" u=\\\\\\\"\\\\\\\">, </> может выпасть.\"," );
                    sw.WriteLine("    [\"Killing <HyperLink stylename=\\\\\\\"Clickable\\\\\\\" u=\\\\\\\"\\\\\\\"> guardian </> has a chance to drop\"] = \"Убив стража <HyperLink stylename=\\\\\\\"Clickable\\\\\\\" u=\\\\\\\"\\\\\\\">, </> может выпасть.\"," );
                }
                sw.WriteLine(lines[i]);
            }
        }
        File.Delete(ruPath);
        File.Move(ruPath + ".tmp", ruPath);
        File.Copy(ruPath, @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua", true);
        Console.WriteLine("Added successfully!");
    }
}
