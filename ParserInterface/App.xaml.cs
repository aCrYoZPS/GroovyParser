namespace ParserInterface
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Current?.Resources["PrimaryDark"] as Color,
                BarTextColor = Colors.White
            };
        }

    }
}