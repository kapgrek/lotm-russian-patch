using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class InspectBatch16Details
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

        var group008 = new List<Tuple<string, string>>();
        var groupHonorific = new List<Tuple<string, string>>();
        var groupWorldActivities = new List<Tuple<string, string>>();
        var groupLifeAndItems = new List<Tuple<string, string>>();
        var groupUIAndSystem = new List<Tuple<string, string>>();
        var groupDialogueAndStory = new List<Tuple<string, string>>();
        var groupOtherMeaningful = new List<Tuple<string, string>>();

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

                        // Исключаем системный шум
                        if (enVal.StartsWith("07_") || enVal.StartsWith("08_") || enVal.Contains("ConfigID") || enVal.Contains("3DConfigID") ||
                            enVal.EndsWith(".lua") || enVal.EndsWith(".uasset") || enVal.Contains("AI Bot - Mechanism") || enVal.Contains("ALS Jogging") ||
                            enVal.StartsWith(">>") || enVal.Contains("cannot be overridden") || enVal.Contains("animation ends") || enVal.Contains("motion matching") ||
                            enVal.StartsWith("[Discarded]") || enVal.StartsWith("CBT2") || enVal.StartsWith("dj") || enVal.Contains("PhyAtk") || enVal.Contains("Jizhi") ||
                            enVal.StartsWith(".p4config") || enVal.Contains("-----") || enVal.Contains(".cpp") || enVal.Contains(".h"))
                            continue;

                        // Классификация
                        if (enVal.Contains("0-08") || enVal.Contains("008") || cnKey.Contains("0-08") || cnKey.Contains("008"))
                        {
                            group008.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.Contains("Honorific") || cnKey.Contains("尊名") || enVal.Contains("The Fool") || cnKey.Contains("愚者") || enVal.Contains("Gray Fog") || cnKey.Contains("灰雾"))
                        {
                            groupHonorific.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.Contains("Synthesis") || cnKey.Contains("合成") || enVal.Contains("Partner") || cnKey.Contains("伙伴") || enVal.Contains("Slide Rail") || enVal.Contains("Treasure") || cnKey.Contains("藏宝") || enVal.Contains("Puzzle") || cnKey.Contains("拼图") || cnKey.Contains("廷根") || enVal.Contains("Tingen"))
                        {
                            groupWorldActivities.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (cnKey.Contains("地板") || cnKey.Contains("家具") || cnKey.Contains("三明治") || cnKey.Contains("药") || enVal.Contains("sandwich") || enVal.Contains("potion") || enVal.Contains("furniture") || enVal.Contains("recipe") || enVal.Contains("Obtain") || cnKey.Contains("获得"))
                        {
                            groupLifeAndItems.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.Length > 40 && !enVal.Contains("_"))
                        {
                            groupDialogueAndStory.Add(Tuple.Create(cnKey, enVal));
                        }
                        else if (enVal.StartsWith("<") || enVal.EndsWith(">") || enVal.Contains("Prompt") || enVal.Contains("Notice") || enVal.Contains("Tips") || enVal.Contains("Failed") || enVal.Contains("Success") || enVal.Contains("Unlock") || enVal.Contains("Level"))
                        {
                            groupUIAndSystem.Add(Tuple.Create(cnKey, enVal));
                        }
                        else
                        {
                            groupOtherMeaningful.Add(Tuple.Create(cnKey, enVal));
                        }
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Group 0-08: {0}", group008.Count));
        Console.WriteLine(string.Format("Group Honorific & Fog: {0}", groupHonorific.Count));
        Console.WriteLine(string.Format("Group World Activities & Synthesis & Tingen: {0}", groupWorldActivities.Count));
        Console.WriteLine(string.Format("Group Life & Items & Recipes: {0}", groupLifeAndItems.Count));
        Console.WriteLine(string.Format("Group Dialogue & Story (>40 chars): {0}", groupDialogueAndStory.Count));
        Console.WriteLine(string.Format("Group UI & System Prompts: {0}", groupUIAndSystem.Count));
        Console.WriteLine(string.Format("Group Other Meaningful: {0}", groupOtherMeaningful.Count));

        Console.WriteLine("\n--- Samples 0-08 ---");
        for (int i = 0; i < Math.Min(10, group008.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", group008[i].Item2, group008[i].Item1));

        Console.WriteLine("\n--- Samples Honorific ---");
        for (int i = 0; i < Math.Min(10, groupHonorific.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", groupHonorific[i].Item2, groupHonorific[i].Item1));

        Console.WriteLine("\n--- Samples World Activities ---");
        for (int i = 0; i < Math.Min(10, groupWorldActivities.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", groupWorldActivities[i].Item2, groupWorldActivities[i].Item1));

        Console.WriteLine("\n--- Samples Dialogue & Story ---");
        for (int i = 0; i < Math.Min(10, groupDialogueAndStory.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", groupDialogueAndStory[i].Item2, groupDialogueAndStory[i].Item1));

        Console.WriteLine("\n--- Samples UI & System ---");
        for (int i = 0; i < Math.Min(10, groupUIAndSystem.Count); i++)
            Console.WriteLine(string.Format("EN: {0} | CN: {1}", groupUIAndSystem[i].Item2, groupUIAndSystem[i].Item1));
    }
}
