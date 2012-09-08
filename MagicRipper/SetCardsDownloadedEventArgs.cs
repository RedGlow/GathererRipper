using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
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

        public SetCardsDownloadedEventArgs(Set set,
            int numCards)
        {
            Set = set;
            NumCards = numCards;
        }
    }
}
