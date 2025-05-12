using System.Windows;

namespace GovTransportApp.Dialogs
{
    public partial class NewTokenDialog: Window
    {
        private string _generatedToken;

        public NewTokenDialog(string generatedToken)
        {
            InitializeComponent();

            _generatedToken = generatedToken;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TBoxGeneratedToken.Text = _generatedToken;
        }

        private void ButtonCopyToken_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_generatedToken);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
