using System.Windows;

namespace GovTransportApp.Dialogs
{
    public partial class AddTokenDialog: Window
    {
        public bool IsFinished = false;

        public AddTokenDialog()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string description = TBoxDescription.Text.Trim();
            DateTime endDate = DPickerEnd.DisplayDate;

            if (string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Введите описание", "Ошибка");

                return;
            }

            if (endDate == DateTime.Today || endDate < DateTime.Today)
            {
                MessageBox.Show("Неверная дата", "Ошибка");

                return;
            }

            if (endDate > DateTime.Today.AddYears(1).AddDays(1))
            {
                MessageBox.Show("Слишком большой срок! Максимальная длина 1 год.", "Ошибка");

                return;
            }

            var newToken = await ((App)Application.Current).AuthService!.AddToken(description, endDate);

            var newTokenDialog = new NewTokenDialog(newToken);
            newTokenDialog.ShowDialog();

            IsFinished = true;

            Close();
        }
    }
}
