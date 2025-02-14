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
            TokenType previousToken = TokenType.None;
            // maybe need to implement some rvalue/lvalue distinctions to distinguish between
            // obj.method() operator
            // a = obj.method() operand?
            // a = obj.method definetly operand
            // a = obj."I HATE THIS LANG" valid groovy code btw
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
                    case '[':

                        if (value != string.Empty)
                        {
                            tokens.Add(new Token()
                            {
                                Type = type,
                                Value = value,
                            });
                        }

                        var innerStr = string.Empty;
                        while (pos != sourceCode.Length - 1 && sourceCode[pos + 1] != ']')
                        {
                            innerStr += sourceCode[++pos];
                        }
                        ++pos;
                        var innerTokens = Tokenize(innerStr);
                        tokens.AddRange(innerTokens);

                        if (innerTokens.Count == 1 && innerTokens[0].Type == TokenType.NumberLiteral || innerTokens[0].Type != TokenType.Identifier)
                        {
                            type = TokenType.SubscriptOperator;
                            tokens.Add(new Token()
                            {
                                Type = type,
                                Value = "arr[]",
                            });
                        }
                        else
                        {
                            type = TokenType.Braces;
                            tokens.Add(new Token()
                            {
                                Type = type,
                                Value = "[]",
                            });
                        }

                        previousToken = type;
                        type = TokenType.None;
                        value = string.Empty;
                        break;

                    case '{':
                        if (value != string.Empty)
                        {
                            tokens.Add(new Token()
                            {
                                Type = type,
                                Value = value,
                            });
                        }

                        innerStr = string.Empty;
                        var bracketsCounter = 1;
                        while (pos != sourceCode.Length - 1 && bracketsCounter != 0)
                        {
                            innerStr += sourceCode[++pos];

                            if (sourceCode[pos + 1] == '{')
                                ++bracketsCounter;

                            if (sourceCode[pos + 1] == '}')
                                --bracketsCounter;
                        }
                        ++pos;
                        innerTokens = Tokenize(innerStr);
                        tokens.AddRange(innerTokens);

                        type = TokenType.Brackets;
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
                        else
                        {
                            type = TokenType.Assignment;
                            tokens.Add(new Token
                            {
                                Value = "=",
                                Type = type,
                            });
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
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '+')
                        {
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
                                tokens.Add(new Token
                                {
                                    Value = value,
                                    Type = type,
                                });
                                type = TokenType.PostfixIncrement;
                                tokens.Add(new Token
                                {
                                    Value = "++",
                                    Type = type,
                                });

                            }
                        }
                        else if (IsOperand(previousToken))
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
                        previousToken = type;
                        value = string.Empty;
                        type = TokenType.None;
                        break;
                    case '-':
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.MinusAssignment;
                            tokens.Add(new Token
                            {

                                Value = "-=",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '-')
                        {
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
                        if (pos != sourceCode.Length - 2 && sourceCode[pos + 1] == '=' && sourceCode[pos + 2] == '>')
                        {
                            pos += 2;
                            type = TokenType.SpaceshipOperator;
                            tokens.Add(new Token
                            {
                                Value = "<=>",
                                Type = type,
                            });
                        }
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
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
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.ElvisAssignment;
                            tokens.Add(new Token
                            {
                                Value = "?=",
                                Type = type,
                            });
                        }
                        else if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '[')
                        {
                            pos += 2;
                            type = TokenType.Identifier;
                            tokens.Add(new Token
                            {
                                Value = value,
                                Type = type,
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
                    case '.':
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '.')
                        {
                            pos++;
                            type = TokenType.RangeOperator;
                            tokens.Add(new Token
                            {
                                Value = "..",
                                Type = type,
                            });
                            previousToken = type;
                            type = TokenType.None;
                            value = string.Empty;
                        }
                        else
                        {
                            if (type != TokenType.NumberLiteral)
                            {
                                isMember = true;
                            }
                            value += ch;
                        }
                        break;
                    case '\'':
                    case '"':
                        if (isMember)
                        {
                            while (pos != sourceCode.Length - 1 && sourceCode[pos] != ch)
                            {
                                value += sourceCode[pos];
                            }
                            value += sourceCode[pos];
                            type = TokenType.Identifier;
                        }
                        else
                        {
                            pos++;
                            while (pos != sourceCode.Length - 1 && sourceCode[pos] != ch)
                            {
                                value += sourceCode[pos];
                            }
                            type = TokenType.StringLiteral;
                        }
                        break;
                    //ignore single-line and multiline comments
                    case '/':
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '*')
                        {
                            while ((ch != '*' || sourceCode[pos + 1] != '/') && pos != sourceCode.Length - 2)
                            {
                                pos++;
                            }
                        }
                        else
                        {
                            while (ch != '\n' && pos != sourceCode.Length - 1)
                            {
                                pos++;
                            }
                        }
                        break;

                    case ';':
                    case '\t':
                    case '\n':
                    case '\r':
                    case ' ':
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            break;
                        }
                        //check for keyword
                        if (keywords.Contains(value) && !considerNextIdentifier)
                        {
                            type = TokenType.Keyword;
                        }

                        if (considerNextIdentifier)
                        {
                            type = TokenType.Identifier;
                            considerNextIdentifier = false;
                        }

                        tokens.Add(new Token
                        {
                            Value = value,
                            Type = type,
                        });
                        previousToken = type;
                        type = TokenType.None;

                        if (type == TokenType.Keyword && value == "def")
                        {
                            considerNextIdentifier = true;
                        }
                        if (ch == '\n' || ch == '\r' || ch == ';')
                        {
                            previousToken = TokenType.None;
                        }
                        value = string.Empty;
                        break;

                    default:
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
            "null", "non-sealed", "package", "public", "protected", "private",
            "return", "static", "strictfp", "super", "switch", "synchronized",
            "this", "threadsafe", "throw", "throws", "transient", "try", "while",
        };

        public static readonly List<TokenType> operandTypes = new List<TokenType>(){
            TokenType.NumberLiteral, TokenType.StringLiteral, TokenType.Identifier,
            TokenType.FunctionCall, TokenType.Parentheses, TokenType.Braces,
            TokenType.PostfixDecrement, TokenType.PostfixIncrement, TokenType.SubscriptOperator,
            TokenType.SafeSubscriptOperator
        };

        private static bool IsOperand(TokenType type)
        {
            return operandTypes.Contains(type);
        }
    }
}
