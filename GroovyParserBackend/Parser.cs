using GroovyParserBackend.Entities;

namespace GroovyParserBackend
{
    using TokenDict = Dictionary<Token, int>;
    public class Parser
    {
        public static List<Token> GetNormalisedIfs(List<Token> tokens)
        {
            Stack<Token> ifStack = new Stack<Token>();
            var result = new List<Token>();
            for (int i = 0; i < tokens.Count; ++i)
            {
                var token = tokens[i];
                if (token.Type == TokenType.Keyword && token.Value == "if")
                {
                    ifStack.Push(token);
                }
                else if (token.Type == TokenType.Braces)
                {
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
            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Identifier:
                    case TokenType.NumberLiteral:
                    case TokenType.StringLiteral:
                        if (!operandOperatorDicts.Item1.TryAdd(token, 1))
                        {
                            operandOperatorDicts.Item1[token] += 1;
                        }
                        break;
                    default:
                        if (!operandOperatorDicts.Item2.TryAdd(token, 1))
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
