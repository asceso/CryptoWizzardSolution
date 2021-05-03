using CryptoWizzard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoWizzard.Views
{
    /// <summary>
    /// Логика взаимодействия для HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow(List<HelpItem> helpItems)
        {
            InitializeComponent();

            ResourceDictionary tb_dic = Application.Current.Resources.MergedDictionaries.FirstOrDefault(md => md.Source.AbsoluteUri.Contains("TextBlock"));

            foreach (HelpItem help in helpItems)
            {
                TextBlock head = new();
                head.Style = (Style)tb_dic["SimpleHeaderTextBlock"];
                head.Margin = new Thickness(0, 10, 0, 0);
                head.Text = help.ChapterHeader;

                TextBlock body = new();
                body.Style = (Style)tb_dic["SimpleTextBlock"];
                head.Margin = new Thickness(5);
                body.Text = help.ChapterBody;
                body.TextWrapping = TextWrapping.Wrap;

                HelpStack.Children.Add(head);
                HelpStack.Children.Add(body);
            }
        }
        private void MoveBorderMouseDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void CloseClick(object sender, RoutedEventArgs e) => Close();

    }
}
