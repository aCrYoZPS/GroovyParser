using GroovyParserBackend;

namespace ParserInterface
{
    public partial class MainPage : ContentPage
    {

        private const int identColumnWidth = 30;
        private const int countColumnWidth = 8;
        

        public MainPage()
        {
            InitializeComponent();
            SpansTable.FontFamily = "Courier New";
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
        }
    }
}
