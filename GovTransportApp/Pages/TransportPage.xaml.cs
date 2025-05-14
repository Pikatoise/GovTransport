using GovTransportApp.Dialogs;
using GovTransportSDK.Enums;
using GovTransportSDK.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace GovTransportApp.Pages
{
    public partial class TransportPage: UserControl
    {
        private DispatcherTimer _searchTimer;
        private const int SearchDelayMs = 300;

        public TransportPage()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                LoadDesignData();
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

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

        private async void LBoxTransports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = LBoxTransports.SelectedItem as ListBoxItem;

            if (selectedItem != null)
            {
                Transport selectedTransport = (selectedItem.Tag as Transport)!;

                TBlockTransportGovNumber.Text = selectedTransport.GovNumber;
                TBlockVin.Text = selectedTransport.VIN;
                TBlockModel.Text = selectedTransport.Model;
                TBlockBodyType.Text = selectedTransport.BodyType.ToString();
                TBlockColor.Text = selectedTransport.Color;
                TBlockReleaseYear.Text = selectedTransport.ReleaseYear.ToString();
                TBlockStatus.Text = selectedTransport.Status.ToString();

                var transportOwner = await ((App)Application.Current).TransportService!.LastOwnerByTransportId(selectedTransport.Id);

                if (transportOwner != null)
                {
                    TBlockTransportOwnerEmpty.Visibility = Visibility.Hidden;

                    TBlockTransportOwnerFullName.Text = transportOwner.Ownership.FullName;
                    TBlockTransportOwnerPassport.Text = transportOwner.Ownership.Passport;
                    string endDate = transportOwner.End != null ? transportOwner.End.Value.ToShortDateString() : "";
                    TBlockTransportOwnerOwnDate.Text = transportOwner.Start.ToShortDateString() + " > " + endDate;

                    SPanelTransportOwnerInfo.Visibility = Visibility.Visible;
                }
                else
                {
                    SPanelTransportOwnerInfo.Visibility = Visibility.Hidden;
                    TBlockTransportOwnerEmpty.Visibility = Visibility.Visible;
                }

                GridTransportInfo.Visibility = Visibility.Visible;
            }
        }

        private async void ButtonAddTransport_Click(object sender, RoutedEventArgs e)
        {
            var addTransportDialog = new AddTransportDialog();

            addTransportDialog.ShowDialog();

            if (addTransportDialog.IsFinished)
            {
                LBoxTransports.ItemsSource = null;
                LBoxTransports.Items.Clear();

                TBoxGovNumberSearch.TextChanged -= TBoxGovNumberSearch_TextChanged;
                TBoxGovNumberSearch.Text = string.Empty;
                TBoxGovNumberSearch.TextChanged -= TBoxGovNumberSearch_TextChanged;

                var transports = await ((App)Application.Current).TransportService!.AllTransports();

                if (transports.Count() == 0)
                {
                    TBlockEmptyTransports.Visibility = Visibility.Visible;
                    return;
                }

                LoadTransportsToListBox(transports);
            }
        }

        private async void ButtonEditTransport_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = LBoxTransports.SelectedItem as ListBoxItem;

            if (selectedItem != null)
            {
                Transport selectedTransport = (selectedItem.Tag as Transport)!;

                var addTransportDialog = new AddTransportDialog(selectedTransport);

                addTransportDialog.ShowDialog();

                if (addTransportDialog.IsFinished)
                {
                    GridTransportInfo.Visibility = Visibility.Hidden;

                    LBoxTransports.ItemsSource = null;
                    LBoxTransports.Items.Clear();

                    TBoxGovNumberSearch.TextChanged -= TBoxGovNumberSearch_TextChanged;
                    TBoxGovNumberSearch.Text = string.Empty;
                    TBoxGovNumberSearch.TextChanged -= TBoxGovNumberSearch_TextChanged;

                    var transports = await ((App)Application.Current).TransportService!.AllTransports();

                    if (transports.Count() == 0)
                    {
                        TBlockEmptyTransports.Visibility = Visibility.Visible;
                        return;
                    }

                    LoadTransportsToListBox(transports);
                }
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

        private void LoadDesignData()
        {
            var transports = new List<Transport>()
            {
                new Transport()
                {
                    VIN = "4DRBWAFN06A207518",
                    Model = "Toyota Camry",
                    ReleaseYear = 2000,
                    Color = "White",
                    GovNumber = "А101МР56",
                    Status = TransportStatus.Ok,
                    BodyType = BodyType.Sedan
                },
                new Transport()
                {
                    VIN = "JT2BF22K6Y0283641",
                    Model = "Hyundai Tucson",
                    ReleaseYear = 2019,
                    Color = "Black",
                    GovNumber = "М536МР56",
                    Status = TransportStatus.Ok,
                    BodyType = BodyType.Sedan
                },
            };

            LoadTransportsToListBox(transports);
        }
    }
}
