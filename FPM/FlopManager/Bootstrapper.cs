using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FlopManager.Services;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Logging;
using Prism.Mef;
using Prism.Modularity;
using Prism.Regions;
using Xceed.Wpf.AvalonDock;

namespace FlopManager
{
    internal class Bootstrapper : MefBootstrapper
    {

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(SettingsModule.SettingsModule).Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(FlopManagerLoanModule.LoanModule).Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(XmlLogger).Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(PaymentsModule.PaymentModule).Assembly));

        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.GetExportedValue<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mapping = base.ConfigureRegionAdapterMappings();
            if (mapping == null) return null;

            mapping.RegisterMapping(typeof(DockingManager), new AvalonDockRegionAdapter(ConfigureDefaultRegionBehaviors()));
            return mapping;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            //Create global settings
            Container.ComposeExportedValue(typeof(FlopManagerSettings));
            //Create global customer logger which log to xml fiel
            Container.ComposeExportedValue(typeof(XmlLogger));
            //Here we register XmlLogger construcotr arguments.
            var setting = Container.GetExport<ISettings>();
            var logfilePath = (string)setting?.Value["LogFilePath"];
            Container.ComposeExportedValue<string>("LogFilePath", logfilePath);



        }


    }
}
