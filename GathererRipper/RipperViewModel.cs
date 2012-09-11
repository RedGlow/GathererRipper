using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicRipper;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System.ComponentModel;

namespace GathererRipper
{
    public class RipperViewModel: DependencyObject
    {
        public RipperViewModel()
        {
            startCommand = new StartCommand(this);
        }



        public string CurrentlyProcessedExpansionName
        {
            get { return (string)GetValue(CurrentlyProcessedExpansionNameProperty); }
            set { SetValue(CurrentlyProcessedExpansionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentlyProcessedExpansion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentlyProcessedExpansionNameProperty =
            DependencyProperty.Register("CurrentlyProcessedExpansionName", typeof(string), typeof(RipperViewModel), new UIPropertyMetadata(string.Empty));



        public int CurrentlyProcessedExpansionNumber
        {
            get { return (int)GetValue(CurrentlyProcessedExpansionNumberProperty); }
            set { SetValue(CurrentlyProcessedExpansionNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentlyProcessedExpansionNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentlyProcessedExpansionNumberProperty =
            DependencyProperty.Register("CurrentlyProcessedExpansionNumber", typeof(int), typeof(RipperViewModel), new UIPropertyMetadata(0, new PropertyChangedCallback(totalProgressChanged)));

        

        public int NumExpansions
        {
            get { return (int)GetValue(NumExpansionsProperty); }
            set { SetValue(NumExpansionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumExpansions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumExpansionsProperty =
            DependencyProperty.Register("NumExpansions", typeof(int), typeof(RipperViewModel), new UIPropertyMetadata(0, new PropertyChangedCallback(totalProgressChanged)));



        public int CurrentlyProcessedCardsNumber
        {
            get { return (int)GetValue(CurrentlyProcessedCardsNumberProperty); }
            set { SetValue(CurrentlyProcessedCardsNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for currentlyProcessedCardsNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentlyProcessedCardsNumberProperty =
            DependencyProperty.Register("CurrentlyProcessedCardsNumber", typeof(int), typeof(RipperViewModel), new UIPropertyMetadata(0, new PropertyChangedCallback(totalProgressChanged)));



        public int CurrentlyProcessedCardNumber
        {
            get { return (int)GetValue(CurrentlyProcessedCardNumberProperty); }
            set { SetValue(CurrentlyProcessedCardNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for currentlyProcessedCardNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentlyProcessedCardNumberProperty =
            DependencyProperty.Register("CurrentlyProcessedCardNumber", typeof(int), typeof(RipperViewModel), new UIPropertyMetadata(0, new PropertyChangedCallback(totalProgressChanged)));



        public double TotalProgress
        {
            get { return (double)GetValue(TotalProgressProperty); }
            set { SetValue(TotalProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalProgressProperty =
            DependencyProperty.Register("TotalProgress", typeof(double), typeof(RipperViewModel), new UIPropertyMetadata(0.0));

        private static void totalProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ripperViewModel = (RipperViewModel)d;
            var processedExpansions = ripperViewModel.CurrentlyProcessedExpansionNumber;
            var numExpansions = ripperViewModel.NumExpansions;
            var processedCards = ripperViewModel.CurrentlyProcessedCardNumber;
            var numCards = ripperViewModel.CurrentlyProcessedCardsNumber;

            double result = 0.0;
            if (numExpansions != 0)
            {
                result = (double)processedExpansions / (double)numExpansions;
                if (numCards != 0)
                {
                    result += (double)processedCards / (double)numCards / (double)numExpansions;
                }
            }

            ripperViewModel.TotalProgress = result;
        }



        public string CurrentlyProcessedCardLanguage
        {
            get { return (string)GetValue(CurrentlyProcessedCardLanguageProperty); }
            set { SetValue(CurrentlyProcessedCardLanguageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentlyProcessedCardLanguage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentlyProcessedCardLanguageProperty =
            DependencyProperty.Register("CurrentlyProcessedCardLanguage", typeof(string), typeof(RipperViewModel), new UIPropertyMetadata(string.Empty));




        public string CurrentlyProcessedCardName
        {
            get { return (string)GetValue(CurrentlyProcessedCardNameProperty); }
            set { SetValue(CurrentlyProcessedCardNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentlyProcessedCardName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentlyProcessedCardNameProperty =
            DependencyProperty.Register("CurrentlyProcessedCardName", typeof(string), typeof(RipperViewModel), new UIPropertyMetadata(string.Empty));



        public bool Running
        {
            get { return (bool)GetValue(RunningProperty); }
            set { SetValue(RunningProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Running.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RunningProperty =
            DependencyProperty.Register("Running", typeof(bool), typeof(RipperViewModel), new UIPropertyMetadata(false));



        private class StartCommand : ICommand
        {
            RipperViewModel parent;

            public StartCommand(RipperViewModel rvm)
            {
                DependencyPropertyDescriptor
                    .FromProperty(RunningProperty, typeof(RipperViewModel))
                    .AddValueChanged(rvm, new EventHandler(runningChanged));
                parent = rvm;
            }

            private void runningChanged(object source, EventArgs e)
            {
                var handler = CanExecuteChanged;
                if (handler != null)
                    handler(this, new EventArgs());
            }

            public bool CanExecute(object parameter)
            {
                return !parent.Running;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                parent.Start();
            }
        }


        private StartCommand startCommand;

        public ICommand DoStart { get { return startCommand; } }



        public void Start()
        {
            Running = true;
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
                {
                    // create a db manager
                    var dbManager = new DbManager();

                    // get the expansions
                    var ripper = new Ripper();
                    var expansions = ripper.GetExpansions().ToList();
                    runIn(() => { NumExpansions = expansions.Count; }, scheduler);

                    // update number of cards of the expansion when starting a new expansion
                    ripper.ExpansionCardsDownloading += new EventHandler<SetCardsDownloadingEventArgs>(
                        (object sender, SetCardsDownloadingEventArgs e) =>
                        {
                            runIn(() =>
                            {
                                CurrentlyProcessedCardNumber = 0;
                                CurrentlyProcessedCardsNumber = e.NumCards;
                            }, scheduler);
                            using (var transaction = dbManager.Transaction())
                            {
                                if (!dbManager.SetExists(e.Set.Name))
                                {
                                    dbManager.AddSet(e.Set.Name, e.NumCards);
                                    transaction.Commit();
                                }
                            }
                        });

                    // decide whether we should download or not a group of
                    // translations for a card
                    ripper.BaseCardDownloading += new EventHandler<BaseCardDownloadingEventArgs>(
                        (object sender, BaseCardDownloadingEventArgs e) =>
                        {
                            runIn(() =>
                            {
                                CurrentlyProcessedCardNumber++;
                            }, scheduler);
                            if (dbManager.CardExistsByBase(e.BaseMultiverseId, e.Part))
                            {
                                System.Diagnostics.Debug.WriteLine("base id {0}, part {1} exists",
                                    e.BaseMultiverseId, e.Part);

                                // update interface
                                runIn(() =>
                                {
                                    CurrentlyProcessedCardName = e.Part;
                                    CurrentlyProcessedCardLanguage = string.Empty;
                                }, scheduler);

                                e.Cancel = true;
                            }
                            else
                                System.Diagnostics.Debug.WriteLine("base id {0}, part {1} does not exist",
                                    e.BaseMultiverseId, e.Part);
                        });

                    // update number of cards and decide whether we should download the
                    // card or not
                    ripper.CardDownloading += new EventHandler<CardDownloadingEventArgs>(
                        (object sender, CardDownloadingEventArgs e) =>
                        {
                            if (dbManager.CardExists(e.BaseMultiverseId, e.Part, e.Language))
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    "id {0} in language {1} exists",
                                    e.MultiverseId, e.Language);
                                e.Cancel = true;
                            }
                            else
                                System.Diagnostics.Debug.WriteLine(
                                    "id {0} in language {1} does not exist",
                                    e.MultiverseId, e.Language);
                        });

                    // run over all the expansions
                    for (int i = 0; i < expansions.Count; i++)
                    {
                        // update interface with current expansion data
                        runIn(() =>
                        {
                            CurrentlyProcessedExpansionName = expansions[i].Name;
                            CurrentlyProcessedExpansionNumber = i;
                        }, scheduler);

                        // check if we need to download the set
                        if (dbManager.SetExists(expansions[i].Name) &&
                            dbManager.IsSetDownloaded(expansions[i].Name))
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Set {0} exists and is completely downloaded.",
                                new object[] { expansions[i].Name });
                            continue;
                        }

                        // get all cards of the expansion
                        foreach (var cards in ripper.GetCards(expansions[i]))
                        {
                            // update interface
                            runIn(() =>
                            {
                                CurrentlyProcessedCardName = string.Join(
                                    ", ",
                                    getCardNames(cards));
                                CurrentlyProcessedCardLanguage =
                                    cards.First().Language.ToString();
                            }, scheduler);
                            // update database
                            using (var transaction = dbManager.Transaction())
                            {
                                foreach(var card in cards)
                                    dbManager.AddCard(card);
                                transaction.Commit();
                            }
                        }
                    }
                }).ContinueWith(t =>
                {
                    CurrentlyProcessedCardName = string.Empty;
                    CurrentlyProcessedCardLanguage = string.Empty;
                    CurrentlyProcessedExpansionName = string.Empty;
                    Running = false;
                }, scheduler);
        }

        private static IEnumerable<string> getCardNames(ICollection<Card> cards)
        {
            return (from card in cards select card.Name);
        }

        private CancellationTokenSource cts =
            new CancellationTokenSource();

        private void runIn(Action action, TaskScheduler scheduler)
        {
            Task.Factory.StartNew(action, cts.Token, TaskCreationOptions.None, scheduler);
        }
    }
}
