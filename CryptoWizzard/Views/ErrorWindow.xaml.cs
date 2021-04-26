using CryptoWizzard.ViewModels;
using System.Windows;

namespace CryptoWizzard.Views
{
    /// <summary>
    /// Логика взаимодействия для ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string message)
        {
            InitializeComponent();
            DataContext = new ErrorViewModel(message, this);
        }
    }
}
