using CryptoWizzard.ViewModels;
using Services.HashingService;
using Services.MemoryService;
using System;
using System.ComponentModel;
using System.IO;
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
            Loaded += MainWindowLoaded;
            Closed += MainWindowClosed;
        }
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo temp = new(Environment.CurrentDirectory + "\\temp");
            temp.Create();
        }
        private void MainWindowClosed(object sender, EventArgs e)
        {
            DirectoryInfo temp = new(Environment.CurrentDirectory + "\\temp");
            temp.Delete(true);
        }
    }
}
