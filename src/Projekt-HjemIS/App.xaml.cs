using Microsoft.Extensions.DependencyInjection;
using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt_HjemIS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _servicesProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            services.UseCustomMiscServices()
                    .UseCustomViewModels()
                    .UseCustomViews();

            _servicesProvider = services.BuildServiceProvider();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _servicesProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
