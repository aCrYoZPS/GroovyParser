namespace GroovyParserBackend.Entities
{
    // Parasite
    // if not parasite than modified
    // if modified possibly io
    // if control then control

    public class VariableStatus
    {
        public bool IsParasite { get; set; } = true;
        public bool IsIO { get; set; } = false;
        public bool IsControl { get; set; } = false;
        public bool IsModified { get; set; } = false;

        public override string ToString()
        {
            var t = "Parasite";
            if (IsControl) 
            {
                t = "Control";
            }
            else if (IsModified)
            {
                t = "Modified";
            }
            else if (IsIO)
            {
                t = "Input/Output";
            }
            return t;
        }
    }

}
