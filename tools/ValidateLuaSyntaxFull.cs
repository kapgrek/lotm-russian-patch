using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class ValidateLuaSyntaxFull
{
    static int Main(string[] args)
    {
        string path = args.Length > 0 ? args[0] : @"d:\gameDev\translate lotm\data\Init.lua";
        if (!File.Exists(path))
        {
            Console.WriteLine("File not found: " + path);
            return 1;
        }

        string text = File.ReadAllText(path, Encoding.UTF8);
        int n = text.Length;
        int i = 0;
        int line = 1;

        var parenStack = new Stack<Tuple<char, int>>();
        int depth = 0;
        int rootLocals = 0;
        bool prevWasElseif = false;
        int errors = 0;

        while (i < n)
        {
            char c = text[i];
            if (c == '\n') { line++; i++; continue; }
            if (char.IsWhiteSpace(c)) { i++; continue; }

            // Comments
            if (c == '-' && i + 1 < n && text[i + 1] == '-')
            {
                if (i + 3 < n && text[i + 2] == '[' && text[i + 3] == '[')
                {
                    i += 4;
                    while (i + 1 < n && !(text[i] == ']' && text[i + 1] == ']'))
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

            // String literals
            if (c == '"' || c == '\'')
            {
                char q = c;
                i++;
                while (i < n && text[i] != q)
                {
                    if (text[i] == '\\' && i + 1 < n)
                    {
                        i += 2;
                        continue;
                    }
                    if (text[i] == '\n') line++;
                    i++;
                }
                if (i < n) i++;
                continue;
            }

            // Long bracket strings [=[ ... ]=] or [[ ... ]]
            if (c == '[' && i + 1 < n && (text[i + 1] == '[' || text[i + 1] == '='))
            {
                int eqCount = 0;
                int j = i + 1;
                while (j < n && text[j] == '=') { eqCount++; j++; }
                if (j < n && text[j] == '[')
                {
                    i = j + 1;
                    string closing = "]" + new string('=', eqCount) + "]";
                    while (i + closing.Length <= n && text.Substring(i, closing.Length) != closing)
                    {
                        if (text[i] == '\n') line++;
                        i++;
                    }
                    i += closing.Length;
                    continue;
                }
            }

            // Parens / Braces / Brackets
            if (c == '(' || c == '{' || c == '[')
            {
                parenStack.Push(new Tuple<char, int>(c, line));
                i++;
                continue;
            }
            if (c == ')' || c == '}' || c == ']')
            {
                if (parenStack.Count == 0)
                {
                    Console.WriteLine(string.Format("Line {0}: Unmatched closing delimiter '{1}'", line, c));
                    errors++;
                }
                else
                {
                    var top = parenStack.Pop();
                    char expected = top.Item1 == '(' ? ')' : (top.Item1 == '{' ? '}' : ']');
                    if (c != expected)
                    {
                        Console.WriteLine(string.Format("Line {0}: Mismatched delimiter: expected '{1}' (opened at line {2}) but found '{3}'", line, expected, top.Item2, c));
                        errors++;
                    }
                }
                i++;
                continue;
            }

            // Words / identifiers
            if (char.IsLetter(c) || c == '_')
            {
                int start = i;
                while (i < n && (char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;
                string word = text.Substring(start, i - start);

                if (depth == 0 && word == "local")
                {
                    rootLocals++;
                }

                if (word == "elseif")
                {
                    prevWasElseif = true;
                }
                else if (word == "then")
                {
                    if (!prevWasElseif) depth++;
                    prevWasElseif = false;
                }
                else if (word == "function" || word == "do" || word == "repeat")
                {
                    depth++;
                }
                else if (word == "end" || word == "until")
                {
                    depth--;
                    if (depth < 0)
                    {
                        Console.WriteLine(string.Format("Line {0}: Unexpected '{1}' (depth < 0)", line, word));
                        errors++;
                        depth = 0;
                    }
                }
                continue;
            }

            i++;
        }

        while (parenStack.Count > 0)
        {
            var unclosed = parenStack.Pop();
            Console.WriteLine(string.Format("Line {0}: Unclosed delimiter '{1}'", unclosed.Item2, unclosed.Item1));
            errors++;
        }

        if (depth != 0)
        {
            Console.WriteLine(string.Format("Unbalanced block depth at EOF: {0}", depth));
            errors++;
        }

        Console.WriteLine(string.Format("Validation complete: Errors={0}, RootLocals={1}, BlockDepth={2}", errors, rootLocals, depth));
        return errors == 0 ? 0 : 1;
    }
}
