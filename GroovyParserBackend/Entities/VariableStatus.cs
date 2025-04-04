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
    }
}
