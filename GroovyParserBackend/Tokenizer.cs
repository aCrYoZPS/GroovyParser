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
            TokenType type = TokenType.Identifier;
            for (int pos = 0; pos < sourceCode.Length; ++pos)
            {
                var ch = sourceCode[pos];
                switch (ch)
                {
                    //ignore single-line comments
                    //ignore multiline comments
                    case '/':
                        if (pos != sourceCode.Length - 1 && sourceCode[pos + 1] == '*')
                        {
                            while (ch != '*' && sourceCode[pos + 1] != '/' && pos != sourceCode.Length - 2)
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
                    case ' ':
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            break;
                        }
                        //check for keyword
                        if (keywords.Contains(value) && !considerNextIdentifier)
                        {
                            type = TokenType.Keyword;
                            // we can go 
                            // def "null" {1}
                            // this.null() // returns 1
                            if (value == "def")
                            {
                                considerNextIdentifier = true;
                            }
                        }

                        if (considerNextIdentifier)
                        {
                            type = TokenType.Identifier;
                            considerNextIdentifier = false;
                        }

                        var token = new Token
                        {
                            Type = type,
                            Value = value,
                        };
                        tokens.Add(token);
                        break;

                    default:
                        value += ch;
                        type = TokenType.Identifier;
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
