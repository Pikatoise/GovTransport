using GovAuthSDK;
using GovTransportSDK;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Windows;

namespace GovTransportApp
{
    public partial class App: Application
    {
        public GovAuthService? AuthService { get; set; }
        public GovTransportService? TransportService { get; set; }

        public ILoggerFactory LoggerFactory { get; private set; }

        public void InitServices()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("./logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });

            if (AuthService == null)
                AuthService = new GovAuthService(LoggerFactory.CreateLogger<GovAuthService>());

            if (TransportService == null)
                TransportService = new GovTransportService(AuthService, LoggerFactory.CreateLogger<GovTransportService>());
        }
    }

}
