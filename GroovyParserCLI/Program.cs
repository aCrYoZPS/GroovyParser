using GroovyParserCLI;
using GroovyParserBackend;
using System.Text;
using GroovyParserBackend.Entities;

var sourceCode = File.ReadAllText(Path.Join(Globals.RESOURCES_PATH, "main.groovy"));
var metr = Parser.GetBasicMetrics(Parser.GetNormalisedIfs(Tokenizer.Tokenize(sourceCode)));

using (var file = File.Open(Globals.OUTPUT_PATH, FileMode.Create))
{
    file.Write(Encoding.UTF8.GetBytes("Operators:\n"));
    foreach (var i in metr.operatorDict)
    {
        file.Write(Encoding.UTF8.GetBytes($"{i.Key.Type}({i.Key.Value}): {i.Value}\n"));
        Console.WriteLine($"{i.Key.Type}({i.Key.Value}): {i.Value}");
    }

    file.Write(Encoding.UTF8.GetBytes("\nOperands:\n"));
    foreach (var i in metr.operandDict)
    {
        file.Write(Encoding.UTF8.GetBytes($"{i.Key.Type}({i.Key.Value}): {i.Value}\n"));
        if (i.Key.Type == TokenType.Identifier)
        {
            Console.WriteLine(i.Key.Display());
        }
    }
}

