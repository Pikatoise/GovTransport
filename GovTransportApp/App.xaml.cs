using GovAuthSDK;
using GovTransportSDK;
using System.Windows;

namespace GovTransportApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App: Application
    {
        public GovAuthService? AuthService { get; set; }
        public GovTransportService? TransportService { get; set; }

        public void InitServices()
        {
            if (AuthService == null)
                AuthService = new GovAuthService();

            if (TransportService == null)
                TransportService = new GovTransportService();
        }
    }

}
