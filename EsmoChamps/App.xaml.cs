using EsmoChamps.Utility;
using System.Configuration;
using System.Data;
using System.Windows;

namespace EsmoChamps
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ImageManager.InitializeImageFolder();

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Unhandled exception: {e.Exception.Message}\n\n{e.Exception.StackTrace}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true; // Prevent app from crashing
        }
    }

}
