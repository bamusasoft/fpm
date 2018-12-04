using System.ComponentModel.Composition.Hosting;
using System.Windows;
using FlopManager.Domain.EF;
using FlopManager.Services;
using FlopManager.Services.ViewModelInfrastructure;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Prism.Ioc;
using Xceed.Wpf.AvalonDock;
using System;

namespace FlopManager
{
    internal class Bootstrapper : PrismApplication
    {
        public Bootstrapper()
        {
        }

        //protected override void ConfigureAggregateCatalog()
        //{
        //    base.ConfigureAggregateCatalog();

        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(SettingsModule.SettingsModule).Assembly));
        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(FlopManagerLoanModule.LoanModule).Assembly));
        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(XmlLogger).Assembly));
        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(PaymentsModule.PaymentModule).Assembly));
        //    AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(FamilyContext).Assembly));

        //}
        //protected override IModuleCatalog CreateModuleCatalog()
        //{
        //    return new ConfigurationModuleCatalog();
        //}

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        //protected override void InitializeShell()
        //{
        //    base.InitializeShell();
        //    Application.Current.MainWindow = (Window)Shell;
        //    Application.Current.MainWindow.Show();
        //}

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);

            //regionAdapterMappings.RegisterMapping(typeof(DockingManager), new AvalonDockRegionAdapter(ConfigureDefaultRegionBehaviors()));
            regionAdapterMappings.RegisterMapping(typeof(DockingManager), Container.Resolve<AvalonDockRegionAdapter>());

        }



        //protected override void ConfigureContainer()
        //{
        //    base.ConfigureContainer();

        //    //Create global settings
        //    Container.RegisterType(typeof(FlopManagerSettings));
        //    //Create global customer logger which log to xml fiel
        //    Container.RegisterType(typeof(XmlLogger));
        //    //Here we register XmlLogger construcotr arguments.
        //    var setting = Container.Resolve<ISettings>();
        //    var logfilePath = (string)setting?["LogFilePath"];
        //    Container.RegisterType<string>( logfilePath);
        //    //Register Global Db Context
        //    Container.RegisterType(typeof(FamilyContext));
        //    //
        //    Container.RegisterType(typeof(FlopResoureces));




    }


    }

