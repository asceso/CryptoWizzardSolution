using CryptoWizzard.ViewModels;
using Services.HashingService;
using Services.MemoryService;
using System.Windows;

namespace CryptoWizzard.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IMemoryService memory, IHashingService hashing)
        {
            InitializeComponent();
            Width = SystemParameters.FullPrimaryScreenWidth - 10;
            Height = SystemParameters.FullPrimaryScreenHeight + 10;
            DataContext = new MainWindowViewModel(memory, hashing);
        }
    }
}
