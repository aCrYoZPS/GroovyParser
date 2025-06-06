using GroovyParserBackend.Entities;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace GroovyParserBackend
{
    using TokenDict = Dictionary<Token, int>;
    public class Parser
    {
        public static List<Token> GetNormalisedSwitch(List<Token> tokens)
        {
            Stack<Token> switchStack = new Stack<Token>();
            var braceCount = 0;
            var currentBraceLevel = new Stack<int>();
            var switchBrace = false;
            var result = new List<Token>();
            for (int i = 0; i < tokens.Count; ++i)
            {
                var token = tokens[i];
                if (token.Type == TokenType.Keyword && token.Value == "switch")
                {
                    switchStack.Push(token);
                    switchBrace = true;
                }
                else if (token.Type == TokenType.OpenBrace)
                {
                    braceCount += 1;

                    if (switchBrace)
                    {
                        currentBraceLevel.Push(braceCount);
                        switchBrace = false;
                    }
                }
                else if (token.Type == TokenType.Braces)
                {
                    if (currentBraceLevel.Count > 0 && braceCount > currentBraceLevel.Peek())
                    {
                        braceCount -= 1;
                        result.Add(token);
                        continue;
                    }
                    result.Add(token);
                    if (i < tokens.Count - 1 && tokens[i + 1].Type == TokenType.Keyword && (tokens[i + 1].Value == "case" || tokens[i + 1].Value == "default"))
                    {
                        i++;
                        switchStack.Peek().elseIfSequence.Add(tokens[i]);
                    }
                    else
                    {
                        if (switchStack.Count != 0)
                        {
                            switchStack.Pop();
                            currentBraceLevel.Pop();
                        }
                    }
                    continue;
                }
                else if (token.Type == TokenType.Keyword && token.Value == "case")
                {
                    switchStack.Peek().elseIfSequence.Add(tokens[i]);
                    continue;
                }
                result.Add(token);
            }
            return result;
        }
        public static List<Token> GetNormalisedIfs(List<Token> tokens)
        {
            tokens = GetNormalisedSwitch(tokens);
            Stack<Token> ifStack = new Stack<Token>();
            var braceCount = 0;
            var currentBraceLevel = new Stack<int>();
            var ifBrace = false;
            var result = new List<Token>();
            var ignoreNextBrace = false;

            for (int i = 0; i < tokens.Count; ++i)
            {
                var token = tokens[i];
                if (token.Type == TokenType.Keyword && token.Value == "if")
                {
                    ifStack.Push(token);
                    ifBrace = true;
                }
                else if (token.Type == TokenType.OpenBrace)
                {
                    if (ignoreNextBrace)
                    {
                        ignoreNextBrace = false;
                        continue;
                    }
                    braceCount += 1;
                    if (ifBrace)
                    {
                        currentBraceLevel.Push(braceCount);
                        ifBrace = false;
                    }
                    continue;
                }
                else if (token.Type == TokenType.Braces)
                {
                    if (currentBraceLevel.Count > 0 && braceCount > currentBraceLevel.Peek())
                    {
                        braceCount -= 1;
                        result.Add(token);
                        continue;
                    }
                    result.Add(token);
                    if (i < tokens.Count - 1 && tokens[i + 1].Type == TokenType.Keyword && tokens[i + 1].Value == "else")
                    {
                        i++;
                        ifStack.Peek().elseIfSequence.Add(tokens[i]);
                        if (i < tokens.Count - 1 && tokens[i + 1].Type == TokenType.Keyword && tokens[i + 1].Value == "if")
                        {
                            i++;
                            ifStack.Peek().elseIfSequence.Add(tokens[i]);
                        }
                        ignoreNextBrace = true;
                    }
                    else
                    {
                        if (ifStack.Count != 0)
                        {
                            ifStack.Pop();
                        }
                    }
                    continue;
                }
                result.Add(token);
            }
            return result;
        }
        public static HalsteadMetrics GetBasicMetrics(List<Token> tokens)
        {
            var operandOperatorDicts = new Tuple<TokenDict, TokenDict>(new TokenDict(), new TokenDict());
            for (int i = 0; i < tokens.Count; ++i)
            {
                var token = tokens[i];
                if (token.Type == TokenType.FunctionCall && token.Value.StartsWith("print"))
                {
                    if (tokens[i + 1].Type == TokenType.Identifier)
                    {
                        tokens[i + 1].Status.IsIO = true;
                        ++i;
                    }
                }
                else if (token.Type == TokenType.Keyword && token.Value == "return")
                {
                    if (tokens[i + 1].Type == TokenType.Identifier)
                    {
                        tokens[i + 1].Status.IsModified = true;
                        ++i;
                    }

                }
            }
            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Identifier:
                        if (token.Value.StartsWith("System") || token.Value.StartsWith("File") ||
                        token.Value.StartsWith("Console"))
                            continue;

                        if (operandOperatorDicts.Item1.ContainsKey(token))
                        {
                            var count = operandOperatorDicts.Item1[token];
                            var prevKey = operandOperatorDicts.Item1.First(x => x.Value == count && x.Key.Equals(token)).Key;
                            operandOperatorDicts.Item1.Remove(token);
                            token.Status.IsControl = prevKey.Status.IsControl || token.Status.IsControl;
                            token.Status.IsModified = prevKey.Status.IsModified || token.Status.IsModified;
                            token.Status.IsIO = prevKey.Status.IsIO || token.Status.IsIO;
                            token.Status.IsInput = prevKey.Status.IsInput || token.Status.IsInput;
                            token.Status.IsParasite = prevKey.Status.IsParasite || token.Status.IsParasite;
                            operandOperatorDicts.Item1[token] = count + 1;
                        }
                        else
                        {
                            operandOperatorDicts.Item1[token] = 1;
                        }
                        break;
                    case TokenType.NumberLiteral:
                    case TokenType.StringLiteral:
                        if (!operandOperatorDicts.Item1.TryAdd(token, 1))
                        {
                            operandOperatorDicts.Item1[token] += 1;
                        }
                        break;
                    default:
                        if (token.Type == TokenType.Keyword && Tokenizer.operandKeywords.Contains(token.Value))
                        {
                            if (!operandOperatorDicts.Item1.TryAdd(token, 1))
                            {
                                operandOperatorDicts.Item1[token] += 1;
                            }

                        }
                        else if (!operandOperatorDicts.Item2.TryAdd(token, 1))
                        {
                            operandOperatorDicts.Item2[token] += 1;
                        }
                        break;
                }
            }

            return new HalsteadMetrics
            {
                operatorCount = operandOperatorDicts.Item2.Values.Sum(),
                operandCount = operandOperatorDicts.Item1.Values.Sum(),
                uniqueOperatorCount = operandOperatorDicts.Item2.Keys.Count,
                uniqueOperandCount = operandOperatorDicts.Item1.Keys.Count,
                operandDict = operandOperatorDicts.Item1,
                operatorDict = operandOperatorDicts.Item2,
            };
        }
        public static Dictionary<List<Token>, int> GetIdentificators(TokenDict tokenDict, bool isIO = false)
        {
            var tokens = tokenDict.Keys.Where(token => token.Type == TokenType.Identifier).ToList();
            var dict = new Dictionary<List<Token>, int>();

            if (isIO)
            {
                tokens = tokens.Where(token => token.Status.IsIO).ToList();
            }

            List<Token> control, modified, io, parasite, input;
            control = new();
            modified = new();
            io = new();
            parasite = new();
            input = new();

            foreach (var token in tokens)
            {
                if (token.Status.IsControl)
                {
                    control.Add(token);
                }
                else if (token.Status.IsInput)
                {
                    input.Add(token);
                }
                else if (token.Status.IsModified)
                {
                    modified.Add(token);
                }
                else if (token.Status.IsIO)
                {
                    io.Add(token);
                }
                else if (token.Value != "")
                {
                    parasite.Add(token);
                }

            }
            if (control.Any())
            {
                dict[control] = control.Count;
            }
            if (input.Any())
            {
                dict[input] = input.Count;
            }
            if (modified.Any())
            {
                dict[modified] = modified.Count;
            }
            if (io.Any())
            {
                dict[io] = io.Count;
            }
            if (parasite.Any())
            {
                dict[parasite] = parasite.Count;
            }
            return dict;

        }

        public static double GetChepinMetric(List<Token> tokens)
        {
            var control = 0;
            var io = 0;
            var modified = 0;
            var parasite = 0;
            foreach (var token in tokens)
            {
                if (token.Status.IsControl)
                {
                    control += 1;
                }
                else if (token.Status.IsModified)
                {
                    modified += 1;
                }
                else if (token.Status.IsIO)
                {
                    io += 1;
                }
                else
                {
                    parasite += 1;
                }
            }
            return io + 2 * modified + 3 * control + 0.5 * parasite;
        }

        public static TokenDict GetSpans(List<Token> tokens)
        {
            TokenDict spans = new();
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.Identifier ||
                    token.Type == TokenType.FunctionCall ||
                    token.Type == TokenType.SubscriptOperator)
                {
                    Console.WriteLine($"{token.Value} : {token.Type}");
                    if (token.Type == TokenType.FunctionCall && !token.Value.Contains('.') ||
                        token.Value.StartsWith("System") || token.Value.StartsWith("File") ||
                        token.Value.StartsWith("Console"))
                        continue;

                    if (token.Value.Contains('.'))
                    {
                        token.Value = token.Value.Split('.')[0];
                    }
                    if (token.Value.Contains('['))
                    {
                        token.Value = token.Value.Split('[')[0];
                    }
                    if (token.Value == "")
                        continue;

                    token.Type = TokenType.Identifier;
                    if (!spans.TryAdd(token, 1))
                    {
                        spans[token] += 1;
                    }
                }
            }
            foreach (var key in spans.Keys.ToList())
            {
                spans[key] -= 1;
                if (spans[key] == 0)
                {
                    spans.Remove(key);
                }
            }
            return spans;
        }
        public static DerivedMetrics GetDerivedMetrics(HalsteadMetrics metrics)
        {
            int dictionary = metrics.uniqueOperatorCount + metrics.uniqueOperandCount;
            int length = metrics.operandCount + metrics.operatorCount;
            double volume = Math.Log2(dictionary) * length;

            return new DerivedMetrics
            {
                dictionary = dictionary,
                length = length,
                volume = Math.Ceiling(volume),
            };
        }
    }
}
