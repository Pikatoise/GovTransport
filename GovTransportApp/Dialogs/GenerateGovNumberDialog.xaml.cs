using System.Windows;

namespace GovTransportApp.Dialogs
{
    public partial class GenerateGovNumberDialog: Window
    {
        public string? NewNumber = null;
        public bool IsFinished = false;

        public GenerateGovNumberDialog()
        {
            InitializeComponent();
        }

        private async void ButtonGenerateGovNumber_Click(object sender, RoutedEventArgs e)
        {
            string region = TBoxRegion.Text.Trim();

            if (string.IsNullOrEmpty(region))
            {
                MessageBox.Show("Введите регион!", "Ошибка");

                return;
            }

            string newNumber = await ((App)Application.Current).TransportService.GenerateUniqueGovNumber(region);

            TBoxGeneratedGovNumber.Text = newNumber;

            NewNumber = newNumber;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NewNumber == null)
            {
                MessageBox.Show("Сгенерируйте номер!", "Ошибка");

                return;
            }

            IsFinished = true;

            Close();
        }
    }
}
