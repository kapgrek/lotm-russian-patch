using System;
using System.IO;
using System.Text;

class FixInitLocals
{
    static int Main(string[] args)
    {
        string filePath = args.Length > 0 ? args[0] : @"d:\gameDev\translate lotm\data\Init.lua";
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found: " + filePath);
            return 1;
        }

        string text = File.ReadAllText(filePath, Encoding.UTF8);

        // First, check if line 845 has '-- runtimeFixes declared above' and restore 'local runtimeFixes = {}'
        if (text.Contains("-- runtimeFixes declared above\n\ndo"))
        {
            text = text.Replace("-- runtimeFixes declared above\n\ndo", "local runtimeFixes = {}\n\ndo");
            Console.WriteLine("Restored local runtimeFixes = {} at line 845.");
        }
        else if (text.Contains("-- runtimeFixes declared above\r\n\r\ndo"))
        {
            text = text.Replace("-- runtimeFixes declared above\r\n\r\ndo", "local runtimeFixes = {}\r\n\r\ndo");
            Console.WriteLine("Restored local runtimeFixes = {} at line 845.");
        }

        // Clean up the extra unmatched '}' near runtimeFixes.panelTextRepair
        string extraBrace1 = "runtimeFixes.panelTextRepair = panelTextRepair\r\n}\r\n";
        if (text.Contains(extraBrace1))
        {
            text = text.Replace(extraBrace1, "runtimeFixes.panelTextRepair = panelTextRepair\r\n");
            Console.WriteLine("Removed extra brace on Windows newlines.");
        }
        string extraBrace2 = "runtimeFixes.panelTextRepair = panelTextRepair\n}\n";
        if (text.Contains(extraBrace2))
        {
            text = text.Replace(extraBrace2, "runtimeFixes.panelTextRepair = panelTextRepair\n");
            Console.WriteLine("Removed extra brace on Unix newlines.");
        }

        // Write output
        File.WriteAllText(filePath, text, new UTF8Encoding(false));
        Console.WriteLine("Successfully updated: " + filePath);
        return 0;
    }
}
