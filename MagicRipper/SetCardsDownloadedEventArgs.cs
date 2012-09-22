using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    /// <summary>
    /// Provides data for the <see cref="Ripper.ExpansionCardsDownloaded"/> event.
    /// </summary>
    public class SetCardsDownloadedEventArgs: EventArgs
    {
        /// <summary>
        /// The number of cards, not considering translations. It's the
        /// number of cards effectively downloaded.
        /// </summary>
        public readonly int NumCards;

        /// <summary>
        /// The expansion this event is referring to.
        /// </summary>
        public readonly Set Set;

        /// <summary>
        /// Initializes a new instance of <c>SetCardsDownloadedEventArgs</c> class.
        /// </summary>
        /// <param name="set">The number of cards, not considering translations. It's the
        /// number of cards effectively downloaded.</param>
        /// <param name="numCards">The expansion this event is referring to.</param>
        public SetCardsDownloadedEventArgs(Set set,
            int numCards)
        {
            Set = set;
            NumCards = numCards;
        }
    }
}
