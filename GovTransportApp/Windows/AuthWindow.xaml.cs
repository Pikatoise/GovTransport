using GovAuthSDK;
using GovAuthSDK.Exceptions;
using System.Windows;

namespace GovTransportApp.Windows
{
    public partial class AuthWindow: Window
    {
        GovAuthService _authService = ((App)Application.Current).AuthService!;

        public AuthWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = TBoxLogin.Text.Trim();
            string password = PBoxPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            try
            {
                var user = await _authService.LoginAuth(login, password);

                MessageBox.Show("Ok");
            }
            catch (Exception ex)
            {
                if (ex is UserNotFoundException)
                {
                    MessageBox.Show("Пользователь не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                if (ex is WrongPasswordException)
                {
                    MessageBox.Show("Неверный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                MessageBox.Show("Неизвестная ошибка", "");

                throw;
            }
        }
    }
}
