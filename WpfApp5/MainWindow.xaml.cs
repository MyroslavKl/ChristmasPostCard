using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp5
{
    public partial class MainWindow : Window
    {
        private const int BlizzardStrengthFactor = 5;
        private const double InitialFairyLightsOpacity = 0.3;
        private readonly DispatcherTimer _timer;
        private readonly Random _random;
        private bool _fairyLightsOn = false;
        private readonly DoubleAnimation _fairyLightsOpacityAnim;

        public MainWindow()
        {
            InitializeComponent();
            StartFloatingAnimation();

            _random = new Random();
            _timer = new DispatcherTimer();
            ChangeBlizzardStrength();
            _timer.Tick += (_, _) => SpawnBlizzard();
            _timer.Start();
            _fairyLightsOpacityAnim = new DoubleAnimation
            {
                From = InitialFairyLightsOpacity,
                To = 1,
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            FairyLightsCanvas.Opacity = InitialFairyLightsOpacity;
        }
        private void StartFloatingAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = main.Width - 380,
                Duration = TimeSpan.FromSeconds(5),
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };
            floatingText.BeginAnimation(Canvas.LeftProperty, animation);
        }

        public void ChangeBlizzardStrength(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            BlizzardStrengthValue.Text = $"{BlizzardStrengthSlider.Value:N1}";
            ChangeBlizzardStrength();
        }

        public void ChangeBlizzardStrength()
        {
            double strength = BlizzardStrengthSlider.Value;
            double period = strength > 0 ? 1 / strength : 2;
            _timer.Interval = TimeSpan.FromSeconds(period);
        }

        public void SpawnBlizzard()
        {
            int n = BlizzardStrengthInt * BlizzardStrengthFactor;
            double step = BlizzardCanvas.ActualWidth / (n + 0);
            for (var i = 0; i < n; ++i) {
                SpawnSnowflake(i * step + step / 2);
            }
                
        }

        public void SpawnSnowflake(double centerX)
        {
            var snowflake = new TextBlock
            {
                Text = "*",
                FontSize = 30 + _random.NextInt64(0, BlizzardStrengthInt * 2),
                Foreground = new SolidColorBrush(Colors.Aqua)
            };

            const int deviation = 48;
            double x = centerX - snowflake.ActualWidth / 2 - _random.NextInt64(-deviation, deviation);
            double y = -snowflake.ActualHeight - _random.NextInt64(0, deviation);

            snowflake.SetValue(Canvas.LeftProperty, x);
            snowflake.SetValue(Canvas.TopProperty, y);

            BlizzardCanvas.Children.Add(snowflake);

            var duration = TimeSpan.FromSeconds(5 - BlizzardStrengthSlider.Value + 2 * _random.NextDouble());

            var animY = new DoubleAnimation
            {
                From = y,
                To = BlizzardCanvas.ActualHeight + snowflake.ActualHeight,
                Duration = duration
            };

            if (BlizzardAngleSlider.Value != 0)
            {
                var animX = new DoubleAnimation
                {
                    From = x,
                    To = x + BlizzardAngleSlider.Value / 45 * BlizzardCanvas.ActualHeight,
                    Duration = duration
                };
                snowflake.BeginAnimation(Canvas.LeftProperty, animX);
            }
            animY.Completed += (_, _) => BlizzardCanvas.Children.Remove(snowflake);
            snowflake.BeginAnimation(Canvas.TopProperty, animY);

            var animAngle = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(_random.NextInt64(4, 6)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            var transform = new RotateTransform();
            snowflake.RenderTransform = transform;

            transform.BeginAnimation(RotateTransform.AngleProperty, animAngle);
        }

        private int BlizzardStrengthInt => Convert.ToInt32(Math.Round(BlizzardStrengthSlider.Value));

        public void CreateFairyLight(object sender, MouseButtonEventArgs e)
        {
            if (Picker.SelectedColor == null)
                return;

            Point pos = e.GetPosition(Tree);
            var bulb = new Ellipse
            {
                Fill = new SolidColorBrush(Picker.SelectedColor.Value),
                Width = 10,
                Height = 10
            };
            bulb.SetValue(Canvas.LeftProperty, pos.X);
            bulb.SetValue(Canvas.TopProperty, pos.Y);
            FairyLightsCanvas.Children.Add(bulb);
            bulb.MouseRightButtonUp += (_, _) => FairyLightsCanvas.Children.Remove(bulb);
        }

        private void ToggleFairyLights(object sender, RoutedEventArgs args)
        {
            if (_fairyLightsOn)
            {
                TurnFairyLightsOff();
            }
            else {
                TurnFairyLightsOn();
            }
                
        }

        private void TurnFairyLightsOn()
        {
            FairyLightsCanvas.BeginAnimation(OpacityProperty, _fairyLightsOpacityAnim);
        }

        private void TurnFairyLightsOff()
        {
            FairyLightsCanvas.BeginAnimation(OpacityProperty, null);
        }

        private void ChangeBlizzardAngle(object sender, RoutedEventArgs args)
        {
            BlizzardAngleValue.Text = $"{BlizzardAngleSlider.Value:N1}";
        }
    }


}

