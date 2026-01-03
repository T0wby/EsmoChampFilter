using EsmoChamps.Models;
using EsmoChamps.Utility;
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
                InitialDirectory = ImageManager.UserImagesFolder
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var vm = DataContext as AddChampionViewModel;
                if (vm != null)
                {
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    vm.ImagePath = fileName;
                }
            }
        }
    }
}
