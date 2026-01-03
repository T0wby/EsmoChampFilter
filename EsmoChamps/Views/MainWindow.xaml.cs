using EsmoChamps.Data;
using EsmoChamps.Models;
using EsmoChamps.Utility;
using EsmoChamps.ViewModels;
using EsmoChamps.Views;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private void ChampionCardImage_Loaded(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            if (image == null) return;

            string imageName = image.Tag as string;

            if (!string.IsNullOrEmpty(imageName))
            {
                string imagePath = ImageManager.GetImagePath(imageName);

                if (imagePath != null && File.Exists(imagePath))
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.DecodePixelWidth = 50; // Optimize for card size
                        bitmap.EndInit();
                        bitmap.Freeze();

                        image.Source = bitmap;

                        // Hide the fallback letter
                        var parent = VisualTreeHelper.GetParent(image) as Grid;
                        if (parent != null)
                        {
                            var letter = parent.Children.OfType<TextBlock>().FirstOrDefault();
                            if (letter != null)
                            {
                                letter.Visibility = Visibility.Collapsed;
                            }
                        }
                        return;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to load card image: {ex.Message}");
                    }
                }
            }

            // Show fallback letter if image loading failed
            var parentGrid = VisualTreeHelper.GetParent(image) as Grid;
            if (parentGrid != null)
            {
                var letter = parentGrid.Children.OfType<TextBlock>().FirstOrDefault();
                if (letter != null)
                {
                    letter.Visibility = Visibility.Visible;
                }
            }
        }
    }
}