using GovTransportApp.Dialogs;
using GovTransportSDK.DTO;
using GovTransportSDK.Enums;
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

        private async void LBoxOwners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = LBoxOwners.SelectedItem as ListBoxItem;

            if (selectedItem != null)
            {
                Ownership selectedOwner = (selectedItem.Tag as Ownership)!;

                IconOwnerType.Kind = selectedOwner.IsLegal ? PackIconKind.BriefcaseUser : PackIconKind.UserCircle;
                TBlockOwnerFullName.Text = selectedOwner.FullName;
                TBlockPassport.Text = selectedOwner.Passport;
                TBlockAddress.Text = selectedOwner.RegistrationAddress;
                TBlockOsago.Text = string.IsNullOrEmpty(selectedOwner.Osago) ? "-" : selectedOwner.Osago;

                var ownerTransportsHistory = await ((App)Application.Current).TransportService!.OwnershipTransportsHistory(selectedOwner.Id);

                LBoxOwnerTransports.ItemsSource = null;
                LBoxOwnerTransports.Items.Clear();

                if (ownerTransportsHistory.Count() != 0)
                {
                    TBlockOwnerTransportsEmpty.Visibility = Visibility.Hidden;

                    LoadOwnerTransportsToListBox(ownerTransportsHistory);
                }
                else
                    TBlockOwnerTransportsEmpty.Visibility = Visibility.Visible;

                GridOwnerInfo.Visibility = Visibility.Visible;
            }
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

            LBoxOwners.ItemsSource = null;
            LBoxOwners.Items.Clear();

            LoadOwnersToListBox(owners);
        }

        private async void ButtonAddOwner_Click(object sender, RoutedEventArgs e)
        {
            var addOwnerDialog = new AddOwnerDialog();

            addOwnerDialog.ShowDialog();

            if (addOwnerDialog.IsFinished)
            {
                LBoxOwners.ItemsSource = null;
                LBoxOwners.Items.Clear();

                TBoxPassportSearch.TextChanged -= TBoxPassportSearch_TextChanged;
                TBoxPassportSearch.Text = string.Empty;
                TBoxPassportSearch.TextChanged -= TBoxPassportSearch_TextChanged;

                var owners = await ((App)Application.Current).TransportService!.AllOwners();

                if (owners.Count() == 0)
                {
                    TBlockEmptyOwners.Visibility = Visibility.Visible;
                    return;
                }

                LoadOwnersToListBox(owners);
            }
        }

        private async void ButtonEditOwner_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = LBoxOwners.SelectedItem as ListBoxItem;

            if (selectedItem != null)
            {
                Ownership selectedOwner = (selectedItem.Tag as Ownership)!;

                var addOwnerDialog = new AddOwnerDialog(selectedOwner);

                addOwnerDialog.ShowDialog();

                if (addOwnerDialog.IsFinished)
                {
                    GridOwnerInfo.Visibility = Visibility.Hidden;

                    LBoxOwners.ItemsSource = null;
                    LBoxOwners.Items.Clear();

                    TBoxPassportSearch.TextChanged -= TBoxPassportSearch_TextChanged;
                    TBoxPassportSearch.Text = string.Empty;
                    TBoxPassportSearch.TextChanged -= TBoxPassportSearch_TextChanged;

                    var owners = await ((App)Application.Current).TransportService!.AllOwners();

                    if (owners.Count() == 0)
                    {
                        TBlockEmptyOwners.Visibility = Visibility.Visible;
                        return;
                    }

                    LoadOwnersToListBox(owners);
                }
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

        private void LoadOwnerTransportsToListBox(IEnumerable<OwnerHistoryDetailedDto> histories)
        {
            foreach (var history in histories)
            {
                ListBoxItem lbitem = new ListBoxItem();

                Canvas canvas = new Canvas() { Height = 70 };

                TextBlock tbGovNumber = new TextBlock()
                {
                    Text = history.Transport.GovNumber,
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Arial"),
                    Width = 120
                };

                TextBlock tbModel = new TextBlock()
                {
                    Text = history.Transport.Model,
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                };

                PackIcon packIcon = new PackIcon()
                {
                    Kind = PackIconKind.ContentCopy,
                    Foreground = Brushes.Black
                };

                Button btnCopy = new Button()
                {
                    Height = 35,
                    Width = 35,
                    Padding = new Thickness(0),
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    ToolTip = "Скопировать VIN",
                    Content = packIcon
                };

                string endDate = history.End != null ? history.End.Value.ToShortDateString() : "";

                TextBlock tbDate = new TextBlock()
                {
                    Text = $"{history.Start.ToShortDateString()} > {endDate}",
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                };

                btnCopy.Click += (s, e) =>
                {
                    Clipboard.SetText(history.Transport.VIN);
                };

                Canvas.SetTop(tbGovNumber, 12);
                Canvas.SetTop(tbModel, 40);
                Canvas.SetRight(btnCopy, 5);
                Canvas.SetTop(tbDate, 60);

                canvas.Children.Add(tbGovNumber);
                canvas.Children.Add(tbModel);
                canvas.Children.Add(btnCopy);
                canvas.Children.Add(tbDate);

                lbitem.Content = canvas;

                LBoxOwnerTransports.Items.Add(lbitem);
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
                    FullName = "Сидоровафцвфцв Ольга Ивановна",
                    IsLegal = true,
                    Osago = "84726872393",
                    Passport = "4211 407441",
                    RegistrationAddress = "Г.Орск Пр. Мира 1, Кв. 4"
                }
            };

            var histories = new List<OwnerHistoryDetailedDto>()
            {
                new OwnerHistoryDetailedDto()
                {
                    Transport = new Transport()
                    {
                        VIN = "4DRBWAFN06A207518",
                        Model = "Toyota Camry",
                        ReleaseYear = 2000,
                        Color = "White",
                        GovNumber = "А101МР56",
                        Status = TransportStatus.Ok,
                        BodyType = BodyType.Sedan
                    },
                    Start = new DateTime(2020, 3, 13),
                    End = new DateTime(2022, 5, 1)
                },
                new OwnerHistoryDetailedDto()
                {
                    Transport = new Transport()
                    {
                        VIN = "JT2BF22K6Y0283641",
                        Model = "Hyundai Tucson",
                        ReleaseYear = 2019,
                        Color = "Black",
                        GovNumber = "М536МР56",
                        Status = TransportStatus.Ok,
                        BodyType = BodyType.Sedan
                    },
                    Start = new DateTime(2022, 6, 24)
                }
            };

            LoadOwnersToListBox(owners);
            LoadOwnerTransportsToListBox(histories);
        }
    }
}
