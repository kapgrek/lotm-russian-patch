using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class Audit
{
    static void Main(string[] args)
    {
        string path = args.Length > 0 ? args[0] : @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\Init.lua";
        string text = File.ReadAllText(path, Encoding.UTF8);
        
        var tokens = new List<Tuple<string, int>>();
        int line = 1;
        int i = 0;
        int n = text.Length;

        while (i < n)
        {
            char c = text[i];
            if (c == '\n') { line++; i++; continue; }
            if (char.IsWhiteSpace(c)) { i++; continue; }

            // Comments
            if (c == '-' && i + 1 < n && text[i+1] == '-')
            {
                if (i + 3 < n && text[i+2] == '[' && text[i+3] == '[')
                {
                    i += 4;
                    while (i + 1 < n && !(text[i] == ']' && text[i+1] == ']'))
                    {
                        if (text[i] == '\n') line++;
                        i++;
                    }
                    i += 2;
                }
                else
                {
                    while (i < n && text[i] != '\n') i++;
                }
                continue;
            }

            // Strings
            if (c == '"' || c == '\'')
            {
                char q = c;
                i++;
                while (i < n && text[i] != q)
                {
                    if (text[i] == '\\' && i + 1 < n) { i += 2; continue; }
                    if (text[i] == '\n') line++;
                    i++;
                }
                if (i < n) i++;
                continue;
            }

            // Long bracket strings
            if (c == '[' && i + 1 < n && text[i+1] == '[')
            {
                i += 2;
                while (i + 1 < n && !(text[i] == ']' && text[i+1] == ']'))
                {
                    if (text[i] == '\n') line++;
                    i++;
                }
                if (i + 1 < n) i += 2;
                continue;
            }

            // Identifiers / keywords
            if (char.IsLetter(c) || c == '_')
            {
                int start = i;
                while (i < n && (char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;
                tokens.Add(new Tuple<string, int>(text.Substring(start, i - start), line));
                continue;
            }

            tokens.Add(new Tuple<string, int>(c.ToString(), line));
            i++;
        }

        int depth = 0;
        int rootLocals = 0;
        bool prevWasElseif = false;

        for (int t = 0; t < tokens.Count; t++)
        {
            string tok = tokens[t].Item1;
            int tokLine = tokens[t].Item2;

            if (depth == 0 && tok == "local")
            {
                if (t + 1 < tokens.Count && tokens[t+1].Item1 == "function")
                {
                    rootLocals++;
                    string fn = (t + 2 < tokens.Count) ? tokens[t+2].Item1 : "?";
                    Console.WriteLine(string.Format("Line {0} [Local #{1}]: local function {2}", tokLine, rootLocals, fn));
                }
                else
                {
                    int j = t + 1;
                    var names = new List<string>();
                    while (j < tokens.Count && tokens[j].Item1 != "=" && tokens[j].Item1 != "local" && tokens[j].Item1 != "function")
                    {
                        if (tokens[j].Item1 != "," && char.IsLetter(tokens[j].Item1[0]))
                        {
                            names.Add(tokens[j].Item1);
                        }
                        j++;
                        if (j < tokens.Count && (tokens[j].Item1 == ";" || tokens[j].Item1 == "if" || tokens[j].Item1 == "for")) break;
                    }
                    foreach (var name in names)
                    {
                        rootLocals++;
                        Console.WriteLine(string.Format("Line {0} [Local #{1}]: local {2}", tokLine, rootLocals, name));
                    }
                }
            }

            if (tok == "elseif")
            {
                prevWasElseif = true;
            }
            else if (tok == "then")
            {
                if (!prevWasElseif) depth++;
                prevWasElseif = false;
            }
            else if (tok == "function" || tok == "do" || tok == "repeat")
            {
                depth++;
            }
            else if (tok == "end" || tok == "until")
            {
                depth--;
            }
        }
        Console.WriteLine(string.Format("Final depth: {0}, Total root locals: {1}", depth, rootLocals));
    }
}