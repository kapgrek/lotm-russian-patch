using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class PatchRemaining3
{
    static string CleanForLua(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        string r = s.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "");
        r = Regex.Replace(r, @"(\\+)\""", "\"");
        r = r.Replace("\"", "\\\"");
        return r;
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string tsvPath = @"tools\true_untranslated_strings.tsv";
        string masterPath = @"RuntimeTextRussian.lua";

        var lines = File.ReadAllLines(tsvPath, Encoding.UTF8);
        var ruTranslations = new Dictionary<string, string>();

        // Line 2: Desire Messenger note
        ruTranslations["1"] = @"\n<Tips stylename=""Letter_Highlight"" u=""1"" id=""#160_R"">На испачканной записке осталось лишь несколько разборчивых строк:</>\n«Я наконец-то понял...»\n«В первом раунде был <Letter_Highlight_HW>номер два</>! Номер два — <Letter_Highlight_HW>настоящий</>!»\n«Посланник Желаний говорил чистую <Letter_Highlight_HW>правду</>, но мы всё равно погибнем...»\n<Hide stylename=""Transparent"" id=""#161_R"">Почерк обрывается</>, <Hide id=""#157"">дальше идёт растёртое тёмное пятно.</>";

        // Line 3: Missing children in Tingen
        ruTranslations["2"] = @"\n……\n\nС начала июля в Тингене участились случаи пропажи детей. Общее число пропавших без вести составляет <Mark id=""#159""> тринадцать человек </>, среди них пять мальчиков и восемь девочек.\nНа данный момент найдены тела троих, убийца задержан, дело закрыто.\nОднако вскрытие показало разные причины смерти, относящиеся к трём не связанным обычным происшествиям.";

        // Line 7: So Greedy
        ruTranslations["6"] = @"Как\nже\nжадно";

        var keysToUpdate = new Dictionary<string, string>(StringComparer.Ordinal);
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split('\t');
            if (parts.Length >= 3 && ruTranslations.ContainsKey(parts[0]))
            {
                string id = parts[0];
                string cn = parts[1];
                string en = parts[2];
                string ru = ruTranslations[id];
                keysToUpdate[cn] = ru;
                keysToUpdate[en] = ru;
            }
        }

        Console.WriteLine("Updating exact keys: " + keysToUpdate.Count);
        var mlines = new List<string>(File.ReadAllLines(masterPath, Encoding.UTF8));
        var keyRegex = new Regex(@"^\s*\[""(.*)""\]\s*=\s*""(.*)"",?\s*$");
        var handled = new HashSet<string>(StringComparer.Ordinal);

        for (int i = 0; i < mlines.Count; i++)
        {
            string line = mlines[i];
            var m = keyRegex.Match(line);
            if (m.Success)
            {
                string rawKey = m.Groups[1].Value.Replace("\\\"", "\"");
                if (keysToUpdate.ContainsKey(rawKey))
                {
                    string newRu = CleanForLua(keysToUpdate[rawKey]);
                    mlines[i] = string.Format("    [\"{0}\"] = \"{1}\",", m.Groups[1].Value, newRu);
                    handled.Add(rawKey);
                }
            }
        }

        int insertPos = mlines.Count - 1;
        foreach (var kvp in keysToUpdate)
        {
            if (!handled.Contains(kvp.Key))
            {
                mlines.Insert(insertPos++, string.Format("    [\"{0}\"] = \"{1}\",", CleanForLua(kvp.Key), CleanForLua(kvp.Value)));
            }
        }

        File.WriteAllLines(masterPath, mlines, Encoding.UTF8);
        Console.WriteLine("Master dictionary patched! Handled: " + handled.Count);
    }
}
