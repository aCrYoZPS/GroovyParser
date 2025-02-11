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
                    case '=':
                        if (pos < sourceCode.Length - 2 && sourceCode[pos + 1] == '=' && sourceCode[pos + 2] == '=')
                        {
                            type = TokenType.Identical;
                            pos += 2;
                            tokens.Add(new Token
                            {
                                //maybe no hardcode?
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
                                //maybe no hardcode?
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
                        //logically it cannot be the last symbol in a file but idk should we fall on ill-formed files
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '=')
                        {
                            pos++;
                            type = TokenType.PlusAssignment;
                            tokens.Add(new Token
                            {
                                //maybe no hardcode?
                                Value = "+=",
                                Type = type,
                            });
                        }
                        else if (previousToken == TokenType.StringLiteral || previousToken == TokenType.NumberLiteral || previousToken == TokenType.Identifier || previousToken == TokenType.FunctionCall)
                        {
                            type = TokenType.Plus;
                            tokens.Add(new Token
                            {
                                //maybe no hardcode?
                                Value = "+",
                                Type = type,
                            });
                        }
                        else
                        {
                            type = TokenType.UnaryPlus;
                            tokens.Add(new Token
                            {
                                //maybe no hardcode?
                                Value = "+",
                                Type = type,
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
                                //maybe no hardcode?
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
                        value = string.Empty;
                        break;

                    default:
                        type = TokenType.Identifier;
                        value += ch;
                        break;
                }
            }
            return tokens;
        }
        public static List<string> keywords = new List<string>(){
            "abstract", "assert", "break", "case", "catch",
            "class", "const", "continue", "def", "default", "do", "else",
            "enum", "extends", "final", "finally", "for", "goto", "if",
            "implements", "import", "instanceof", "interface", "native", "new",
            "null", "non-sealed", "package", "public", "protected", "private",
            "return", "static", "strictfp", "super", "switch", "synchronized",
            "this", "threadsafe", "throw", "throws", "transient", "try", "while",
        };
    }
}
