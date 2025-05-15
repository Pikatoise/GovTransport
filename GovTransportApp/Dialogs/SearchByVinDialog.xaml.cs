using System.Windows;

namespace GovTransportApp.Dialogs
{
    public partial class SearchByVinDialog: Window
    {
        public SearchByVinDialog()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string vin = TBoxVin.Text.Trim();

            if (string.IsNullOrEmpty(vin))
            {
                MessageBox.Show("Введите VIN!", "Ошибка");

                return;
            }

            var transportInfo = await ((App)Application.Current).TransportService.InfoByVin(vin);

            if (transportInfo == null)
            {
                MessageBox.Show("Транспорт с данным VIN номером не найден!");

                return;
            }

            var infoByVinDialog = new InfoByVinDialog(transportInfo);

            infoByVinDialog.ShowDialog();
        }
    }
}
