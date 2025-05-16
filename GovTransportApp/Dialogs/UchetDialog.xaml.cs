using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Media;

namespace GovTransportApp.Dialogs
{
    public partial class UchetDialog: Window
    {
        private Guid? _ownerId;
        private Guid? _transportId;

        public UchetDialog()
        {
            InitializeComponent();
        }

        private void ButtonAddOwner_Click(object sender, RoutedEventArgs e)
        {
            var addOwnerDialog = new AddOwnerDialog();

            addOwnerDialog.ShowDialog();

            if (addOwnerDialog.IsFinished)
            {
                _ownerId = addOwnerDialog.NewOwnershipId;

                IconOwner.Kind = PackIconKind.CheckboxMarkedCircleOutline;
                IconOwner.Foreground = Brushes.Green;

                GridOwner.IsEnabled = false;
            }
        }

        private void ButtonAddTransport_Click(object sender, RoutedEventArgs e)
        {
            var addTransportDialog = new AddTransportDialog();

            addTransportDialog.ShowDialog();

            if (addTransportDialog.IsFinished)
            {
                _transportId = addTransportDialog.NewTransportId;

                IconTransport.Kind = PackIconKind.CheckboxMarkedCircleOutline;
                IconTransport.Foreground = Brushes.Green;

                GridTransport.IsEnabled = false;
            }
        }

        private void ButtonSelectOwner_Click(object sender, RoutedEventArgs e)
        {
            var selectOwnerDialog = new SelectOwnerDialog();

            selectOwnerDialog.ShowDialog();

            if (selectOwnerDialog.SelectedOwner != null)
            {
                _ownerId = selectOwnerDialog.SelectedOwner.Id;

                IconOwner.Kind = PackIconKind.CheckboxMarkedCircleOutline;
                IconOwner.Foreground = Brushes.Green;

                GridOwner.IsEnabled = false;
            }
        }

        private void ButtonSelectTransport_Click(object sender, RoutedEventArgs e)
        {
            var selectTransportDialog = new SelectTransportDialog();

            selectTransportDialog.ShowDialog();

            if (selectTransportDialog.SelectedTransport != null)
            {
                _transportId = selectTransportDialog.SelectedTransport.Id;

                IconTransport.Kind = PackIconKind.CheckboxMarkedCircleOutline;
                IconTransport.Foreground = Brushes.Green;

                GridTransport.IsEnabled = false;
            }
        }

        private async void ButtonSaveUchet_Click(object sender, RoutedEventArgs e)
        {
            if (_ownerId == null)
            {
                MessageBox.Show("Укажите владельца!", "Ошибка");

                return;
            }

            if (_transportId == null)
            {
                MessageBox.Show("Укажите транспорт!", "Ошибка");

                return;
            }

            await ((App)Application.Current).TransportService.TransportOwnerRegistration((Guid)_ownerId, (Guid)_transportId);

            MessageBox.Show("Успешно");

            Close();
        }
    }
}
