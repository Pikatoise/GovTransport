using GovTransportSDK.Models;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace GovTransportApp.Pages
{
    public partial class OwnersPage: UserControl
    {
        private DispatcherTimer _searchTimer;
        private const int SearchDelayMs = 300;

        public OwnersPage()
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

            LBoxOwners.Items.Clear();

            var owners = await ((App)Application.Current).TransportService!.AllOwners();

            if (owners.Count() == 0)
            {
                TBlockEmptyOwners.Visibility = Visibility.Visible;
                return;
            }

            LoadOwnersToListBox(owners);
        }

        private void LBoxOwners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TBoxPassportSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
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

            LBoxOwners.Items.Clear();

            LoadOwnersToListBox(owners);
        }

        private void LoadOwnersToListBox(IEnumerable<Ownership> owners)
        {
            foreach (var owner in owners)
            {
                ListBoxItem lbitem = new ListBoxItem() { Tag = owner };

                Canvas canvas = new Canvas() { Height = 50 };

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
                    Text = owner.FullName,
                    FontSize = 12,
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

        private void LoadDesignData()
        {
            var owners = new List<Ownership>()
            {
                new Ownership()
                {
                    FullName = "Петров Петр Петрович",
                    IsLegal = false,
                    Osago = "7189671298",
                    Passport = "6433 629663",
                    RegistrationAddress = "Г.Орск Ул.Пушкина 4, Кв. 1"
                },
                new Ownership()
                {
                    FullName = "Иванов Иван Иванович",
                    IsLegal = false,
                    Osago = "92874823316",
                    Passport = "7544 730774",
                    RegistrationAddress = "Г.Орск Ул.Колотушкина 5, Кв. 20"
                },
                new Ownership()
                {
                    FullName = "Сидорова Ольга Ивановна",
                    IsLegal = true,
                    Osago = "84726872393",
                    Passport = "4211 407441",
                    RegistrationAddress = "Г.Орск Пр. Мира 1, Кв. 4"
                }
            };

            LoadOwnersToListBox(owners);
        }
    }
}
