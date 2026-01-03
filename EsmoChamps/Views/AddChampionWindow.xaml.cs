using EsmoChamps.ViewModels;
using EsmoChamps.Models;
using System.Windows;

namespace EsmoChamps.Views
{
    /// <summary>
    /// Interaktionslogik für AddChampionWindow.xaml
    /// </summary>
    public partial class AddChampionWindow : Window
    {
        public AddChampionWindow(AddChampionViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.RequestClose += () => Close();

            Title = vm.IsEditMode ? "Edit Champion" : "Add Champion";
        }
    }
}
