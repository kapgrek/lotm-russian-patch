using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class SelectBatch17
{
    static string GeminiPath = @"d:\gameDev\translate lotm\source_en\RuntimeTextGemini.lua";
    static string RuPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var existingRu = new HashSet<string>(StringComparer.Ordinal);
        if (File.Exists(RuPath))
        {
            foreach (var line in File.ReadAllLines(RuPath, Encoding.UTF8))
            {
                string t = line.Trim();
                if (t.StartsWith("[\"") && (t.EndsWith("\",") || t.EndsWith("\"")))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 0)
                    {
                        string k = t.Substring(2, delim - 2);
                        existingRu.Add(k);
                    }
                }
            }
        }

        var candidates = new List<Tuple<string, string>>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        using (var reader = new StreamReader(GeminiPath, Encoding.UTF8))
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
                        string cnKey = t.Substring(2, delim - 2);
                        int valStart = delim + 6;
                        int valEnd = t.EndsWith("\",") ? t.Length - 2 : t.Length - 1;
                        string enVal = valEnd >= valStart ? t.Substring(valStart, valEnd - valStart) : "";

                        if (existingRu.Contains(cnKey) || existingRu.Contains(enVal) || seen.Contains(cnKey) || seen.Contains(enVal))
                            continue;

                        seen.Add(cnKey);
                        seen.Add(enVal);

                        if (string.IsNullOrWhiteSpace(enVal) || Regex.IsMatch(enVal, @"^\d+$"))
                            continue;

                        // Фильтруем технический мусор движка/сервера
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") ||
                            enVal.StartsWith(".p4config") || enVal.Contains("-----") || enVal.Contains(".cpp") || enVal.Contains(".h") ||
                            enVal.StartsWith("523") || enVal.StartsWith("520") || enVal.StartsWith("GTA-") || enVal.StartsWith("GTA ") ||
                            enVal.StartsWith("Camera_") || enVal.StartsWith("Mat_") || enVal.StartsWith("SK_") || enVal.StartsWith("SM_") ||
                            enVal.StartsWith("SkillID: ") || enVal.Contains("InterruptMode") || enVal.Contains("BindSkillID") ||
                            Regex.IsMatch(enVal, @"^\d+:\s*(Disable|Enable|Hide)"))
                            continue;

                        // Отбираем живой текст игры
                        bool isStory = enVal.Length > 20 && (enVal.Contains("\"") || enVal.Contains("!") || enVal.Contains("?") || enVal.Contains("...") ||
                                       enVal.Contains("—") || enVal.Contains("Goddess") || enVal.Contains("Lord") || enVal.Contains("Church") ||
                                       enVal.Contains("Tingen") || enVal.Contains("Backlund") || enVal.Contains("Beyonder") || enVal.Contains("Potion") ||
                                       enVal.Contains("Artifact") || enVal.Contains("Mr.") || enVal.Contains("Miss") || enVal.Contains("you") ||
                                       enVal.Contains("You") || enVal.Contains("I ") || enVal.Contains("We ") || enVal.Contains("They ") ||
                                       enVal.Contains("the ") || enVal.Contains("The ") || enVal.Contains("of ") || enVal.Contains("in ") ||
                                       enVal.Contains("is ") || enVal.Contains("to ") || enVal.Contains("and "));

                        bool isCombatOrSkill = enVal.Contains("HyperLink") || enVal.Contains("damage") || enVal.Contains("Damage") ||
                                               enVal.Contains("Cooldown") || enVal.Contains("cooldown") || enVal.Contains("Slash") ||
                                               enVal.Contains("Dusk") || enVal.Contains("Statue") || enVal.Contains("Mutated") ||
                                               enVal.Contains("mutated");

                        bool isUI = enVal.Length >= 4 && !enVal.Contains("_") && !enVal.Contains("/") &&
                                    (enVal.Contains(" ") || char.IsUpper(enVal[0])) &&
                                    Regex.IsMatch(enVal, @"^[A-Za-z0-9\s,\.\-'\?!%]+$") &&
                                    !Regex.IsMatch(enVal, @"^\d+$");

                        if (isStory || isCombatOrSkill || isUI)
                        {
                            candidates.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Selected candidates count: {0}", candidates.Count));
        for (int i = 0; i < Math.Min(20, candidates.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] EN: {1} | CN: {2}", i, candidates[i].Item2, candidates[i].Item1));
        }
    }
}
