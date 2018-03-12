using System.Linq;
using System.Windows;
using FlopManager.Domain.EF;
using AppSettings =FlopManager.Properties.Settings;
namespace FlopManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            CheckUpgrade();
            if (!e.Args.Any())
            {
                ConnectionString.ServerName = ".";
            }
            else
            {
                ConnectionString.ServerName = e.Args[0];
            }
            AppSettings.Default.Save();
            //Logger.LogFilePath = AppSettings.Default.LogFilePath;
            //GlobalConst.CurrentYear = AppSettings.Default.CurrentYear;
            //base.OnStartup(e);
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
        private void CheckUpgrade()
        {
            if (AppSettings.Default.UpgradedVersion)
            {
                AppSettings.Default.Upgrade();
                AppSettings.Default.UpgradedVersion = false;
                AppSettings.Default.Save();
            }
        }
        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //string msg = Helper.ProcessExceptionMessages(e.Exception);
            //Logger.Log(LogMessageTypes.Error,
            //    msg,
            //    e.Exception.TargetSite.ToString(),
            //    e.Exception.StackTrace);
        }
    }
}
