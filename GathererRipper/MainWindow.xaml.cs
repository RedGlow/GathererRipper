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

            var ripper = new MagicRipper.Ripper();
            var names = new List<string>();
            var cards = ripper.GetCards(new MagicRipper.Expansion("Alara Reborn"),
                printNumCards);
            foreach (var card in cards)
            {
                System.Diagnostics.Debug.WriteLine(card.Name);
                names.Add(card.Name);
            }
            textBlock.Text = string.Join(", ", names);
        }

        private void printNumCards(int numCards)
        {
            System.Diagnostics.Debug.WriteLine("Number of cards: {0}",
                numCards);
        }
    }
}
