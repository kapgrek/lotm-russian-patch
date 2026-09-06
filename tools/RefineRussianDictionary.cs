using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class RefineRussianDictionary
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";
        string dataRuPath = @"d:\gameDev\translate lotm\data\RuntimeTextRussian.lua";
        string gameRuPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua";

        Console.WriteLine("Очистка и канонизация словаря RuntimeTextRussian.lua...");

        var orderedKeys = new List<string>();
        var entries = new Dictionary<string, string>(StringComparer.Ordinal);

        string content = File.ReadAllText(ruPath, Encoding.UTF8);
        int pos = 0;
        int len = content.Length;

        while (pos < len)
        {
            int keyStart = content.IndexOf("[\"", pos);
            if (keyStart == -1) break;
            keyStart += 2;

            StringBuilder sbKey = new StringBuilder();
            bool escaped = false;
            int keyEnd = -1;
            for (int i = keyStart; i < len - 1; i++)
            {
                char c = content[i];
                if (escaped) { sbKey.Append(c); escaped = false; }
                else if (c == '\\') { sbKey.Append(c); escaped = true; }
                else if (c == '"' && content[i + 1] == ']') { keyEnd = i; break; }
                else { sbKey.Append(c); }
            }
            if (keyEnd == -1) break;

            int valAssign = content.IndexOf("=", keyEnd + 2);
            if (valAssign == -1) break;
            int valQuoteStart = content.IndexOf("\"", valAssign + 1);
            if (valQuoteStart == -1) break;
            valQuoteStart += 1;

            StringBuilder sbVal = new StringBuilder();
            escaped = false;
            int valEnd = -1;
            for (int i = valQuoteStart; i < len; i++)
            {
                char c = content[i];
                if (escaped) { sbVal.Append(c); escaped = false; }
                else if (c == '\\') { sbVal.Append(c); escaped = true; }
                else if (c == '"') { valEnd = i; break; }
                else { sbVal.Append(c); }
            }
            if (valEnd == -1) break;

            string k = sbKey.ToString();
            string v = sbVal.ToString();

            if (!entries.ContainsKey(k))
            {
                orderedKeys.Add(k);
            }
            entries[k] = v;
            pos = valEnd + 1;
        }

        Console.WriteLine(string.Format("Загружено {0} записей.", entries.Count));

        int hexCleaned = 0;
        int termsRefined = 0;

        for (int i = 0; i < orderedKeys.Count; i++)
        {
            string key = orderedKeys[i];
            string val = entries[key];
            string originalVal = val;

            // 1. Очистка 32-битных хешей Google Translate
            if (Regex.IsMatch(val, @"[a-f0-9]{32}"))
            {
                val = Regex.Replace(val, @"[a-f0-9]{32}", "");
                hexCleaned++;
            }

            // 2. Исправление каноничных терминов и стоек
            val = Regex.Replace(val, @"\b(Знак индукции|знак индукции|индукционной метки|индукционную метку|индукционная метка)\b", "Метка внушения", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(Nightmare Stance|Кошмар Stance|стойка Кошмар|стойке Кошмар)\b", "Стойка кошмара", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(в Стойка кошмара|в Стойку кошмара)\b", "в Стойке кошмара", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(Imagination Stance|позиции воображения|стойка Воображение|стойке Воображение)\b", "Стойка фантазии", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(в Стойка фантазии|в Стойку фантазии)\b", "в Стойке фантазии", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(Offensive Stance|атакующая стойка)\b", "Атакующая стойка", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(в Атакующая стойка)\b", "в Атакующей стойке", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(Defense Form|защитная стойка)\b", "Защитная стойка", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(в Защитная стойка)\b", "в Защитной стойке", RegexOptions.IgnoreCase);

            // 3. Исправление игровых формулировок
            val = Regex.Replace(val, @"\bснижение исцеления\b", "Снижение лечения", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\bснижение урона\b", "Снижение урона", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\bзаточить врагов\b", "накладывая Заточение на врагов", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\bсостояние гипноза\b", "состояние Гипноза", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\bпосле суммирования (\d+) раз\b", "при накоплении $1 уровней", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\bприменить 1 стек\b", "наложить 1 уровень", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\bсек\.\.\b", "сек.", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\bмаг\. урона (\*d|\*f)\b", "$1 маг. урона", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\bфиз\. урона (\*d|\*f)\b", "$1 физ. урона", RegexOptions.IgnoreCase);

            // 4. Канонизация имен и валюты
            val = Regex.Replace(val, @"Кляйн([а-яА-ЯёЁ]*)", "Клейн$1");
            val = Regex.Replace(val, @"кляйн([а-яА-ЯёЁ]*)", "клейн$1");

            val = Regex.Replace(val, @"\bСтарый Нил\b", "Старина Нил");
            val = Regex.Replace(val, @"\bстарый Нил\b", "старина Нил");
            val = Regex.Replace(val, @"\bстарику Нилу\b", "старине Нилу");
            val = Regex.Replace(val, @"\bстарика Нила\b", "старины Нила");

            val = Regex.Replace(val, @"\b(\d+)\s*(солей|соля|соли)\b", "$1 суле", RegexOptions.IgnoreCase);

            val = Regex.Replace(val, @"\b(Потусторонняя характеристика|потусторонняя характеристика|потусторонней характеристики|потустороннюю характеристику|потусторонней характеристике)\b", "Потустороннее свойство", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(Потусторонние характеристики|потусторонние характеристики|потусторонних характеристик)\b", "Потусторонние свойства", RegexOptions.IgnoreCase);

            val = Regex.Replace(val, @"\b(Черный чертополох|Черный Терновник|Черный терновник|Блэкторн)\b", "Чёрный Чертополох", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(семьи Антигонуса|семья Антигонуса|род Антигонуса|рода Антигонуса|Антигонуса|Антигонус)\b", "Антигона", RegexOptions.IgnoreCase);
            val = Regex.Replace(val, @"\b(Райел Бибер|Рейл Бибер|Риэль Бибер)\b", "Райэль Бибер", RegexOptions.IgnoreCase);

            val = val.Replace("\\п", "\\n").Replace("\\т", "\\t").Replace("\\р", "\\r");

            if (val != originalVal)
            {
                entries[key] = val;
                termsRefined++;
            }
        }

        Console.WriteLine(string.Format("Очищено хешей Google Translate: {0}", hexCleaned));
        Console.WriteLine(string.Format("Улучшено и канонизировано записей: {0}", termsRefined));

        // Запись обратно
        using (var sw = new StreamWriter(ruPath, false, new UTF8Encoding(false)))
        {
            sw.WriteLine("-- Generated Russian Translation Dictionary for Lord of Mysteries");
            sw.WriteLine(string.Format("-- Entries: {0}", entries.Count));
            sw.WriteLine("return {");
            foreach (var key in orderedKeys)
            {
                sw.WriteLine(string.Format("    [\"{0}\"] = \"{1}\",", key, entries[key]));
            }
            sw.WriteLine("}");
        }

        File.Copy(ruPath, dataRuPath, true);
        if (File.Exists(gameRuPath) || Directory.Exists(Path.GetDirectoryName(gameRuPath)))
        {
            File.Copy(ruPath, gameRuPath, true);
        }

        Console.WriteLine("Словарь успешно перезаписан и синхронизирован!");
    }
}
