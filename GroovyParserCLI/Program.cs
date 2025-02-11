using GroovyParserCLI;
using GroovyParserBackend;

var sourceCode = File.ReadAllText(Path.Join(Globals.RESOURCES_PATH, "main.groovy"));

foreach (var token in Tokenizer.Tokenize(sourceCode))
{
    Console.WriteLine(token);
}

