using CryptoWizzard.ViewModels;
using Services.HashingService;
using Services.MemoryService;
using System.Windows;

namespace CryptoWizzard.Views
{
    /// <summary>
    /// Логика взаимодействия для KeysWindow.xaml
    /// </summary>
    public partial class KeysWindow : Window
    {
        public KeysWindow(IMemoryService memory, IHashingService hashing)
        {
            InitializeComponent();
            DataContext = new KeysViewModel(memory, hashing, this);
        }
    }
}
