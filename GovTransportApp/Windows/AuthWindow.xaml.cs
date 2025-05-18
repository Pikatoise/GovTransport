using GovAuthSDK.Enums;
using GovAuthSDK.Exceptions;
using GovTransportSDK;
using System.Windows;

namespace GovTransportApp.Windows
{
    public partial class AuthWindow: Window
    {
        GovTransportService _transportService = ((App)Application.Current).TransportService!;

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
                var user = await _transportService.Auth(login, password);

                if (user.AccessLevel != AccessLevel.High)
                {
                    MessageBox.Show("Недостаточно прав!", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                ((App)Application.Current).MainWindow = new MainWindow(user);
                ((App)Application.Current).MainWindow.Show();

                Close();
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
