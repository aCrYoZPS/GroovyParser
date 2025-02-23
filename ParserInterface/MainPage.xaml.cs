using GroovyParserBackend;

namespace ParserInterface
{
    public partial class MainPage : ContentPage
    {
        private const string _tableOperandHead = "     Operand       |     Count      ";
        private const string _tableOperatorHead = "     Operator       |     Count      ";

        public MainPage()
        {
            InitializeComponent();
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
            OperandTable.Text = _tableOperandHead;
            OperatorTable.Text = _tableOperatorHead;
            

            var sourceCode = FileEditor.Text;
            if (sourceCode == null)
            {
                await DisplayAlert("Error!", "Please, write some code or choose one file", "OK");
                return;
            }
            var tokens = Tokenizer.Tokenize(sourceCode);
            HalsteadMetrics halsteadMetrics = Parser.GetBasicMetrics(tokens);
            DerivedMetrics derivedMetrics = Parser.GetDerivedMetrics(halsteadMetrics);

            // Processing basic metrics
            foreach (var _operand in halsteadMetrics.operandDict)
            {
                OperandTable.Text += $"{_operand.Key}{((_operand.Key.ToString().Length > 7) ? null : '\t')}\t\t{_operand.Value}\n";
            }

            foreach (var _operator in halsteadMetrics.operatorDict)
            {
                OperatorTable.Text += $"{_operator.Key}{((_operator.Key.ToString().Length >= 6) ? null : '\t')}\t\t{_operator.Value}\n";
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
