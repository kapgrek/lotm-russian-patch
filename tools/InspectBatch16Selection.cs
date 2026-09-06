using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectBatch16Selection
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

        var seen = new HashSet<string>(StringComparer.Ordinal);

        var list008AndHonorific = new List<Tuple<string, string>>();
        var listWorldAndSynthesis = new List<Tuple<string, string>>();
        var listLifeAndItems = new List<Tuple<string, string>>();
        var listSystemAndUI = new List<Tuple<string, string>>();
        var listRichTextAndStory = new List<Tuple<string, string>>();
        var listNaturalEnglish = new List<Tuple<string, string>>();

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

                        // Исключаем внутренний дебаг и мусор
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") ||
                            enVal.StartsWith(".p4config") || enVal.Contains("-----") || enVal.Contains(".cpp") || enVal.Contains(".h") ||
                            enVal.StartsWith("523") || enVal.StartsWith("520") || enVal.StartsWith("GTA-") || enVal.StartsWith("GTA "))
                            continue;

                        // Классифицируем
                        if (enVal.Contains("0-08") || enVal.Contains("008") || cnKey.Contains("0-08") || cnKey.Contains("008") ||
                            enVal.Contains("Honorific") || cnKey.Contains("尊名") || enVal.Contains("The Fool") || cnKey.Contains("愚者") ||
                            enVal.Contains("Gray Fog") || cnKey.Contains("灰雾"))
                        {
                            list008AndHonorific.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.Contains("Synthesis") || cnKey.Contains("合成") || enVal.Contains("Partner") || cnKey.Contains("伙伴") ||
                            enVal.Contains("Slide Rail") || enVal.Contains("Treasure") || cnKey.Contains("藏宝") || enVal.Contains("Puzzle") ||
                            cnKey.Contains("拼图") || cnKey.Contains("廷根") || enVal.Contains("Tingen"))
                        {
                            listWorldAndSynthesis.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (cnKey.Contains("地板") || cnKey.Contains("家具") || cnKey.Contains("三明治") || cnKey.Contains("药") ||
                            enVal.Contains("sandwich") || enVal.Contains("potion") || enVal.Contains("furniture") || enVal.Contains("recipe") ||
                            enVal.Contains("Obtain") || cnKey.Contains("获得"))
                        {
                            listLifeAndItems.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.StartsWith("<") && enVal.Contains("</>"))
                        {
                            listRichTextAndStory.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.StartsWith("<") || enVal.EndsWith(">") || enVal.Contains("Prompt") || enVal.Contains("Notice") ||
                            enVal.Contains("Tips") || enVal.Contains("Failed") || enVal.Contains("Success") || enVal.Contains("Unlock") || enVal.Contains("Level"))
                        {
                            listSystemAndUI.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.Length > 20 && Regex.IsMatch(enVal, @"[a-zA-Z]{3,}") && !enVal.Contains("_"))
                        {
                            listNaturalEnglish.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("0-08, Honorific, Fool, Grey Fog: {0}", list008AndHonorific.Count));
        Console.WriteLine(string.Format("World Activities, Partner Synthesis, Puzzles, Tingen: {0}", listWorldAndSynthesis.Count));
        Console.WriteLine(string.Format("Life Skills, Furniture, Potions, Items: {0}", listLifeAndItems.Count));
        Console.WriteLine(string.Format("RichText Dialogues & Stories: {0}", listRichTextAndStory.Count));
        Console.WriteLine(string.Format("System UI, Prompts, Tips: {0}", listSystemAndUI.Count));
        Console.WriteLine(string.Format("Natural English Phrases (>20 chars): {0}", listNaturalEnglish.Count));

        Console.WriteLine("\n--- Samples RichText Dialogues & Stories (First 15) ---");
        for (int i = 0; i < Math.Min(15, listRichTextAndStory.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] EN: {1} | CN: {2}", i, listRichTextAndStory[i].Item2, listRichTextAndStory[i].Item1));
        }

        Console.WriteLine("\n--- Samples Natural English Phrases (First 15) ---");
        for (int i = 0; i < Math.Min(15, listNaturalEnglish.Count); i++)
        {
            Console.WriteLine(string.Format("[{0}] EN: {1} | CN: {2}", i, listNaturalEnglish[i].Item2, listNaturalEnglish[i].Item1));
        }
    }
}
