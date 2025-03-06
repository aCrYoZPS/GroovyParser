using GroovyParserBackend;

namespace ParserInterface
{
    public partial class MainPage : ContentPage
    {

        private const int keyColumnWidth = 30;
        private const int valueColumnWidth = 8;
        

        public MainPage()
        {
            InitializeComponent();
            OperatorTable.FontFamily = "Courier New";
            OperandTable.FontFamily = "Courier New";
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
            OperandTable.Text = $"{"Operand".PadRight(keyColumnWidth)}" +
                                $"{"Counter".PadRight(valueColumnWidth)}\n";
            OperatorTable.Text = $"{"Operator".PadRight(keyColumnWidth)}" +
                                 $"{"Counter".PadRight(valueColumnWidth)}\n";

            var sourceCode = FileEditor.Text;
            if (sourceCode == null)
            {
                await DisplayAlert("Error!", "Please, write some code or choose one file", "OK");
                return;
            }
            var tokens = Tokenizer.Tokenize(sourceCode);
            HalsteadMetrics halsteadMetrics = Parser.GetBasicMetrics(Parser.GetNormalisedIfs(tokens));
            DerivedMetrics derivedMetrics = Parser.GetDerivedMetrics(halsteadMetrics);

            // Processing basic metrics
            OperandTable.Text += new string('-', keyColumnWidth + valueColumnWidth) + "\n";
            OperatorTable.Text += new string('-', keyColumnWidth + valueColumnWidth) + "\n";

            foreach (var _operand in halsteadMetrics.operandDict)
            {
                string operand = _operand.Key.ToString().PadRight(keyColumnWidth);
                string counter = _operand.Value.ToString().PadRight(valueColumnWidth);

                OperandTable.Text += $"{operand}{counter}\n";
            }

            foreach (var _operator in halsteadMetrics.operatorDict)
            {
                string oper = _operator.Key.ToString().PadRight(keyColumnWidth);
                string counter = _operator.Value.ToString().PadRight(valueColumnWidth);

                OperatorTable.Text += $"{oper}{counter}\n";
            }

            UniqueOperandCounter.Text = halsteadMetrics.uniqueOperandCount.ToString();
            UniqueOperatorCounter.Text = halsteadMetrics.uniqueOperatorCount.ToString();
            TotalOperandCounter.Text = halsteadMetrics.operandCount.ToString();
            TotalOperatorCounter.Text = halsteadMetrics.operatorCount.ToString();

            //Processing derived metrics
            DictionaryLabel.Text = derivedMetrics.dictionary.ToString();
            LengthLabel.Text = derivedMetrics.length.ToString();
            VolumeLabel.Text = derivedMetrics.volume.ToString();
        }
    }
}
