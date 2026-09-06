using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class SimulateInitLoad
{
    static int Main(string[] args)
    {
        string path = args.Length > 0 ? args[0] : @"d:\gameDev\translate lotm\data\Init.lua";
        if (!File.Exists(path))
        {
            Console.WriteLine("File not found: " + path);
            return 1;
        }

        Console.WriteLine("Auditing top-level execution of " + path + "...");
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        var definedGlobals = new HashSet<string>() {
            "assert", "pcall", "xpcall", "require", "import", "type", "tonumber", "tostring",
            "pairs", "ipairs", "unpack", "rawget", "rawset", "setmetatable", "getmetatable",
            "setfenv", "getfenv", "select", "collectgarbage", "table", "string", "math", "os",
            "debug", "_G", "LOMModLoader", "Log", "LaunchLog", "LuaCLogger", "LogLevel",
            "Game", "Enum", "Loader"
        };

        var definedLocals = new HashSet<string>();
        int depth = 0;
        int issues = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            int lineNum = i + 1;

            if (string.IsNullOrEmpty(line) || line.StartsWith("--")) continue;

            // Simple block tracking
            // Note: ignore inside strings/comments
            string codeOnly = line;
            int commentIdx = codeOnly.IndexOf("--");
            if (commentIdx >= 0) codeOnly = codeOnly.Substring(0, commentIdx).Trim();

            // Track do ... end depth
            if (codeOnly == "do" || codeOnly.StartsWith("do ") || codeOnly.EndsWith(" do"))
            {
                depth++;
            }
            if (codeOnly.StartsWith("function ") || (codeOnly.Contains(" = function(") && !codeOnly.EndsWith("end")))
            {
                depth++;
            }
            if (codeOnly == "end" || codeOnly.StartsWith("end)") || codeOnly.StartsWith("end,") || codeOnly.StartsWith("end;"))
            {
                if (depth > 0) depth--;
            }

            // Track local declarations at depth 0
            if (depth == 0 && codeOnly.StartsWith("local "))
            {
                string rest = codeOnly.Substring(6).Trim();
                if (rest.StartsWith("function "))
                {
                    string fnName = rest.Substring(9).Trim();
                    int paren = fnName.IndexOf('(');
                    if (paren > 0) fnName = fnName.Substring(0, paren).Trim();
                    definedLocals.Add(fnName);
                }
                else
                {
                    int eqIdx = rest.IndexOf('=');
                    string decls = eqIdx >= 0 ? rest.Substring(0, eqIdx) : rest;
                    string[] vars = decls.Split(',');
                    foreach (var v in vars)
                    {
                        string name = v.Trim();
                        if (!string.IsNullOrEmpty(name)) definedLocals.Add(name);
                    }
                }
            }

            // At depth 0, check accesses to runtimeFixes.*
            if (depth == 0 && codeOnly.Contains("runtimeFixes."))
            {
                if (!definedLocals.Contains("runtimeFixes"))
                {
                    Console.WriteLine(string.Format("  [CRITICAL ERROR] Line {0}: 'runtimeFixes' accessed before declaration!", lineNum));
                    issues++;
                }
            }
        }

        Console.WriteLine(string.Format("Audit finished: {0} issues found.", issues));
        return issues == 0 ? 0 : 1;
    }
}
