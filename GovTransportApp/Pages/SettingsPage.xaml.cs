using GovAuthSDK;
using GovAuthSDK.DTO;
using GovTransportApp.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace GovTransportApp.Pages
{
    public partial class SettingsPage: UserControl
    {
        GovAuthService _authService;

        public SettingsPage()
        {
            InitializeComponent();

            _authService = ((App)Application.Current).AuthService!;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var users = await _authService.AllUsers();
            var tokens = await _authService.AllTokens();

            DGridUsers.ItemsSource = users;
            DGridTokens.ItemsSource = tokens;
        }

        private async void ButtonAddUser_Click(object sender, RoutedEventArgs e)
        {
            var addUserDialog = new AddUserDialog();

            addUserDialog.ShowDialog();

            if (addUserDialog.IsFinished)
            {
                var users = await _authService.AllUsers();
                DGridUsers.ItemsSource = users;
            }
        }

        private async void ButtonRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            UserDto? selectedItem = DGridUsers.SelectedItem as UserDto;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }

            var result = MessageBox.Show($"Удалить пользователя {selectedItem.Login}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            await _authService.DeleteUser(selectedItem.Id);

            var users = await _authService.AllUsers();
            DGridUsers.ItemsSource = users;
        }

        private async void ButtonAddToken_Click(object sender, RoutedEventArgs e)
        {
            var addTokenDialog = new AddTokenDialog();

            addTokenDialog.ShowDialog();

            if (addTokenDialog.IsFinished)
            {
                var tokens = await _authService.AllTokens();
                DGridTokens.ItemsSource = tokens;
            }
        }

        private async void ButtonRemoveToken_Click(object sender, RoutedEventArgs e)
        {
            TokenDto? selectedItem = DGridTokens.SelectedItem as TokenDto;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите токен!");
                return;
            }

            var result = MessageBox.Show($"Удалить токен {selectedItem.Description}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            await _authService.DeleteToken(selectedItem.TokenValue);

            var tokens = await _authService.AllTokens();
            DGridTokens.ItemsSource = tokens;
        }
    }
}
