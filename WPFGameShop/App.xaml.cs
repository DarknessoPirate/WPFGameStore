using WPFGameShop.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WPFGameShop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);



            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }

}
