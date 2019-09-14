using System;
using System.Windows;

namespace KaLEDoscope
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = GetInnerException(e.Exception);
            MessageBox.Show($"{exception.Message} => {exception.StackTrace}", "Необработанная ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }

        private Exception GetInnerException(Exception exception)
        {
            return exception.InnerException == null ? exception : GetInnerException(exception.InnerException);
        }
    }
}
