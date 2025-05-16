using GovTransportApp.Dialogs;
using System.Windows.Controls;

namespace GovTransportApp.Pages
{
    public partial class MainPage: UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ButtonMakeOwnership_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var uchetDialog = new UchetDialog();

            uchetDialog.ShowDialog();
        }

        private void ButtonVinSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var searchByVinDialog = new SearchByVinDialog();

            searchByVinDialog.ShowDialog();
        }
    }
}
