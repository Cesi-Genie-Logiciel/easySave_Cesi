using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace EasySave.GUI.Views
{
    /// <summary>
    /// MainWindow conforme au diagramme v2.0 (lignes 22-25)
    /// Vue principale de l'application - Pattern MVVM strict (pas de logique ici)
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ouvre le dossier dans l'explorateur Windows lors du double-clic
        /// </summary>
        private void OpenFolder_Click(object sender, MouseButtonEventArgs e)
        {
            // Vérifier si c'est un double-clic
            if (e.ClickCount == 2 && sender is System.Windows.Controls.TextBlock textBlock)
            {
                var path = textBlock.Tag as string;
                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        // Vérifier si le dossier existe
                        if (Directory.Exists(path))
                        {
                            // Ouvrir l'explorateur Windows à ce chemin
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "explorer.exe",
                                Arguments = $"\"{path}\"",
                                UseShellExecute = true
                            });
                        }
                        else
                        {
                            System.Windows.MessageBox.Show(
                                $"Le dossier n'existe pas :\n{path}",
                                "Dossier introuvable",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(
                            $"Impossible d'ouvrir le dossier :\n{ex.Message}",
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
