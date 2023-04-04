using Microsoft.Extensions.DependencyInjection;
using Projekt_HjemIS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Services
{
    public static class ServiceManager
    {

        public static IServiceCollection UseCustomViews(this IServiceCollection services)
        {
            services.AddSingleton(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
            
            return services;
        }

        public static IServiceCollection UseCustomViewModels(this IServiceCollection services)
        {
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<EditCustomerViewModel>();

            return services;
        }

        public static IServiceCollection UseCustomMiscServices(this IServiceCollection services)
        {
            services.AddSingleton<Func<Type, BaseViewModel>>(
                serviceProvider => viewModelType =>
                    (BaseViewModel)serviceProvider.GetRequiredService(viewModelType));

            return services;
        }
    }
}
