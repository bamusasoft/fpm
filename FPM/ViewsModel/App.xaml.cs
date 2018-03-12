using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Fbm.ViewsModel.Helpers;
using Fbm.DomainModel;
using AppSettings = Fbm.ViewsModel.Properties.Settings;
namespace Fbm.ViewsModel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ar-SA");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ar-SA");
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            if (e.Args.Count() == 0)
            {
                GlobalConst.ServerName = ".";
            }
            else
            {
                GlobalConst.ServerName = e.Args[0];
            }
            CheckUpgrade();
            base.OnStartup(e);
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
            string msg = Helper.ProcessExceptionMessages(e.Exception);
            Logger.Log(LogMessageTypes.Error,
                msg,
                e.Exception.TargetSite.ToString(),
                e.Exception.StackTrace);
        }

    }
}
