using GroovyParserBackend;

namespace ParserInterface
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

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

        private async void OnAnalizeClicked(object sender, EventArgs e)
        {
            ResultTable.Text = "";
            // Обновить 3 метки и вывести результат в скроллбар
            var sourceCode = FileEditor.Text;
            if (sourceCode == null)
            {
                await DisplayAlert("Error!", "Please, write some code or choose one file", "OK");
                return;
            }
            foreach (var token in Tokenizer.Tokenize(sourceCode))
            {
                ResultTable.Text += token.ToString();
            }
        }
    }

}
