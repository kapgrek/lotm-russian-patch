using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class ScanInitLuaOverrides
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string initPath = @"d:\gameDev\translate lotm\data\Init.lua";
        Console.WriteLine("Scanning " + initPath + " for English tables...");

        string[] lines = File.ReadAllLines(initPath, Encoding.UTF8);
        string currentTable = null;
        var tableCounts = new Dictionary<string, int>();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("local ") && line.Contains(" = {"))
            {
                int eq = line.IndexOf(" = {");
                currentTable = line.Substring(6, eq - 6).Trim();
                tableCounts[currentTable] = 0;
            }
            else if (line == "}" || line == "};")
            {
                currentTable = null;
            }
            else if (currentTable != null && (line.StartsWith("[\"") || line.StartsWith("\"") || Regex.IsMatch(line, @"^\w+\s*=")))
            {
                tableCounts[currentTable]++;
            }
        }

        foreach (var kvp in tableCounts)
        {
            if (kvp.Value > 5)
            {
                Console.WriteLine(string.Format("Table '{0}': {1} entries", kvp.Key, kvp.Value));
            }
        }
    }
}
