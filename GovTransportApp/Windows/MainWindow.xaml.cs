using GovAuthSDK.DTO;
using GovTransportApp.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GovTransportApp.Windows
{
    public partial class MainWindow: Window
    {
        UserDto _user;
        UserControl _currentPage;

        public MainWindow(UserDto user)
        {
            InitializeComponent();

            _user = user;

            TBlockUsername.Text = _user.Login;

            _currentPage = new MainPage();
            PageNavigator.Content = _currentPage;
        }

        private void ButtonMainPage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangePage(new MainPage());
        }

        private void ButtonTransportPage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangePage(new TransportPage());
        }

        private void ButtonOwnersPage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangePage(new OwnersPage());
        }

        private async void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(new SettingsPage(_user));
        }

        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).MainWindow = new AuthWindow();

            ((App)Application.Current).MainWindow.Show();

            Close();
        }

        void ChangePage(UserControl page)
        {
            if (_currentPage.GetType() == page.GetType())
                return;

            UnSelectPageButton(_currentPage.GetType().Name);
            SelectPageButton(page.GetType().Name);

            _currentPage = page;
            PageNavigator.Content = _currentPage;
        }

        void SelectPageButton(string pageType)
        {
            switch (pageType)
            {
                case "MainPage":
                {
                    ButtonMainPage.BorderBrush = Brushes.Black;
                    IconMain.Foreground = Brushes.Black;
                    BorderMainText.Background = Brushes.Black;

                    return;
                }

                case "TransportPage":
                {
                    ButtonTransportPage.BorderBrush = Brushes.Black;
                    IconTransport.Foreground = Brushes.Black;
                    BorderTransportText.Background = Brushes.Black;

                    return;
                }

                case "OwnersPage":
                {
                    ButtonOwnersPage.BorderBrush = Brushes.Black;
                    IconOwners.Foreground = Brushes.Black;
                    BorderOwnersText.Background = Brushes.Black;

                    return;
                }

                case "SettingsPage":
                {
                    ButtonSettings.Background = Brushes.Black;

                    return;
                }
            }
        }

        void UnSelectPageButton(string pageType)
        {
            switch (pageType)
            {
                case "MainPage":
                {
                    ButtonMainPage.BorderBrush = Brushes.DimGray;
                    IconMain.Foreground = Brushes.DimGray;
                    BorderMainText.Background = Brushes.DimGray;

                    return;
                }

                case "TransportPage":
                {
                    ButtonTransportPage.BorderBrush = Brushes.DimGray;
                    IconTransport.Foreground = Brushes.DimGray;
                    BorderTransportText.Background = Brushes.DimGray;

                    return;
                }

                case "OwnersPage":
                {
                    ButtonOwnersPage.BorderBrush = Brushes.DimGray;
                    IconOwners.Foreground = Brushes.DimGray;
                    BorderOwnersText.Background = Brushes.DimGray;

                    return;
                }

                case "SettingsPage":
                {
                    ButtonSettings.Background = Brushes.DimGray;

                    return;
                }
            }
        }
    }
}