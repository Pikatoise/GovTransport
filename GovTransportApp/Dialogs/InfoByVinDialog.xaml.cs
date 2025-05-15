using GovTransportSDK.DTO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GovTransportApp.Dialogs
{
    public partial class InfoByVinDialog: Window
    {
        VinInfoDto vinInfo;

        public InfoByVinDialog(VinInfoDto transportInfo)
        {
            InitializeComponent();

            vinInfo = transportInfo;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TBlockTransportGovNumber.Text = vinInfo.Transport.GovNumber;
            TBlockVin.Text = vinInfo.Transport.VIN;
            TBlockModel.Text = vinInfo.Transport.Model;
            TBlockBodyType.Text = vinInfo.Transport.BodyType.ToString();
            TBlockColor.Text = vinInfo.Transport.Color;
            TBlockReleaseYear.Text = vinInfo.Transport.ReleaseYear.ToString();
            TBlockStatus.Text = vinInfo.Transport.Status.ToString();

            if (vinInfo.History.Count() > 0)
            {
                TBlockTransportOwnersEmpty.Visibility = Visibility.Hidden;

                LoadTransportOwnersToListBox(vinInfo.History);
            }
            else
            {
                TBlockTransportOwnersEmpty.Visibility = Visibility.Visible;
            }
        }

        private void LoadTransportOwnersToListBox(IEnumerable<OwnerHistoryMinimizedDto> histories)
        {
            foreach (var history in histories)
            {
                ListBoxItem lbitem = new ListBoxItem()
                {
                    BorderBrush = (Brush)new BrushConverter().ConvertFromString("#3f51b5"),
                    BorderThickness = new Thickness(0, 0, 0, 2)
                };

                Canvas canvas = new Canvas() { Height = 75 };


                TextBlock tbIsLegal = new TextBlock()
                {
                    Text = history.IsLegal ? "Юр. лицо" : "Физ. лицо",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Arial"),
                    Width = 100
                };

                string endDate = history.End != null ? history.End.Value.ToShortDateString() : string.Empty;

                TextBlock tbDate = new TextBlock()
                {
                    Text = $"{history.Start} > {endDate}",
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                };



                Canvas.SetTop(tbIsLegal, 12);

                Canvas.SetTop(tbDate, 40);

                canvas.Children.Add(tbIsLegal);
                canvas.Children.Add(tbDate);

                lbitem.Content = canvas;

                LBoxOwnersHistory.Items.Add(lbitem);
            }
        }
    }
}
