using GroovyParserBackend;

namespace ParserInterface
{
    using Token = GroovyParserBackend.Entities.Token;
    public partial class MainPage : ContentPage
    {

        private const int identColumnWidth = 30;
        private const int countColumnWidth = 8;
        

        public MainPage()
        {
            InitializeComponent();
            SpansTable.FontFamily = "Courier New";
            InputOutputTable.FontFamily = "Courier New";
            ChepinTable.FontFamily = "Courier New";
        }

        private async void OnOpenFileClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a file",
              
            });

            if (result != null)
            {
                if (!result.FileName.EndsWith(".groovy")) {
                    await DisplayAlert("Error!", "The file must have a .groovy extension!", "OK");
                    return;
                }

                var text = File.ReadAllText(result.FullPath);
                FileEditor.Text = text;

            }
        }

        private async void OnAnalyseClicked(object sender, EventArgs e)
        {
            SpansTable.Text = $"{"Identifier".PadRight(identColumnWidth)}"+
                              $"{"Counter".PadRight(countColumnWidth)}\n";
            ChepinTable.Text = $"{"Type".PadRight(identColumnWidth)}" +
                               $"{"Counter".PadRight(countColumnWidth)}\n" +
                               new string('-', identColumnWidth + countColumnWidth) + "\n";
            InputOutputTable.Text = ChepinTable.Text;

            var sourceCode = FileEditor.Text;
            if (sourceCode == null)
            {
                await DisplayAlert("Error!", "Please, write some code or choose one file", "OK");
                return;
            }
            var tokens = Tokenizer.Tokenize(sourceCode);
            var tokenDict = Parser.GetSpans(tokens);
            SpansTable.Text += new string('-', identColumnWidth + countColumnWidth) + "\n";

            foreach (var ident in tokenDict)
            {
                string name = ident.Key.ToString().PadRight(identColumnWidth);
                string counter = ident.Value.ToString().PadRight(countColumnWidth);

                SpansTable.Text += $"{name}{counter}\n";
            }

            TotalSpenCounter.Text = tokenDict.Values.Sum().ToString();

            // Chepin's metrics
            var hal = Parser.GetBasicMetrics(tokens);
            var normalIdent = Parser.GetIdentificators(hal.operandDict);
            var IOIdent = Parser.GetIdentificators(hal.operandDict, true);

            var keys = normalIdent.Keys.ToList();
            var ioKeys = IOIdent.Keys.ToList();

            var chepinText = "";
            var ioText = "";

            var totalIdent = new List<Token>();

            foreach (var key in keys)
            {

                chepinText += '\n' + key.First().Status.ToString().ToUpper();

                string valuesLine = string.Join(" ", key.Select(k => k.Value));

                chepinText += '\t' + normalIdent[key].ToString() + '\n' + valuesLine + "\n";
                totalIdent.AddRange(key);
            }

            ChepinTable.Text += chepinText;
            TotalChepinMetric.Text = Parser.GetChepinMetric(totalIdent).ToString();
            totalIdent.Clear();

            foreach (var key in ioKeys)
            {

                ioText += '\n' + key.First().Status.ToString().ToUpper();

                string valuesLine = string.Join(" ", key.Select(k => k.Value));

                ioText += '\t' + IOIdent[key].ToString() + '\n' + valuesLine + "\n";
                totalIdent.AddRange(key);
            }

            InputOutputTable.Text += ioText;
            TotalIOMetric.Text = Parser.GetChepinMetric(totalIdent).ToString();
        }
    }
}
