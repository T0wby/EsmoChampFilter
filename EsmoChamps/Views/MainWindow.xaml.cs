using EsmoChamps.Data;
using EsmoChamps.Models;
using EsmoChamps.ViewModels;
using EsmoChamps.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EsmoChamps
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            var border = sender as Border;
            var champion = border?.Tag as Champion;

            if (champion != null && DataContext is MainViewModel vm)
            {
                vm.SelectedChampion = champion;

                if (e.ClickCount == 2)
                {
                    vm.OpenChampionDetailsCommand.Execute(champion);
                }
            }
        }
    }
}