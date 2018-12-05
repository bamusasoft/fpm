using FlopManager.Domain.EF;
using FlopManager.Services;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Xceed.Wpf.AvalonDock;
using AppSettings = FlopManager.Properties.Settings;

namespace FlopManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
     



        private void CheckUpgrade()
        {
            if (AppSettings.Default.UpgradedVersion)
            {
                AppSettings.Default.Upgrade();
                AppSettings.Default.UpgradedVersion = false;
                AppSettings.Default.Save();
            }
        }
        private void CheckReportingTemplatesSettings()
        {
            string templatesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Templates";
            string paymentTransTemplate = templatesDirectory + "\\PaymReport.xltx";
            if (File.Exists(paymentTransTemplate))
            {
                AppSettings.Default.PaymReportPath = paymentTransTemplate;
                AppSettings.Default.Save();
            }

        }
       

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);

            regionAdapterMappings.RegisterMapping(typeof(DockingManager), Container.Resolve<AvalonDockRegionAdapter>());

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<DockingManager>();
            //containerRegistry.RegisterSingleton<AvalonDockRegionAdapter>();

            //AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            //        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(SettingsModule.SettingsModule).Assembly));
            //        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(FlopManagerLoanModule.LoanModule).Assembly));
            //        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(XmlLogger).Assembly));
            //        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(PaymentsModule.PaymentModule).Assembly));
            //        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(FamilyContext).Assembly));
            //containerRegistry.Register(new Ass)

            containerRegistry.Register<ISettings, FlopManagerSettings>();
            
            //Create global customer logger which log to xml fiel
            containerRegistry.Register<ILogger, XmlLogger>();
            
            //Here we register XmlLogger construcotr arguments.
            var setting = Container.Resolve<ISettings>();
            var logfilePath = (string)setting?["LogFilePath"];
            containerRegistry.RegisterInstance<string>(logfilePath);
            
            //Register Global Db Context
            containerRegistry.Register(typeof(FamilyContext));
            
            //
            containerRegistry.Register(typeof(FlopResoureces));
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            

        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<FlopManagerLoanModule.LoanModule>();
           
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            //Call base OnStartup first to let Prism do its work.
            base.OnStartup(e);

            CheckUpgrade();
            CheckReportingTemplatesSettings();
            if (!e.Args.Any())
            {
                ConnectionString.ServerName = ".";
            }
            else
            {
                ConnectionString.ServerName = e.Args[0];
            }
            AppSettings.Default.Save();

        }

    }
}
