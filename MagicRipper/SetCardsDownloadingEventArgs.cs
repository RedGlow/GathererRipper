using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    /// <summary>
    /// Provides data for the <c>Ripper.ExpansionCardsDownloading</c> event.
    /// </summary>
    public class SetCardsDownloadingEventArgs: EventArgs
    {
        /// <summary>
        /// The declared number of cards, not considering translations.
        /// </summary>
        public readonly int NumCards;

        /// <summary>
        /// The expansion this event is referring to.
        /// </summary>
        public readonly Set Set;

        /// <summary>
        /// Create a new SetCardsDownloadingEventArgs.
        /// </summary>
        /// <param name="set">The expansion this event is referring to.</param>
        /// <param name="numCards">The declared number of cards, not considering translations.</param>
        public SetCardsDownloadingEventArgs(Set set,
            int numCards)
        {
            Set = set;
            NumCards = numCards;
        }
    }
}
