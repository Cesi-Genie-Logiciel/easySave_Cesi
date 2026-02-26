using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace EasySave.GUI.Views
{
    // Main window code-behind.
    // Kept minimal because we follow MVVM. The only logic here is
    // opening folders on double-click, which is purely a UI concern.
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Opens the folder in Windows Explorer when the user double-clicks a path
        private void OpenFolder_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is System.Windows.Controls.TextBlock textBlock)
            {
                string? path = textBlock.Tag as string;
                if (!string.IsNullOrEmpty(path))
                {
                    LanguageManager lang = LanguageManager.Instance;
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "explorer.exe",
                                Arguments = "\"" + path + "\"",
                                UseShellExecute = true
                            });
                        }
                        else
                        {
                            MessageBox.Show(
                                lang.Translate("FolderNotFound") + ":\n" + path,
                                lang.Translate("Error"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            lang.Translate("CannotOpenFolder") + ":\n" + ex.Message,
                            lang.Translate("Error"),
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}