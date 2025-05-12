using GovAuthSDK.Enums;
using GovAuthSDK.Exceptions;
using System.Windows;
using System.Windows.Controls;

namespace GovTransportApp.Dialogs
{
    public partial class AddUserDialog: Window
    {
        public bool IsFinished = false;

        public AddUserDialog()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = TBoxLogin.Text.Trim();
            string password = TBoxPassword.Password.Trim();
            var accessLevel = (AccessLevel)int.Parse((string)((ComboBoxItem)CBoxAccessLevel.SelectedItem).Tag);

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите Логин", "Ошибка");

                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите Логин", "Ошибка");

                return;
            }

            try
            {
                await ((App)Application.Current).AuthService!.AddUser(login, password, accessLevel);
            }
            catch (Exception ex)
            {
                if (ex is ExistUserWithSameLoginException)
                {
                    MessageBox.Show("Логин занят!", "Ошибка");

                    return;
                }

                throw;
            }

            MessageBox.Show("Успешно!");
            IsFinished = true;

            Close();
        }
    }
}
