using EsmoChamps.Utility;
using EsmoChamps.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EsmoChamps.Views
{
    /// <summary>
    /// Interaction logic for ChampionDetailWindow.xaml
    /// </summary>
    public partial class ChampionDetailWindow : Window
    {
        private static readonly string ImagesFolder = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "EsmoChamps",
            "ChampionImages"
        );

        public ChampionDetailWindow(ChampionDetailViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Title = vm.Name;
            this.Loaded += ChampionDetailWindow_Loaded;
            //vm.RequestClose += () => Close();
            SetupHoverEffects();
            EnsureImagesFolderExists();
            LoadChampionImage();
        }

        private void ChampionDetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PowerCurveCanvas.SizeChanged += PowerCurveCanvas_SizeChanged;

            if (PowerCurveCanvas.ActualWidth > 0)
            {
                DrawPowerCurve();
            }

            DrawDifficultyRanges();
        }

        #region PowerCurve
        private void PowerCurveCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                DrawPowerCurve();
            }
        }

        private void DrawPowerCurve()
        {
            ChampionDetailViewModel? vm = this.DataContext as ChampionDetailViewModel;
            if (vm == null) return;

            // Power curve values (0-100 scale)
            double earlyPower = Convert.ToDouble(vm.PowerStart);
            double midPower = Convert.ToDouble(vm.PowerMid);
            double latePower = Convert.ToDouble(vm.PowerEnd);

            // Update value labels
            EarlyValue.Text = earlyPower.ToString("F0");
            MidValue.Text = midPower.ToString("F0");
            LateValue.Text = latePower.ToString("F0");

            double width = PowerCurveCanvas.ActualWidth;
            double height = 250;

            // Safety check
            if (width <= 0) return;

            // Calculate positions (invert Y because canvas Y increases downward)
            double x1 = 5;
            double y1 = height - (earlyPower / 100.0 * height);

            double x2 = width / 2;
            double y2 = height - (midPower / 100.0 * height);

            double x3 = width - 5;
            double y3 = height - (latePower / 100.0 * height);

            // Create smooth Bezier curve
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure { StartPoint = new Point(x1, y1) };

            // Calculate control points for smooth curve
            double controlOffset = width / 4;
            Point control1 = new Point(x1 + controlOffset, y1);
            Point control2 = new Point(x2 - controlOffset, y2);

            // Early to Mid
            pathFigure.Segments.Add(new BezierSegment(
                control1,
                control2,
                new Point(x2, y2),
                true
            ));

            // Mid to Late
            Point control3 = new Point(x2 + controlOffset, y2);
            Point control4 = new Point(x3 - controlOffset, y3);

            pathFigure.Segments.Add(new BezierSegment(
                control3,
                control4,
                new Point(x3, y3),
                true
            ));

            pathGeometry.Figures.Add(pathFigure);
            CurveLinePath.Data = pathGeometry;

            // Fill area under curve
            PathFigure fillFigure = new PathFigure { StartPoint = new Point(x1, height) };
            fillFigure.Segments.Add(new LineSegment(new Point(x1, y1), true));
            fillFigure.Segments.Add(new BezierSegment(control1, control2, new Point(x2, y2), true));
            fillFigure.Segments.Add(new BezierSegment(control3, control4, new Point(x3, y3), true));
            fillFigure.Segments.Add(new LineSegment(new Point(x3, height), true));
            fillFigure.IsClosed = true;

            PathGeometry fillGeometry = new PathGeometry();
            fillGeometry.Figures.Add(fillFigure);
            CurveFillPath.Data = fillGeometry;

            // Position data point GROUPS (not individual points)
            Canvas.SetLeft(EarlyPointGroup, x1 - 5);
            Canvas.SetTop(EarlyPointGroup, y1 - 5);

            Canvas.SetLeft(MidPointGroup, x2 - 5);
            Canvas.SetTop(MidPointGroup, y2 - 5);

            Canvas.SetLeft(LatePointGroup, x3 - 5);
            Canvas.SetTop(LatePointGroup, y3 - 5);

            // Set initial label opacity
            EarlyLabel.Opacity = 0.8;
            MidLabel.Opacity = 0.8;
            LateLabel.Opacity = 0.8;
        }

        private void SetupHoverEffects()
        {
            // Early point hover
            EarlyPointGroup.MouseEnter += (s, e) => AnimatePoint(EarlyPoint, EarlyLabel, true);
            EarlyPointGroup.MouseLeave += (s, e) => AnimatePoint(EarlyPoint, EarlyLabel, false);

            // Mid point hover
            MidPointGroup.MouseEnter += (s, e) => AnimatePoint(MidPoint, MidLabel, true);
            MidPointGroup.MouseLeave += (s, e) => AnimatePoint(MidPoint, MidLabel, false);

            // Late point hover
            LatePointGroup.MouseEnter += (s, e) => AnimatePoint(LatePoint, LateLabel, true);
            LatePointGroup.MouseLeave += (s, e) => AnimatePoint(LatePoint, LateLabel, false);
        }

        private void AnimatePoint(Ellipse point, Border label, bool isHover)
        {
            // Animate point size
            DoubleAnimation sizeAnimation = new DoubleAnimation
            {
                To = isHover ? 14 : 10,
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            point.BeginAnimation(Ellipse.WidthProperty, sizeAnimation);
            point.BeginAnimation(Ellipse.HeightProperty, sizeAnimation);

            // Animate label
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                To = isHover ? 1.0 : 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };

            DoubleAnimation scaleAnimation = new DoubleAnimation
            {
                To = isHover ? 1.1 : 1.0,
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            label.BeginAnimation(OpacityProperty, opacityAnimation);

            ScaleTransform? scale = label.RenderTransform as ScaleTransform;
            if (scale == null)
            {
                scale = new ScaleTransform(1.0, 1.0, 0.5, 0.5);
                label.RenderTransform = scale;
                label.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }
        #endregion

        #region Difficulty
        private void DrawDifficultyRanges()
        {
            ChampionDetailViewModel? vm = this.DataContext as ChampionDetailViewModel;
            if (vm == null) return;

            double maxWidth = MechanicsBarContainer.ActualWidth;

            UpdateBar(MechanicsBar, vm.MechanicsMin, vm.MechanicsMax, maxWidth);
            UpdateBar(MacroBar, vm.MacroMin, vm.MacroMax, maxWidth);
            UpdateBar(TacticalBar, vm.TacticalMin, vm.TacticalMax, maxWidth);
        }

        private void UpdateBar(Border bar, int min, int max, double containerWidth)
        {
            double startPercent = min / 100.0;
            double widthPercent = (max - min) / 100.0;

            bar.Width = containerWidth * widthPercent;
            bar.Margin = new Thickness(containerWidth * startPercent, 0, 0, 0);
        }
        #endregion

        #region Images

        private void LoadChampionImage()
        {
            ChampionDetailViewModel? vm = this.DataContext as ChampionDetailViewModel;
            if (vm == null) return;

            string imagePath = ImageManager.GetImagePath(vm.ImagePath);

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    ChampionImage.Source = bitmap;
                    return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load image: {ex.Message}");
                }
            }

            DrawDefaultIcon(vm.Name);
        }

        private void DrawDefaultIcon(string name)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                // Background gradient
                RadialGradientBrush brush = new RadialGradientBrush();
                brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#9B8CFF"), 0));
                brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#7B6CDF"), 1));

                dc.DrawRectangle(brush, null, new Rect(0, 0, 60, 60));

                // First letter
                string initial = string.IsNullOrEmpty(name) ? "?" : name.Substring(0, 1).ToUpper();
                FormattedText text = new FormattedText(
                    initial,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                    32,
                    Brushes.White,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                dc.DrawText(text, new Point(30 - text.Width / 2, 30 - text.Height / 2));
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap(60, 60, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            bitmap.Freeze();
            ChampionImage.Source = bitmap;
        }

        private void EnsureImagesFolderExists()
        {
            if (!Directory.Exists(ImagesFolder))
            {
                Directory.CreateDirectory(ImagesFolder);

                // Optionally create a README file to help users
                string readmePath = System.IO.Path.Combine(ImagesFolder, "README.txt");
                File.WriteAllText(readmePath,
                    "Champion Images Folder\n" +
                    "=====================\n\n" +
                    "Place your champion images here.\n" +
                    "Image files should be named exactly as the champion name (e.g., 'Ahri.png', 'Zed.jpg').\n" +
                    "Supported formats: .png, .jpg, .jpeg, .bmp\n");
            }
        }
        #endregion
    }
}
