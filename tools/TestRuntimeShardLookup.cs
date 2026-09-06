using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class TestRuntimeShardLookup
{
    static string SourceKey(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        uint hash = 2166136261u;
        for (int i = 0; i < bytes.Length; i++)
        {
            hash ^= bytes[i];
            hash = unchecked(hash + (hash << 1) + (hash << 4) + (hash << 7) + (hash << 8) + (hash << 24));
        }
        return bytes.Length.ToString() + ":" + hash.ToString("x8");
    }

    static string GetPrefix(string key)
    {
        int colon = key.IndexOf(':');
        if (colon < 0 || colon + 4 > key.Length) return null;
        string hashPrefix = key.Substring(colon + 1, 3);
        int num = Convert.ToInt32(hashPrefix, 16);
        return (num / 4).ToString("x3");
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string shardsDir = @"d:\gameDev\translate lotm\data\shards";

        string[] testKeys = new string[] {
            "Releases Mind Fire, with a *d** base probability of applying an Induction Mark to the enemy target. The target suffers mental burn damage equal to mul(spellfielddisc(86023010),*f,100)% attack power every 1.5 seconds, lasting 10 seconds.",
            "释放心灵之火，以*d**的基础概率使敌方目标受到诱导印记，每1.5秒受到mul(spellfielddisc(86023010),*f,100)%攻击力的心灵灼烧伤害，持续10秒。",
            "Hoy University Rowing Team is recruiting!",
            "Spirit Body Threads",
            "Training Dummy",
            "木桩训练",
            "安迪哥努斯笔记",
            "Antigonus Notebook"
        };

        Console.WriteLine("==========================================================");
        Console.WriteLine("     Тестирование поиска в шардах по алгоритму Init.lua    ");
        Console.WriteLine("==========================================================");

        int passed = 0;
        foreach (var key in testKeys)
        {
            string sKey = SourceKey(key);
            string prefix = GetPrefix(sKey);
            string shardFile = Path.Combine(shardsDir, "RuntimeTextGemini_" + prefix + ".lua");

            if (!File.Exists(shardFile))
            {
                Console.WriteLine(string.Format("❌ Шард {0} не найден для: {1}", prefix, key.Substring(0, Math.Min(40, key.Length))));
                continue;
            }

            string ruFound = null;
            foreach (var line in File.ReadAllLines(shardFile, Encoding.UTF8))
            {
                string t = line.Trim();
                if (t.StartsWith("[\""))
                {
                    int delim = t.IndexOf("\"] = \"");
                    if (delim > 2)
                    {
                        string k = t.Substring(2, delim - 2);
                        string unescapedK = k.Replace("\\\"", "\"").Replace("\\\\", "\\");
                        if (unescapedK == key)
                        {
                            int valStart = delim + 6;
                            int valEnd = t.EndsWith("\",") ? t.Length - 2 : (t.EndsWith("\"") ? t.Length - 1 : -1);
                            if (valEnd >= valStart)
                            {
                                ruFound = t.Substring(valStart, valEnd - valStart).Replace("\\\"", "\"");
                                break;
                            }
                        }
                    }
                }
            }

            if (ruFound != null)
            {
                passed++;
                Console.WriteLine(string.Format("✅ [Шард {0}] \"{1}\"", prefix, key.Length > 35 ? key.Substring(0, 35) + "..." : key));
                Console.WriteLine(string.Format("   -> \"{0}\"", ruFound.Length > 60 ? ruFound.Substring(0, 60) + "..." : ruFound));
            }
            else
            {
                Console.WriteLine(string.Format("⚠️ [Шард {0}] Не найдено в шарде для: \"{1}\"", prefix, key.Length > 35 ? key.Substring(0, 35) + "..." : key));
            }
        }

        Console.WriteLine(string.Format("\nИтог: Найдено {0}/{1} тестовых фраз в шардах.", passed, testKeys.Length));
    }
}
