namespace GroovyParserBackend.Entities
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Token Type: {this.Type}\nToken Value: {this.Value}";
        }
    }
}
