using ElvUiUpdater.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace ElvUiUpdater
{
    public partial class App
    {
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) =>
            {
                // App Host
                services.AddHostedService<ApplicationHostService>();

                // Dialog service
                services.AddSingleton<IDialogService, DialogService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                services.AddScoped<INavigationWindow, Views.Windows.MainWindow>();
                services.AddScoped<ViewModels.MainWindowViewModel>();

                // Views and ViewModels
                services.AddScoped<Views.Pages.HomePage>();
                services.AddScoped<ViewModels.HomeViewModel>();
                services.AddScoped<Views.Pages.AddonsListPage>();
                services.AddScoped<ViewModels.AddonsListViewModel>();
                services.AddScoped<Views.Pages.ElvUIPage>();
                services.AddScoped<ViewModels.ElvUIViewModel>();
                services.AddScoped<Views.Pages.AUIPage>();
                services.AddScoped<ViewModels.AUIViewModel>();
                services.AddScoped<Views.Pages.FAQPage>();
                services.AddScoped<ViewModels.FAQViewModel>();
                services.AddScoped<Views.Pages.SettingsPage>();
                services.AddScoped<ViewModels.SettingsViewModel>();
            }).Build();

        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
            ElvUiUpdater.Properties.Settings.Default.Save();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            
        }
    }
}