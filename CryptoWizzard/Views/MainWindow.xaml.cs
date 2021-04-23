using System.Windows;

namespace CryptoWizzard.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Width = SystemParameters.FullPrimaryScreenWidth - 10;
            Height = SystemParameters.FullPrimaryScreenHeight + 10;
            InitializeComponent();
        }
    }
}
