using GovTransportSDK.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace GovTransportApp.Dialogs
{
    public partial class SelectTransportDialog: Window
    {
        private DispatcherTimer _searchTimer;
        private const int SearchDelayMs = 300;
        public Transport? SelectedTransport;

        public SelectTransportDialog()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _searchTimer = new DispatcherTimer();
            _searchTimer.Interval = TimeSpan.FromMilliseconds(SearchDelayMs);
            _searchTimer.Tick += SearchTimer_Tick;

            LBoxTransports.ItemsSource = null;
            LBoxTransports.Items.Clear();

            var transports = await ((App)Application.Current).TransportService!.AllTransports();

            if (transports.Count() == 0)
            {
                TBlockEmptyTransports.Visibility = Visibility.Visible;
                return;
            }

            LoadTransportsToListBox(transports);
        }

        private async void SearchTimer_Tick(object? sender, EventArgs e)
        {
            _searchTimer.Stop();

            string query = TBoxGovNumberSearch.Text.Trim();

            IEnumerable<Transport> transports;

            if (String.IsNullOrEmpty(query))
                transports = await ((App)Application.Current).TransportService!.AllTransports();
            else
                transports = await ((App)Application.Current).TransportService!.FindTransportsByGovNumber(query);

            if (transports.Any())
                TBlockEmptyTransports.Visibility = Visibility.Collapsed;
            else
                TBlockEmptyTransports.Visibility = Visibility.Visible;

            LBoxTransports.ItemsSource = null;
            LBoxTransports.Items.Clear();

            LoadTransportsToListBox(transports);
        }

        private void TBoxGovNumberSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void ButtonSelectTransport_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = LBoxTransports.SelectedItem as ListBoxItem;

            if (selectedItem != null)
            {
                SelectedTransport = (selectedItem.Tag as Transport)!;

                Close();
            }
            else
            {
                MessageBox.Show("Выберите транспорт!", "Ошибка");

                return;
            }
        }

        private void LoadTransportsToListBox(IEnumerable<Transport> transports)
        {
            foreach (var transport in transports)
            {
                ListBoxItem lbitem = new ListBoxItem() { Tag = transport };

                Canvas canvas = new Canvas() { Height = 50 };

                TextBlock tbGovNumber = new TextBlock()
                {
                    Text = transport.GovNumber,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Arial"),
                    Width = 120
                };

                TextBlock tbModel = new TextBlock()
                {
                    Text = transport.Model,
                    FontSize = 16,
                    FontFamily = new FontFamily("Arial")
                };

                Canvas.SetTop(tbGovNumber, 5);
                Canvas.SetTop(tbModel, 35);

                canvas.Children.Add(tbGovNumber);
                canvas.Children.Add(tbModel);

                lbitem.Content = canvas;

                LBoxTransports.Items.Add(lbitem);
            }
        }
    }
}
