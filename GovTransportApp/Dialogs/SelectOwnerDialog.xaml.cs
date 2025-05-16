using GovTransportSDK.Models;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace GovTransportApp.Dialogs
{
    public partial class SelectOwnerDialog: Window
    {
        private DispatcherTimer _searchTimer;
        private const int SearchDelayMs = 300;
        public Ownership? SelectedOwner;

        public SelectOwnerDialog()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _searchTimer = new DispatcherTimer();
            _searchTimer.Interval = TimeSpan.FromMilliseconds(SearchDelayMs);
            _searchTimer.Tick += SearchTimer_Tick;

            LBoxOwners.ItemsSource = null;
            LBoxOwners.Items.Clear();

            var owners = await ((App)Application.Current).TransportService!.AllOwners();

            if (owners.Count() == 0)
            {
                TBlockEmptyOwners.Visibility = Visibility.Visible;
                return;
            }

            LoadOwnersToListBox(owners);
        }

        private async void SearchTimer_Tick(object? sender, EventArgs e)
        {
            _searchTimer.Stop();

            string query = TBoxPassportSearch.Text.Trim();

            IEnumerable<Ownership> owners;

            if (String.IsNullOrEmpty(query))
                owners = await ((App)Application.Current).TransportService!.AllOwners();
            else
                owners = await ((App)Application.Current).TransportService!.FindOwnersByPassport(query);

            if (owners.Any())
                TBlockEmptyOwners.Visibility = Visibility.Collapsed;
            else
                TBlockEmptyOwners.Visibility = Visibility.Visible;

            LBoxOwners.ItemsSource = null;
            LBoxOwners.Items.Clear();

            LoadOwnersToListBox(owners);
        }

        private void TBoxPassportSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void ButtonSelectOwner_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = LBoxOwners.SelectedItem as ListBoxItem;

            if (selectedItem != null)
            {
                SelectedOwner = (selectedItem.Tag as Ownership)!;

                Close();
            }
            else
            {
                MessageBox.Show("Выберите владельца!", "Ошибка");

                return;
            }
        }

        private void LoadOwnersToListBox(IEnumerable<Ownership> owners)
        {
            foreach (var owner in owners)
            {
                ListBoxItem lbitem = new ListBoxItem() { Tag = owner };

                Canvas canvas = new Canvas() { Height = 60 };

                PackIcon icon = new PackIcon()
                {
                    Kind = owner.IsLegal ? PackIconKind.BriefcaseUser : PackIconKind.UserCircle,
                    Width = 40,
                    Height = 40
                };

                TextBlock tbPassport = new TextBlock()
                {
                    Text = owner.Passport,
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Arial"),
                    Width = 120
                };

                TextBlock tbFullName = new TextBlock()
                {
                    Width = 150,
                    Text = owner.FullName,
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Arial")
                };

                Canvas.SetLeft(tbPassport, 45);
                Canvas.SetTop(tbPassport, 12);
                Canvas.SetTop(tbFullName, 40);

                canvas.Children.Add(icon);
                canvas.Children.Add(tbPassport);
                canvas.Children.Add(tbFullName);

                lbitem.Content = canvas;

                LBoxOwners.Items.Add(lbitem);
            }
        }
    }
}
