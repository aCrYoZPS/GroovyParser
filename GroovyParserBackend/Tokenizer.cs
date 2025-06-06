using GroovyParserBackend.Entities;

namespace GroovyParserBackend
{
    public class Tokenizer
    {
        public static List<Token> Tokenize(string sourceCode)
        {
            var tokens = new List<Token>();
            var value = string.Empty;
            var considerNextIdentifier = false;
            var isMember = false;
            var isTripleQuotes = false;
            var isIfFor = false;
            var isLHS = true; // Left Hand Side == lvalue
            var considerNextType = false;
            TokenType previousToken = TokenType.None;
            TokenType type = TokenType.None;
            for (int pos = 0; pos < sourceCode.Length; ++pos)
            {
                var ch = sourceCode[pos];
                switch (ch)
                {
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '0':
                        if (type != TokenType.Identifier)
                        {
                            type = TokenType.NumberLiteral;
                        }
                        value += ch;
                        break;
                    case ',':
                        if (keywords.Contains(value))
                        {
                            type = TokenType.Keyword;
                        }
                        tokens.Add(new Token
                        {
                            Value = (value == string.Empty) ? "," : value,
                            Type = type,
                        });
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case ')':
                        break;
                    case '(':
                        var innerStr = string.Empty;
                        var parenthesesCounter = 1;
                        List<Token> innerTokens;

                        if (isIfFor)
                        {
                            isIfFor = false;
                            if (sourceCode[pos] != ')')
                            {
                                while (pos != sourceCode.Length - 1 && parenthesesCounter != 0)
                                {
                                    innerStr += sourceCode[pos];

                                    if (sourceCode[pos + 1] == '(')
                                        ++parenthesesCounter;

                                    if (sourceCode[pos + 1] == ')')
                                        --parenthesesCounter;
                                    pos++;
                                }
                            }

                            innerTokens = Tokenize(innerStr);

                            foreach (var token in innerTokens)
                            {
                                token.Status.IsControl = true;
                            }

                            tokens.AddRange(innerTokens);

                            break;
                        }

                        if (type == TokenType.Identifier)
                        {
                            if (IsOperatorParentheses(value))
                            {
                                type = TokenType.Keyword;
                                tokens.Add(new Token
                                {
                                    Value = value,
                                    Type = type,
                                });
                                pos++;
                                if (sourceCode[pos] != ')')
                                {
                                    while (pos != sourceCode.Length - 1 && parenthesesCounter != 0)
                                    {
                                        innerStr += sourceCode[pos];

                                        if (sourceCode[pos + 1] == '(')
                                            ++parenthesesCounter;

                                        if (sourceCode[pos + 1] == ')')
                                            --parenthesesCounter;
                                        pos++;
                                    }
                                }

                                innerTokens = Tokenize(innerStr);

                                foreach (var token in innerTokens)
                                {
                                    token.Status.IsControl = true;
                                }

                                tokens.AddRange(innerTokens);

                                previousToken = type;
                                value = string.Empty;
                                type = TokenType.None;
                                break;
                            }

                            type = TokenType.FunctionCall;
                            if (considerNextIdentifier)
                            {
                                considerNextIdentifier = false;
                            }

                            tokens.Add(new Token()
                            {
                                Type = type,
                                Value = value + "()",
                            });

                            if (!isLHS && value.StartsWith("System.console.readLine"))
                            {
                                var i = tokens.Count - 1;
                                while (tokens[i].Type != TokenType.Identifier)
                                {
                                    i--;
                                }
                                tokens[i].Status.IsIO = true;
                                tokens[i].Status.IsInput = true;
                            }
                        }
                        else
                        {
                            type = TokenType.Parentheses;
                            tokens.Add(new Token()
                            {
                                Type = type,
                                Value = "()",
                            });
                        }

                        pos++;
                        innerStr = string.Empty;

                        parenthesesCounter = 1;
                        if (pos <= sourceCode.Length - 1 && sourceCode[pos] != ')')
                        {
                            while (pos != sourceCode.Length - 1 && parenthesesCounter != 0)
                            {
                                innerStr += sourceCode[pos];

                                if (sourceCode[pos + 1] == '(')
                                    ++parenthesesCounter;

                                if (sourceCode[pos + 1] == ')')
                                    --parenthesesCounter;
                                pos++;
                            }
                        }

                        innerTokens = Tokenize(innerStr);

                        if (type == TokenType.FunctionCall || type == TokenType.Parentheses)
                        {
                            foreach (var token in innerTokens)
                            {
                                token.Status.IsModified = true;
                            }
                        }
                        if (type == TokenType.FunctionCall && value.Contains("print"))
                        {
                            foreach (var token in innerTokens)
                            {
                                token.Status.IsIO = true;
                            }
                        }

                        tokens.AddRange(innerTokens);

                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;

                        break;
                    case '[':
                        innerStr = string.Empty;
                        var bracketsCounter = 1;
                        pos++;
                        if (sourceCode[pos] != ']')
                        {
                            while (pos != sourceCode.Length - 1 && bracketsCounter != 0)
                            {
                                innerStr += sourceCode[pos];

                                if (sourceCode[pos + 1] == '[')
                                    ++bracketsCounter;

                                if (sourceCode[pos + 1] == ']')
                                    --bracketsCounter;
                                pos++;
                            }
                            innerTokens = Tokenize(innerStr);

                        }
                        else
                        {
                            innerTokens = new List<Token>();
                        }

                        if (!types.Contains(value))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                type = TokenType.SubscriptOperator;
                                tokens.Add(new Token()
                                {
                                    Type = type,
                                    Value = value + "[]",
                                });
                                var tok = tokens.FirstOrDefault(t => t.Value == value && t.Type == TokenType.Identifier, null);
                                if (tok != null)
                                {
                                    tok.Status.IsModified = true;
                                }
                            }
                            else
                            {
                                type = TokenType.Brackets;
                                tokens.Add(new Token()
                                {
                                    Type = type,
                                    Value = "[]",
                                });
                            }
                            previousToken = type;
                            value = string.Empty;
                            type = TokenType.None;
                        }
                        else
                        {
                            type = TokenType.TypeAnnotation;
                            value += "[]";
                        }

                        tokens.AddRange(innerTokens);

                        break;

                    case '{':
                        if (value != string.Empty)
                        {
                            if (!considerNextType)
                            {
                                if (keywords.Contains(value) && !considerNextIdentifier)
                                {
                                    type = TokenType.Keyword;
                                }

                                tokens.Add(new Token()
                                {
                                    Type = type,
                                    Value = value,
                                    Status = new VariableStatus
                                    {
                                        IsModified = true,
                                    }
                                });
                            }
                            else
                            {
                                considerNextType = false;
                                if (!types.Contains(value))
                                {
                                    types.Add(value);
                                }
                                tokens.Add(new Token
                                {
                                    Type = TokenType.TypeAnnotation,
                                    Value = value,
                                });
                            }
                        }
                        innerTokens = new List<Token>();
                        if (sourceCode[pos + 1] != '}')
                        {
                            innerStr = string.Empty;
                            var bracesCounter = 1;
                            pos++;

                            while (pos != sourceCode.Length - 1 && bracesCounter != 0)
                            {
                                innerStr += sourceCode[pos];

                                if (sourceCode[pos + 1] == '{')
                                    ++bracesCounter;

                                if (sourceCode[pos + 1] == '}')
                                    --bracesCounter;
                                pos++;
                            }
                            innerTokens = Tokenize(innerStr);
                            tokens.Add(new Token
                            {
                                Value = "{",
                                Type = TokenType.OpenBrace,
                            });
                            tokens.AddRange(innerTokens);
                        }
                        else
                        {
                            tokens.Add(new Token
                            {
                                Value = "{",
                                Type = TokenType.OpenBrace,
                            });
                        }

                        ++pos;
                        type = TokenType.Braces;
                        tokens.Add(new Token()
                        {
                            Type = type,
                            Value = "{}",
                        });

                        previousToken = type;
                        type = TokenType.None;
                        value = string.Empty;
                        break;

                    case '=':
                        if (value != string.Empty)
                        {
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                            });
                        }
                        if (pos < sourceCode.Length - 2 && sourceCode[pos + 1] == '=' && sourceCode[pos + 2] == '=')
                        {
                            type = TokenType.Identical;
                            pos += 2;
                            tokens.Add(new Token
                            {
                                Value = "===",
                                Type = type,
                            });
                        }
                        else if (pos < sourceCode.Length - 2 && sourceCode[pos + 1] == '=' && sourceCode[pos + 2] == '~')
                        {
                            type = TokenType.MatchOperator;
                            pos += 2;
                            tokens.Add(new Token
                            {
                                Value = "==~",
                                Type = type,
                            });
                        }
                        else if (pos < sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            type = TokenType.Equal;
                            pos += 1;
                            tokens.Add(new Token
                            {
                                Value = "==",
                                Type = type,
                            });
                        }
                        else if (pos < sourceCode.Length - 1 && sourceCode[pos + 1] == '~')
                        {
                            type = TokenType.FindOperator;
                            pos += 1;
                            tokens.Add(new Token
                            {
                                Value = "=~",
                                Type = type,
                            });
                        }
                        else
                        {
                            type = TokenType.Assignment;
                            tokens.Add(new Token
                            {
                                Value = "=",
                                Type = type,
                            });
                            isLHS = false;
                        }
                        previousToken = type;
                        type = TokenType.None;
                        value = string.Empty;
                        break;
                    case '+':
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.PlusAssignment;
                            tokens.Add(new Token
                            {
                                Value = "+=",
                                Type = type,
                            });
                            isLHS = false;
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '+')
                        {
                            previousToken = type;
                            if (type == TokenType.None || value == string.Empty)
                            {
                                pos++;
                                type = TokenType.PrefixIncrement;
                                tokens.Add(new Token
                                {

                                    Value = "++",
                                    Type = type,
                                });

                            }
                            else
                            {
                                pos++;
                                type = TokenType.PostfixIncrement;
                                tokens.Add(new Token
                                {
                                    Value = "++",
                                    Type = type,
                                });

                            }
                        }
                        else if (IsOperand(type) || IsOperand(previousToken))
                        {
                            type = TokenType.Plus;
                            tokens.Add(new Token
                            {

                                Value = "+",
                                Type = type,
                            });
                        }
                        else
                        {
                            type = TokenType.UnaryPlus;
                            tokens.Add(new Token
                            {
                                Value = "+",
                                Type = type,
                            });
                        }
                        if (value != string.Empty)
                        {
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = previousToken,
                                Status = new VariableStatus
                                {
                                    IsModified = !isLHS,
                                }
                            });
                        }
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '-':
                        if (value != string.Empty)
                        {
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                                Status = new VariableStatus
                                {
                                    IsModified = !isLHS,
                                }
                            });
                            previousToken = type;
                            type = TokenType.None;
                            value = string.Empty;
                        }
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '>')
                        {
                            pos++;
                            type = TokenType.ClosureOperator;
                            tokens.Add(new Token
                            {

                                Value = "->",
                                Type = type,
                            });
                            break;
                        }
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.MinusAssignment;
                            tokens.Add(new Token
                            {

                                Value = "-=",
                                Type = type,
                            });
                            isLHS = false;
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '-')
                        {
                            previousToken = type;
                            if (type == TokenType.None || value == string.Empty)
                            {
                                pos++;
                                type = TokenType.PrefixDecrement;
                                tokens.Add(new Token
                                {

                                    Value = "--",
                                    Type = type,
                                });

                            }
                            else
                            {
                                pos++;
                                tokens.Add(new Token
                                {
                                    Value = value,
                                    Type = type,
                                    Status = new VariableStatus
                                    {
                                        IsModified = !isLHS,
                                    }
                                });
                                type = TokenType.PostfixDecrement;
                                tokens.Add(new Token
                                {
                                    Value = "--",
                                    Type = type,
                                });

                            }
                        }
                        else if (IsOperand(previousToken))
                        {
                            type = TokenType.Minus;
                            tokens.Add(new Token
                            {

                                Value = "-",
                                Type = type,
                            });
                        }
                        else
                        {
                            type = TokenType.UnaryMinus;
                            tokens.Add(new Token
                            {

                                Value = "-",
                                Type = type,
                            });
                        }
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '*':
                        if (value != string.Empty)
                        {
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                                Status = new VariableStatus
                                {
                                    IsModified = !isLHS,
                                }
                            });
                        }
                        else if (pos < sourceCode.Length - 2 && sourceCode[pos + 1] == '*' && sourceCode[pos + 2] == '=')
                        {
                            pos += 2;
                            type = TokenType.DoubleStarAssignment;
                            tokens.Add(new Token
                            {
                                Value = "**=",
                                Type = type,
                            });
                            isLHS = false;
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.StarAssignment;
                            tokens.Add(new Token
                            {
                                Value = "*=",
                                Type = type,
                            });
                            isLHS = false;
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '*')
                        {
                            pos++;
                            type = TokenType.DoubleStar;
                            tokens.Add(new Token
                            {
                                Value = "**",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '.')
                        {
                            pos++;
                            type = TokenType.SpreadOperator;
                            tokens.Add(new Token
                            {
                                Value = "*.",
                                Type = type,
                            });
                            break;
                        }
                        else
                        {
                            type = TokenType.Star;
                            tokens.Add(new Token
                            {
                                Value = "*",
                                Type = type,
                            });
                        }
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '/':
                        if (value != string.Empty)
                        {
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                                Status = new VariableStatus
                                {
                                    IsModified = !isLHS,
                                }
                            });
                        }
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.SlashAssignment;
                            tokens.Add(new Token
                            {
                                Value = "/=",
                                Type = type,
                            });
                            isLHS = false;
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '*')
                        {
                            while (pos != sourceCode.Length - 1 && (sourceCode[pos - 1] != '*' || sourceCode[pos] != '/'))
                            {
                                pos++;
                            }
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '/')
                        {
                            while (pos != sourceCode.Length - 1 && sourceCode[pos] != '\r')
                            {
                                pos++;
                            }
                        }
                        else
                        {
                            type = TokenType.Slash;
                            tokens.Add(new Token
                            {
                                Value = "/",
                                Type = type,
                            });
                        }
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '%':
                        if (value != string.Empty)
                        {
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                                Status = new VariableStatus
                                {
                                    IsModified = !isLHS,
                                }
                            });
                        }
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.PercentAssignment;
                            tokens.Add(new Token
                            {
                                Value = "%=",
                                Type = type,
                            });
                            isLHS = false;
                        }
                        else
                        {
                            type = TokenType.Percent;
                            tokens.Add(new Token
                            {
                                Value = "%",
                                Type = type,
                            });
                        }
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '>':
                        if (pos != sourceCode.Length - 2 && sourceCode[pos + 1] == '>' && sourceCode[pos + 2] == '>')
                        {
                            pos += 2;
                            type = TokenType.UnsignedRightShift;
                            tokens.Add(new Token
                            {
                                Value = ">>>",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.GreaterOrEqual;
                            tokens.Add(new Token
                            {
                                Value = ">=",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '>')
                        {
                            pos++;
                            type = TokenType.RightShift;
                            tokens.Add(new Token
                            {
                                Value = ">>",
                                Type = type,
                            });
                        }
                        else
                        {
                            type = TokenType.GreaterThan;
                            tokens.Add(new Token
                            {
                                Value = ">",
                                Type = type,
                            });

                        }
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '<':
                        if (types.Contains(value) || considerNextType)
                        {
                            value += sourceCode[pos];
                            while (sourceCode[pos] != '>' && pos != sourceCode.Length - 1)
                            {
                                pos++;
                                value += sourceCode[pos];
                            }
                            type = TokenType.TypeAnnotation;
                            break;
                        }
                        else if (pos != sourceCode.Length - 2 && sourceCode[pos + 1] == '=' && sourceCode[pos + 2] == '>')
                        {
                            pos += 2;
                            type = TokenType.SpaceshipOperator;
                            tokens.Add(new Token
                            {
                                Value = "<=>",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.LessOrEqual;
                            tokens.Add(new Token
                            {
                                Value = "<=",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '<')
                        {
                            pos++;
                            type = TokenType.LeftShift;
                            tokens.Add(new Token
                            {
                                Value = "<<",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '>')
                        {
                            pos++;
                            type = TokenType.DiamondOperator;
                            tokens.Add(new Token
                            {
                                Value = "<>",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '.')
                        {
                            pos += 2;
                            value = "<..";
                            if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '<')
                            {
                                pos++;
                                value = "<..<";
                            }
                            type = TokenType.RangeOperator;
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                                Status = new VariableStatus
                                {
                                    IsModified = !isLHS,
                                }
                            });

                        }
                        else
                        {
                            type = TokenType.LessThan;
                            tokens.Add(new Token
                            {
                                Value = "<",
                                Type = type,
                            });
                        }

                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '&':
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '&')
                        {
                            pos++;
                            type = TokenType.And;
                            tokens.Add(new Token
                            {
                                Value = "&&",
                                Type = type,
                            });
                        }
                        else
                        {
                            type = TokenType.BitwiseAnd;
                            tokens.Add(new Token
                            {
                                Value = "&",
                                Type = type,
                            });
                        }
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '|':
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '|')
                        {
                            pos++;
                            type = TokenType.Or;
                            tokens.Add(new Token
                            {
                                Value = "||",
                                Type = type,
                            });
                        }
                        else
                        {
                            type = TokenType.BitwiseOr;
                            tokens.Add(new Token
                            {
                                Value = "|",
                                Type = type,
                            });
                        }
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '^':
                        type = TokenType.BitwiseXor;
                        tokens.Add(new Token
                        {
                            Value = "^",
                            Type = type,
                        });
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case ':':
                        if (type != TokenType.None)
                        {
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                            });

                            previousToken = type;
                            type = TokenType.None;
                            value = string.Empty;
                        }

                        isLHS = false;
                        break;
                    case '?':
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == ':')
                        {
                            pos++;
                            type = TokenType.ElvisOperator;
                            tokens.Add(new Token
                            {
                                Value = "?:",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '.')
                        {
                            type = TokenType.NullSafeMemberAccess;
                            tokens.Add(new Token
                            {
                                Type = type,
                                Value = "?.",
                            });
                            value += ch;
                            value += sourceCode[++pos];
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.ElvisAssignment;
                            tokens.Add(new Token
                            {
                                Value = "?=",
                                Type = type,
                            });

                            isLHS = false;
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '[')
                        {
                            pos += 2;
                            type = TokenType.Identifier;
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                                Status = new VariableStatus
                                {
                                    IsModified = !isLHS,
                                }
                            });

                            innerStr = string.Empty;
                            while (pos != sourceCode.Length - 1 && sourceCode[pos] != ']')
                            {
                                innerStr += sourceCode[pos++];
                            }

                            var tokenList = Tokenize(innerStr);
                            tokens.AddRange(tokenList);

                            type = TokenType.SafeSubscriptOperator;
                            tokens.Add(new Token
                            {
                                Value = "?[]",
                                Type = type,
                            });
                            previousToken = type;
                            value = string.Empty;
                            type = TokenType.None;
                        }
                        else
                        {
                            type = TokenType.TernaryOperator;
                            tokens.Add(new Token
                            {
                                Value = "cond? obj:obj",
                                Type = type,
                            });

                        }
                        break;
                    case '!':
                        if (pos != sourceCode.Length - 2 && sourceCode[pos + 1] == '=' && sourceCode[pos + 2] == '=')
                        {
                            pos += 2;
                            type = TokenType.NotIdentical;
                            tokens.Add(new Token
                            {
                                Type = type,
                                Value = "!==",
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.NotEqual;
                            tokens.Add(new Token
                            {
                                Type = type,
                                Value = "!=",
                            });
                        }
                        else
                        {
                            tokens.Add(new Token
                            {
                                Type = TokenType.Not,
                                Value = "!",
                            });
                        }
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '~':
                        if (pos != sourceCode.Length - 1 && (sourceCode[pos + 1] == '"' || sourceCode[pos + 1] == '$' || sourceCode[pos + 1] == 39))
                        {

                            type = TokenType.PatternOperator;
                            tokens.Add(new Token
                            {
                                Type = type,
                                Value = "~str",
                            });
                        }
                        else
                        {
                            type = TokenType.BitwiseNot;
                            tokens.Add(new Token
                            {
                                Type = type,
                                Value = "~",
                            });
                        }
                        previousToken = type;
                        type = TokenType.None;
                        value = string.Empty;
                        break;
                    case '.':
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '.')
                        {
                            if (value != string.Empty)
                            {
                                tokens.Add(new Token
                                {
                                    Value = value,
                                    Type = type,
                                    Status = new VariableStatus
                                    {
                                        IsModified = !isLHS,
                                    }
                                });
                            }
                            pos++;
                            type = TokenType.RangeOperator;
                            value = "..";
                            if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '<')
                            {
                                value = "..<";
                                pos++;
                            }
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                            });

                            previousToken = type;
                            type = TokenType.None;
                            value = string.Empty;
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '?')
                        {
                            type = TokenType.NullSafeMemberAccess;
                            tokens.Add(new Token
                            {
                                Type = type,
                                Value = ".?",
                            });
                            value += ch;
                            value += sourceCode[++pos];
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '&')
                        {
                            pos++;
                            type = TokenType.MethodPointer;
                            tokens.Add(new Token
                            {
                                Type = type,
                                Value = ".&",
                            });
                            tokens.Add(new Token
                            {
                                Type = TokenType.Identifier,
                                Value = value,
                                Status = new VariableStatus
                                {
                                    IsModified = !isLHS,
                                }
                            });

                            previousToken = type;
                            type = TokenType.None;
                            value = string.Empty;
                        }
                        else
                        {
                            if (type != TokenType.NumberLiteral && !types.Contains(value))
                            {
                                tokens.Add(new Token
                                {
                                    Type = TokenType.MemberAccess,
                                    Value = value + ".member",
                                });
                                var t = tokens.FirstOrDefault(t => t.Value == value && t.Type == TokenType.Identifier, null);
                                if (t != null)
                                {
                                    t.Status.IsModified = true;
                                }
                                isMember = true;
                            }
                            value += ch;
                        }
                        break;
                    case '\'':
                    case '"':
                        type = TokenType.StringLiteral;
                        if (pos <= sourceCode.Length - 3 && sourceCode[pos + 1] == ch && sourceCode[pos + 2] == ch)
                        {
                            pos += 2;
                            isTripleQuotes = true;
                        }
                        if (isMember)
                        {
                            type = TokenType.Identifier;
                        }
                        value += sourceCode[pos];
                        pos++;
                        while (pos != sourceCode.Length - 1 && sourceCode[pos] != ch)
                        {
                            value += sourceCode[pos];
                            if (sourceCode[pos] == '$')
                            {
                                innerTokens = new List<Token>();
                                var interpolationPos = pos + 1;
                                var delim = ' ';
                                if (pos != sourceCode.Length - 2 && sourceCode[pos + 1] == '{')
                                {
                                    type = TokenType.Braces;
                                    tokens.Add(new Token
                                    {
                                        Type = type,
                                        Value = "{}",
                                    });
                                    value += '{';
                                    delim = '}';
                                    interpolationPos++;
                                }

                                var interpolationValue = string.Empty;
                                while (interpolationPos != sourceCode.Length - 1 && sourceCode[interpolationPos] != delim && sourceCode[interpolationPos] != ch)
                                {
                                    interpolationValue += sourceCode[interpolationPos];
                                    interpolationPos++;
                                }
                                innerTokens = Tokenize(interpolationValue);
                                tokens.AddRange(innerTokens);
                                pos = interpolationPos - 1;
                                value += interpolationValue;
                            }
                            pos++;
                        }
                        value += sourceCode[pos];

                        if (considerNextIdentifier)
                        {
                            type = TokenType.Identifier;
                            if (sourceCode[pos + 1] != '(')
                            {
                                considerNextIdentifier = false;
                            }
                        }
                        else
                        {
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
                            });
                            previousToken = type;
                            type = TokenType.None;
                            value = string.Empty;
                        }
                        if (isTripleQuotes)
                        {
                            isTripleQuotes = false;
                            pos += 2;
                        }
                        break;
                    case ';':
                    case '\t':
                    case '\n':
                    case '\r':
                    case ' ':
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            if (sourceCode[pos] == '\n' || sourceCode[pos] == '\r')
                            {
                                isLHS = true;
                            }
                            break;
                        }
                        //check for keyword
                        if (keywords.Contains(value) && !considerNextIdentifier)
                        {
                            type = TokenType.Keyword;
                        }

                        if (specialOperators.Contains(value))
                        {
                            type = value == "as" ? TokenType.CoercionOperation : TokenType.MembershipOperator;
                        }

                        if (considerNextIdentifier)
                        {
                            type = TokenType.Identifier;
                            considerNextIdentifier = false;
                        }

                        if (considerNextType)
                        {
                            considerNextType = false;
                            types.Add(value);
                        }

                        if (types.Contains(value))
                        {
                            type = TokenType.TypeAnnotation;
                        }

                        if (type == TokenType.Keyword)
                        {
                            if (value == "def")
                            {
                                considerNextIdentifier = true;
                            }
                            else if (IsOperatorParentheses(value))
                            {
                                isIfFor = true;
                            }
                            else if (value == "class")
                            {
                                considerNextType = true;
                            }
                            else if (value == "do")
                            {
                                previousToken = type;
                                type = TokenType.None;
                                value = string.Empty;
                                continue;
                            }
                        }

                        tokens.Add(new Token
                        {
                            Value = value,
                            Type = type,
                            Status = new VariableStatus
                            {
                                IsModified = !isLHS,
                            }
                        });
                        previousToken = type;
                        type = TokenType.None;
                        value = string.Empty;

                        if ((ch == ';' || ch == '\r') && previousToken != TokenType.Braces)
                        {
                            if (ch == ';' && pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '\r')
                            {
                                pos += 1;
                            }
                            tokens.Add(new Token
                            {
                                Type = TokenType.Delimiter,
                                Value = (ch == ';') ? ch.ToString() : "new line",
                            });
                        }

                        if (ch == ';' || ch == '\r' || ch == '\n')
                        {
                            isLHS = true;
                        }
                        break;

                    default:

                        if (previousToken == TokenType.MethodPointer)
                            break;

                        type = TokenType.Identifier;
                        value += ch;
                        break;
                }
            }
            if (value != string.Empty)
            {
                tokens.Add(new Token
                {
                    Value = value,
                    Type = type,
                });
            }
            return tokens;
        }
        public static readonly List<string> keywords = new List<string>(){
            "abstract", "assert", "break", "case", "catch",
            "class", "const", "continue", "def", "default", "do", "else",
            "enum", "extends", "final", "finally", "for", "goto", "if",
            "implements", "import", "instanceof", "interface", "native", "new",
            "non-sealed", "package", "public", "protected", "private",
            "return", "static", "strictfp", "super", "switch", "synchronized",
            "this", "threadsafe", "throw", "throws", "transient", "try", "while",
            "print", "println", "true", "false"
        };

        public static readonly List<string> operandKeywords = new List<string>(){
            "null", "true", "false"
        };

        public static readonly List<string> types = new List<string>(){
            "byte", "short", "int", "long","float", "double","char",
            "boolean","Byte","Short","Integer","Long","Float","Double",
            "Character","Boolean","String","GString","Array","List",
            "Map", "Range","BigDecimal", "BigInteger","Date", "TimeZone",
            "void"
        };


        public static readonly List<string> specialOperators = new List<string>()
        { "in", "as", };

        public static readonly List<TokenType> operandTypes = new List<TokenType>(){
            TokenType.NumberLiteral, TokenType.StringLiteral, TokenType.Identifier,
            TokenType.FunctionCall, TokenType.Parentheses, TokenType.Braces,
            TokenType.PostfixDecrement, TokenType.PostfixIncrement, TokenType.SubscriptOperator,
            TokenType.SafeSubscriptOperator
        };

        public static bool IsOperand(TokenType type)
        {
            return operandTypes.Contains(type);
        }

        public static bool IsOperatorParentheses(string value)
        {

            if (value == "switch" || value == "if" || value == "for" || value == "catch" || value == "while")
            {
                return true;
            }
            return false;
        }
    }
}
