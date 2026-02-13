using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using EasySave.Services.Implementations;
using EasySave.Services.Interfaces;
using EasySave.ViewModels;

namespace EasySave.WPF
{
    /// Application entry point with dependency injection setup.
    /// Configures services and initializes main window.
    public partial class App : Application
    {
        public IServiceProvider? ServiceProvider { get; private set; }

        protected void OnStartup(object sender, StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Services - mock for P1, will be replaced with real implementation in P2
            services.AddSingleton<IBackupService, MockBackupService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();

            // Views
            services.AddTransient<MainWindow>(provider =>
            {
                var window = new MainWindow
                {
                    DataContext = provider.GetRequiredService<MainViewModel>()
                };
                return window;
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }
}
