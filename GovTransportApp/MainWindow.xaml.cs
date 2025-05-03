using GovAuthSDK;
using System.Windows;

namespace GovTransportApp
{
    public partial class MainWindow: Window
    {
        GovAuthService service;

        public MainWindow()
        {
            service = new GovAuthService();

            InitializeComponent();
        }

        private async void ButtonLogIn_Click(object sender, RoutedEventArgs e)
        {
            var user = await service.LoginAuth(TBoxLogin.Text, TBoxPassword.Text);

            var users = await service.AllUsers();

            MessageBox.Show($"Succesfully login by {user.Login}\nUsers count: {users.Count()}");
        }
    }
}