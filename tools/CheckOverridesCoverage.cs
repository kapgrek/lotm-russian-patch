using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class CheckOverridesCoverage
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string initPath = @"d:\gameDev\translate lotm\data\Init.lua";
        string ruLocPath = @"d:\gameDev\translate lotm\RussianLocalization.lua";
        string ruPath = @"d:\gameDev\translate lotm\RuntimeTextRussian.lua";

        // Read stringConstOverrides from Init.lua
        var initStringConsts = new Dictionary<string, string>();
        var initExactOverrides = new Dictionary<string, string>();

        string[] lines = File.ReadAllLines(initPath, Encoding.UTF8);
        bool inConst = false;
        bool inExact = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("local stringConstOverrides = {")) { inConst = true; continue; }
            if (inConst && line == "}") { inConst = false; continue; }
            if (line.StartsWith("local visibleTextExactOverrides = {")) { inExact = true; continue; }
            if (inExact && line == "}") { inExact = false; continue; }

            if (inConst)
            {
                int eq = line.IndexOf(" = \"");
                if (eq > 0)
                {
                    string key = line.Substring(0, eq).Trim();
                    int valEnd = line.LastIndexOf("\"");
                    string val = line.Substring(eq + 4, valEnd - (eq + 4));
                    initStringConsts[key] = val;
                }
            }

            if (inExact)
            {
                if (line.StartsWith("[\""))
                {
                    int delim = line.IndexOf("\"] =");
                    if (delim > 0)
                    {
                        string key = line.Substring(2, delim - 2);
                        initExactOverrides[key] = "";
                    }
                }
            }
        }

        Console.WriteLine(string.Format("Init.lua stringConstOverrides: {0}", initStringConsts.Count));
        Console.WriteLine(string.Format("Init.lua visibleTextExactOverrides: {0}", initExactOverrides.Count));

        // Read RussianLocalization.lua stringConstOverrides and exact overrides
        string ruLoc = File.ReadAllText(ruLocPath, Encoding.UTF8);

        int missingConsts = 0;
        foreach (var k in initStringConsts.Keys)
        {
            if (!ruLoc.Contains(k + " = \""))
            {
                Console.WriteLine("Missing stringConstOverride in RussianLocalization: " + k + " (EN: " + initStringConsts[k] + ")");
                missingConsts++;
            }
        }
        Console.WriteLine("Total missing stringConstOverrides: " + missingConsts);

        int missingExact = 0;
        foreach (var k in initExactOverrides.Keys)
        {
            if (!ruLoc.Contains("[\"" + k + "\"]"))
            {
                missingExact++;
                if (missingExact <= 15)
                {
                    Console.WriteLine("Missing visibleTextExactOverride: " + k);
                }
            }
        }
        Console.WriteLine("Total missing visibleTextExactOverrides: " + missingExact);
    }
}
