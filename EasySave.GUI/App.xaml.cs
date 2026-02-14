using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using EasySave.GUI.ViewModels;
using EasySave.GUI.Views;
using EasySave.Interfaces;
using EasySave.Services;

namespace EasySave.GUI
{
    /// <summary>
    /// App conforme au diagramme v2.0 (lignes 12-14)
    /// Point d'entrée de l'application WPF avec Dependency Injection
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configuration du container DI
            var services = new ServiceCollection();

            // Services P2 (backend)
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IJobStorageService, JobStorageService>();
            services.AddSingleton<IBackupService, BackupService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            // Build le service provider
            ServiceProvider = services.BuildServiceProvider();

            // Créer et afficher la fenêtre principale
            var mainWindow = new MainWindow
            {
                DataContext = ServiceProvider.GetRequiredService<MainViewModel>()
            };

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Cleanup du service provider
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnExit(e);
        }
    }
}
