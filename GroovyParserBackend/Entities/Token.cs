namespace GroovyParserBackend.Entities
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public List<Token> elseIfSequence = new List<Token>();

        public override string ToString()
        {
            //return $"Token Type: {this.Type}\nToken Value: {this.Value}";
            return this.Value;
        }

        public string Display()
        {
            return $"Token Type: {this.Type}\nToken Value: {this.Value}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Token other)
            {
                return Type == other.Type && Value == other.Value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }
    }
}

