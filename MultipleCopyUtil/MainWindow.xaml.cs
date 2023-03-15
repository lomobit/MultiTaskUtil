namespace MultipleCopyUtil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            lblCursorPosition.Text = $"Version: {appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
        }
    }
}