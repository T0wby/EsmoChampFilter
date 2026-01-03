using EsmoChamps.Models;
using EsmoChamps.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace EsmoChamps.Views
{
    /// <summary>
    /// Interaktionslogik für AddChampionWindow.xaml
    /// </summary>
    public partial class AddChampionWindow : Window
    {
        private static readonly string ImagesFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "EsmoChamps",
            "ChampionImages"
        );
        public AddChampionWindow(AddChampionViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.RequestClose += () => Close();

            Title = vm.IsEditMode ? "Edit Champion" : "Add Champion";

            if (!Directory.Exists(ImagesFolder))
            {
                Directory.CreateDirectory(ImagesFolder);
            }
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Champion Image",
                Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All Files (*.*)|*.*",
                InitialDirectory = ImagesFolder
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var vm = DataContext as AddChampionViewModel;
                if (vm != null)
                {
                    // Get just the filename, not the full path
                    string fileName = Path.GetFileName(openFileDialog.FileName);

                    // If the file is not already in the ChampionImages folder, copy it
                    string targetPath = Path.Combine(ImagesFolder, fileName);
                    if (!File.Exists(targetPath) || openFileDialog.FileName != targetPath)
                    {
                        try
                        {
                            File.Copy(openFileDialog.FileName, targetPath, true);
                            MessageBox.Show($"Image copied to: {targetPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error copying image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Store just the filename
                    vm.ImagePath = fileName;
                }
            }
        }
    }
}
