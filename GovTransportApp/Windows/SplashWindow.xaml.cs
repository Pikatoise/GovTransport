using System.Windows;
using System.Windows.Media.Animation;

namespace GovTransportApp.Windows
{
    public partial class SplashWindow: Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            var animationTask = AnimateLogoAsync();
            var initTask = InitializeAsync();

            await Task.WhenAll(animationTask, initTask);

            ((App)Application.Current).MainWindow = new AuthWindow();
            ((App)Application.Current).MainWindow.Show();
            this.Close();
        }

        private async Task AnimateLogoAsync()
        {
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            ImageLogo.BeginAnimation(OpacityProperty, fadeIn);

            await Task.Delay(fadeIn.Duration.TimeSpan);
        }

        private async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                ((App)Application.Current).InitServices();
            });
        }
    }
}
