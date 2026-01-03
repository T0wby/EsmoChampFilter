using EsmoChamps.Data;
using EsmoChamps.Models;
using EsmoChamps.ViewModels;
using EsmoChamps.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            System.Diagnostics.Debug.WriteLine($"ClickCount: {e.ClickCount}");
            var border = sender as Border;
            var champion = border?.Tag as Champion;

            if (champion != null && DataContext is MainViewModel vm)
            {
                if (e.ClickCount == 1)
                {
                    vm.SelectedChampion = champion;
                    UpdateCardVisual(border, true);

                    UpdateAllCardSelections(border);
                }
                else if (e.ClickCount == 2)
                {
                    vm.SelectedChampion = champion;
                    vm.OpenChampionDetailsCommand.Execute(champion);
                    e.Handled = true;
                }
            }
        }

        private void UpdateCardVisual(Border border, bool isSelected)
        {
            if (isSelected)
            {
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B8CFF"));
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E28"));
            }
            else
            {
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2A32"));
                border.BorderThickness = new Thickness(1);
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16161C"));
            }
        }

        private void UpdateAllCardSelections(Border selectedBorder)
        {
            if (DataContext is MainViewModel vm && vm.SelectedChampion != null)
            {
                // Find the ItemsControl
                var scrollViewer = FindName("ChampionScrollViewer") as ScrollViewer;
                if (scrollViewer == null)
                {
                    // Try to find it through visual tree
                    scrollViewer = FindVisualChild<ScrollViewer>(this);
                }

                if (scrollViewer != null)
                {
                    var itemsControl = FindVisualChild<ItemsControl>(scrollViewer);
                    if (itemsControl != null)
                    {
                        // Iterate through all Border elements in the visual tree
                        var borders = FindVisualChildren<Border>(itemsControl);

                        foreach (var border in borders)
                        {
                            // Only process card borders (those with Champion tags)
                            if (border.Tag is Champion champion)
                            {
                                bool isSelected = champion.Id == vm.SelectedChampion.Id;
                                UpdateCardVisual(border, isSelected);
                            }
                        }
                    }
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    yield return typedChild;

                foreach (var grandChild in FindVisualChildren<T>(child))
                {
                    yield return grandChild;
                }
            }
        }
    }
}