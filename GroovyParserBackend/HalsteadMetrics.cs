using GroovyParserBackend.Entities;

namespace GroovyParserBackend
{
    using TokenDict = Dictionary<Token, int>;
    public class HalsteadMetrics
    {
        public int operatorCount;
        public int operandCount;
        public int uniqueOperatorCount;
        public int uniqueOperandCount;
        public TokenDict operatorDict;
        public TokenDict operandDict;
    }
}
