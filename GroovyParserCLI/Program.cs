using GroovyParserCLI;
using GroovyParserBackend;

var sourceCode = File.ReadAllText(Path.Join(Globals.RESOURCES_PATH, "main.groovy"));
var metr = Parser.GetBasicMetrics(Parser.GetNormalisedIfs(Tokenizer.Tokenize(sourceCode)));

foreach (var i in metr.operatorDict)
{
    System.Console.WriteLine($"{i.Key.Type}({i.Key.Value}): {i.Value}");
}
