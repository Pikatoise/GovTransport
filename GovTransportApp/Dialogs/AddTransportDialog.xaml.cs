using GovTransportSDK.DTO;
using GovTransportSDK.Enums;
using GovTransportSDK.Models;
using System.Windows;
using System.Windows.Controls;

namespace GovTransportApp.Dialogs
{
    public partial class AddTransportDialog: Window
    {
        public bool IsFinished = false;
        private bool isVinOk = false;
        private Transport? _transport = null;
        public Guid? NewTransportId;

        public AddTransportDialog()
        {
            InitializeComponent();
        }

        public AddTransportDialog(Transport transport)
        {
            InitializeComponent();

            this._transport = transport;

            ButtonSave.Content = "Изменить";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._transport != null)
            {
                TBoxVin.Text = _transport.VIN;
                isVinOk = true;

                ButtonCheckVin.IsEnabled = false;

                TBoxModel.Text = _transport.Model;
                TBoxModel.IsEnabled = true;

                CBoxBodyType.SelectedItem = CBoxBodyType.Items
                    .OfType<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == _transport.BodyType.ToString());
                CBoxBodyType.IsEnabled = true;

                TBoxColor.Text = _transport.Color;
                TBoxColor.IsEnabled = true;

                TBoxReleaseYear.Text = _transport.ReleaseYear.ToString();
                TBoxReleaseYear.IsEnabled = true;

                CBoxStatus.SelectedItem = CBoxStatus.Items
                    .OfType<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == _transport.Status.ToString());
                CBoxStatus.IsEnabled = true;

                TBoxGovNumber.Text = _transport.GovNumber;
                ButtonGetGovNumber.IsEnabled = true;
            }
        }

        private void TBoxVin_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newVin = TBoxVin.Text;

            isVinOk = false;

            ButtonCheckVin.IsEnabled = true;
        }

        private async void ButtonCheckVin_Click(object sender, RoutedEventArgs e)
        {
            string vin = TBoxVin.Text.Trim();

            if (_transport != null && _transport.VIN.Equals(vin))
            {
                isVinOk = true;
                ButtonCheckVin.IsEnabled = false;

                return;
            }

            if (string.IsNullOrWhiteSpace(vin))
            {
                MessageBox.Show("Введите VIN!", "Ошибка");

                return;
            }

            Transport? transportWithSameVin = await ((App)Application.Current).TransportService.FindTransportByVIN(vin);

            if (transportWithSameVin != null)
            {
                MessageBox.Show("Транспорт с таким VIN номером уже зарегистрирован!", "Ошибка");

                isVinOk = false;
            }
            else
            {
                isVinOk = true;

                ButtonCheckVin.IsEnabled = false;

                TBoxModel.IsEnabled = true;

                CBoxBodyType.IsEnabled = true;

                TBoxColor.IsEnabled = true;

                TBoxReleaseYear.IsEnabled = true;

                CBoxStatus.IsEnabled = true;

                ButtonGetGovNumber.IsEnabled = true;

                MessageBox.Show("Транспорт с таким VIN номером еще не зарегистрирован!", "Успех", MessageBoxButton.OK);
            }
        }

        private void ButtonGetGovNumber_Click(object sender, RoutedEventArgs e)
        {
            var generateGovNumberDialog = new GenerateGovNumberDialog();

            generateGovNumberDialog.ShowDialog();

            if (generateGovNumberDialog.IsFinished)
                TBoxGovNumber.Text = generateGovNumberDialog.NewNumber;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!isVinOk)
            {
                MessageBox.Show("Проверьте VIN!", "Предупреждение");

                return;
            }

            string vin = TBoxVin.Text.Trim();
            string model = TBoxModel.Text.Trim();

            string bodyTypeСontent = (CBoxBodyType.SelectedItem as ComboBoxItem).Content as string;
            Enum.TryParse<BodyType>(bodyTypeСontent, out var selectedBodyType);
            BodyType bodyType = selectedBodyType;

            string color = TBoxColor.Text.Trim();
            string releaseYear = TBoxReleaseYear.Text.Trim();

            string statusСontent = (CBoxStatus.SelectedItem as ComboBoxItem).Content as string;
            Enum.TryParse<TransportStatus>(statusСontent, out var selectedStatus);
            TransportStatus status = selectedStatus;

            string govNumber = TBoxGovNumber.Text.Trim();

            if (string.IsNullOrEmpty(vin))
            {
                MessageBox.Show("Введите VIN номер!", "Ошибка");

                return;
            }

            if (string.IsNullOrEmpty(model))
            {
                MessageBox.Show("Введите модель!", "Ошибка");

                return;
            }

            if (string.IsNullOrEmpty(color))
            {
                MessageBox.Show("Введите цвет!", "Ошибка");

                return;
            }

            if (string.IsNullOrEmpty(releaseYear))
            {
                MessageBox.Show("Введите год выпуска!", "Ошибка");

                return;
            }


            if (string.IsNullOrEmpty(govNumber))
            {
                MessageBox.Show("Получите номер!", "Ошибка");

                return;
            }

            if (_transport != null)
            {
                _transport.VIN = vin;
                _transport.Model = model;
                _transport.BodyType = bodyType;
                _transport.Color = color;
                _transport.ReleaseYear = int.Parse(releaseYear);
                _transport.Status = status;
                _transport.GovNumber = govNumber;

                await ((App)Application.Current).TransportService.UpdateTransport(_transport);
            }
            else
            {
                NewTransportId = await ((App)Application.Current).TransportService.AddTransport(new AddTransportDto()
                {
                    VIN = vin,
                    Model = model,
                    BodyType = bodyType,
                    Color = color,
                    ReleaseYear = int.Parse(releaseYear),
                    Status = status,
                    GovNumber = govNumber
                });
            }

            IsFinished = true;

            Close();
        }
    }

}