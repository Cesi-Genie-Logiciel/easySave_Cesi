using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using EasySave.GUI.ViewModels;
using EasySave.GUI.Views;
using EasySave.Interfaces;
using EasySave.Services;

namespace EasySave.GUI
{
    public partial class App : System.Windows.Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ServiceCollection services = new ServiceCollection();

            // Load settings to check if a business software is configured
            SettingsService settingsService = new SettingsService();
            services.AddSingleton<ISettingsService>(settingsService);
            services.AddSingleton<IJobStorageService, JobStorageService>();

            // Build the BackupService manually so we can pass the detector
            string businessSoftwareName = settingsService.Load().BusinessSoftwareName;
            BusinessSoftwareDetector? detector = null;

            if (!string.IsNullOrWhiteSpace(businessSoftwareName))
            {
                detector = new BusinessSoftwareDetector(businessSoftwareName, 500);
            }

            // We build BackupService ourselves instead of letting DI do it,
            // because DI does not inject the optional detector properly
            IJobStorageService storageService = new JobStorageService();
            services.AddSingleton<IJobStorageService>(storageService);
            BackupService backupService = new BackupService(storageService, detector);
            services.AddSingleton<IBackupService>(backupService);

            services.AddTransient<MainViewModel>();

            ServiceProvider = services.BuildServiceProvider();

            MainWindow mainWindow = new MainWindow
            {
                DataContext = ServiceProvider.GetRequiredService<MainViewModel>()
            };

            mainWindow.Show();
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