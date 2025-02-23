using GroovyParserBackend;

namespace ParserInterface
{
    public partial class MainPage : ContentPage
    {
        private const string _tableOperandHead = "Operand\n";
        private const string _tableOperatorHead = "Operator\n";
        private const string _counterHead = "Counter";

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
            OperandCounterTable.Text = _counterHead;
            OperatorCounterTable.Text = _counterHead;
            

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
            foreach (var _operand in halsteadMetrics.operandDict)
            {
                OperandTable.Text += $"{_operand.Key}\n";
                if(_operand.Key.ToString().Length >= 15)
                {
                    OperandCounterTable.Text += '\n';
                }
                OperandCounterTable.Text += $"{_operand.Value}\n";
            }

            foreach (var _operator in halsteadMetrics.operatorDict)
            {
                OperatorTable.Text += $"{_operator.Key}\n";
                if (_operator.Key.ToString().Length >= 15)
                {
                    OperatorCounterTable.Text += '\n';
                }
                OperatorCounterTable.Text += $"{_operator.Value}\n";
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
