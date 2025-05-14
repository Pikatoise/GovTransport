using GovTransportSDK.DTO;
using GovTransportSDK.Models;
using System.Windows;

namespace GovTransportApp.Dialogs
{
    public partial class AddOwnerDialog: Window
    {
        public bool IsFinished = false;
        private bool isPassportOk = false;
        private Ownership? _ownership = null;

        public AddOwnerDialog()
        {
            InitializeComponent();
        }

        public AddOwnerDialog(Ownership ownership)
        {
            InitializeComponent();

            this._ownership = ownership;

            ButtonSave.Content = "Изменить";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._ownership != null)
            {
                TBoxPassport.Text = _ownership.Passport;
                isPassportOk = true;

                ButtonCheckPassport.IsEnabled = false;

                TBoxFullName.Text = _ownership.FullName;
                TBoxFullName.IsEnabled = true;

                TBoxOsago.Text = _ownership.Osago;
                TBoxOsago.IsEnabled = true;

                TBoxRegistration.Text = _ownership.RegistrationAddress;
                TBoxRegistration.IsEnabled = true;

                CBoxIsLegal.IsChecked = _ownership.IsLegal;
                CBoxIsLegal.IsEnabled = true;
            }
        }

        private void TBoxPassport_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string newPassport = TBoxPassport.Text;

            isPassportOk = false;

            ButtonCheckPassport.IsEnabled = true;
        }

        private async void ButtonCheckPassport_Click(object sender, RoutedEventArgs e)
        {
            string passport = TBoxPassport.Text.Trim();

            if (_ownership != null && _ownership.Passport.Equals(passport))
            {
                isPassportOk = true;
                ButtonCheckPassport.IsEnabled = false;

                return;
            }

            if (string.IsNullOrWhiteSpace(passport))
            {
                MessageBox.Show("Введите паспорт!", "Ошибка");

                return;
            }

            Ownership? ownerWithSamePassport = await ((App)Application.Current).TransportService.OwnerByPassport(passport);

            if (ownerWithSamePassport != null)
            {
                MessageBox.Show("Владелец с таким паспортом уже зарегистрирован!", "Ошибка");

                isPassportOk = false;
            }
            else
            {
                isPassportOk = true;

                ButtonCheckPassport.IsEnabled = false;

                TBoxFullName.IsEnabled = true;

                TBoxOsago.IsEnabled = true;

                TBoxRegistration.IsEnabled = true;

                CBoxIsLegal.IsEnabled = true;

                MessageBox.Show("Владелец с таким паспортом еще не зарегистрирован!", "Успех", MessageBoxButton.OK);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isPassportOk)
            {
                MessageBox.Show("Проверьте паспорт!", "Предупреждение");

                return;
            }

            string passport = TBoxPassport.Text.Trim();
            string fullName = TBoxFullName.Text.Trim();
            string registration = TBoxRegistration.Text.Trim();
            string osago = TBoxOsago.Text.Trim();
            bool isLegal;
            if (CBoxIsLegal.IsChecked == null)
                isLegal = false;
            else if (CBoxIsLegal.IsChecked == true)
                isLegal = true;
            else
                isLegal = false;

            if (string.IsNullOrEmpty(passport))
            {
                MessageBox.Show("Введите паспорт!", "Ошибка");

                return;
            }

            if (string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка");

                return;
            }

            if (string.IsNullOrEmpty(registration))
            {
                MessageBox.Show("Введите адрес регистрации!", "Ошибка");

                return;
            }

            if (_ownership != null)
            {
                _ownership.Passport = passport;
                _ownership.FullName = fullName;
                _ownership.RegistrationAddress = registration;
                _ownership.Osago = osago;
                _ownership.IsLegal = isLegal;

                await ((App)Application.Current).TransportService.UpdateOwnership(_ownership);
            }
            else
                await ((App)Application.Current).TransportService.AddOwnership(new AddOwnershipDto()
                {
                    Passport = passport,
                    FullName = fullName,
                    RegistrationAddress = registration,
                    Osago = osago,
                    IsLegal = isLegal
                });

            IsFinished = true;

            Close();
        }
    }
}
