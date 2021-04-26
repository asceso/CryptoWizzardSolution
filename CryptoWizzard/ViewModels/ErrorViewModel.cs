using CryptoWizzard.Views;
using Prism.Commands;
using Prism.Mvvm;

namespace CryptoWizzard.ViewModels
{
    public class ErrorViewModel : BindableBase
    {
        private readonly ErrorWindow error_window;

        public DelegateCommand CloseCommand { get; private set; }

        private string _title;
        private string _message;
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        public ErrorViewModel(string message, ErrorWindow error_window)
        {
            Title = "Ошибка";
            Message = message;
            this.error_window = error_window;
            CloseCommand = new DelegateCommand(OnClose);
        }
        private void OnClose() => error_window.Close();
    }
}
