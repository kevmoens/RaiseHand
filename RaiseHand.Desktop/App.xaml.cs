using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Prism.Ioc;
using Prism.Mvvm;
using RaiseHand.Core;
using RaiseHand.Core.Messaging;
using RaiseHand.Core.Navigation;
using RaiseHand.Core.SignalR;
using RaiseHand.Core.Threading;
using RaiseHand.Core.ViewModels;
using RaiseHand.Desktop.Messaging;
using RaiseHand.Desktop.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace RaiseHand.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        IServiceCollection services;


        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainWindow>();
            return w;
        }
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                Locator locator = new Locator();
                var viewName = viewType.FullName.Replace("RaiseHand.Desktop.Views.", "RaiseHand.Core.ViewModels.");
                var viewAssemblyName = locator.GetType().Assembly.FullName;
                var viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            //Prism to Microsoft DI
            containerRegistry.RegisterServices((s) =>
            {
                ConfigureServices(s);
                services = s;
            });

            containerRegistry.RegisterForNavigation<MainWindow>("MainWindow");
            containerRegistry.RegisterForNavigation<Home>("Home");
            containerRegistry.RegisterForNavigation<Host>("Host");
            containerRegistry.RegisterForNavigation<Join>("Join");
            containerRegistry.RegisterForNavigation<Meeting>("Meeting");


            var serviceProvider = services.BuildServiceProvider();
            containerRegistry.RegisterInstance<IServiceProvider>(serviceProvider);
            //ALL SINGLETONS SO WE DON'T CREATE MULTIPLE from both DI containers
            //containerRegistry.RegisterInstance<Globals>(serviceProvider.GetService<Globals>());
            //containerRegistry.RegisterInstance<Utility>(serviceProvider.GetService<Utility>());
            //containerRegistry.RegisterInstance<LCInstallValidation>(serviceProvider.GetService<LCInstallValidation>());

        }

        public static ServiceProvider ServiceProvider;
        private void ConfigureServices(IServiceCollection services)
        {
            //Register everything but navigation here    
            services.AddTransient<IMessage, Message>();
            services.AddTransient<IRHDispatcher, Threading.RHDispatcher>();
            services.AddTransient<INavigation, Navigation.Navigation>();
            services.AddSingleton<NavigationState>();
            services.AddTransient<MainWindowViewModel>();

            services.AddSingleton<Connection>();
            //Logging
            services.AddLogging(builder =>
            {
                builder.AddNLog();
            });

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
           
            var serviceProvider = Container.Resolve<IServiceProvider>();
            var logfactory = serviceProvider.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
            System.Reflection.FieldInfo _filterOptionsMember = logfactory.GetType().GetField("_filterOptions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
            var _filterOptions = (Microsoft.Extensions.Logging.LoggerFilterOptions)_filterOptionsMember.GetValue(logfactory);
            _filterOptions.MinLevel = LogLevel.Trace;

            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

        }
        static void Application_ThreadException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Unhandled Thread Exception");
            // here you can log the exception ...
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show((e.ExceptionObject as Exception).Message, "Unhandled UI Exception");
            // here you can log the exception ...
        }
    }
}
