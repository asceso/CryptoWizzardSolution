using CryptoWizzard.Views;
using Prism.Ioc;
using Services.HashingService;
using Services.MemoryService;
using System.Windows;
using System.Windows.Threading;
using Unity;

namespace CryptoWizzard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            DispatcherUnhandledException += ExceptionHandler;
            return Container.Resolve<MainWindow>();
        }
        private void ExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorWindow error = new(e.Exception.Message);
            error.ShowDialog();
            e.Handled = true;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMemoryService, MemoryServiceImplementation>();
            containerRegistry.RegisterSingleton<IHashingService, HashingServiceImplementation>();
        }
    }
}
