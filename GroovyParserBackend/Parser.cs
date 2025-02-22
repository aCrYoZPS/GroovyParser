using GroovyParserBackend.Entities;

namespace GroovyParserBackend
{
    using TokenDict = Dictionary<Token, int>;
    public class Parser
    {
        public static HalsteadMetrics GetMetrics(List<Token> tokens)
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
    }
}
