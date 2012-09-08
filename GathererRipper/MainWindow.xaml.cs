using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GathererRipper
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            //var ripper = new MagicRipper.Ripper();
            //ripper.ExpansionCardsDownloading += new EventHandler<MagicRipper.ExpansionCardsDownloadingEventArgs>(ripper_ExpansionCardsDownloading);
            //ripper.CardDownloading += new EventHandler<MagicRipper.CardDownloadingEventArgs>(ripper_CardDownloading);
            //var names = new List<string>();
            //var cards = ripper.GetCards(new MagicRipper.Expansion("Alara Reborn"));
            //foreach (var card in cards)
            //{
            //    System.Diagnostics.Debug.WriteLine(card.Name);
            //    names.Add(card.Name);
            //}
            //textBlock.Text = string.Join(", ", names);
        }

        void ripper_CardDownloading(object sender, MagicRipper.CardDownloadingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Going to download card with id={0}",
                e.MultiverseId);
        }

        void ripper_ExpansionCardsDownloading(object sender, MagicRipper.SetCardsDownloadingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Going to download {0} cards for {1}",
                e.NumCards, e.Set.Name);
        }
    }
}
