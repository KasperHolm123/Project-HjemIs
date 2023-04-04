using Microsoft.Extensions.DependencyInjection;
using Projekt_HjemIS.ViewModels;
using Projekt_HjemIS.Views.Login;
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
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<UsersViewModel>();
            services.AddSingleton<ProductsViewModel>();
            services.AddSingleton<EmailViewModel>();
            services.AddSingleton<LogViewModel>();
            services.AddSingleton<EmulatorViewModel>();

            return services;
        }

        public static IServiceCollection UseCustomMiscServices(this IServiceCollection services)
        {
            services.AddSingleton<Func<Type, BaseViewModel>>(
                serviceProvider => viewModelType =>
                    (BaseViewModel)serviceProvider.GetRequiredService(viewModelType));
            services.AddSingleton<INavigationService, NavigationService>();

            return services;
        }
    }
}
