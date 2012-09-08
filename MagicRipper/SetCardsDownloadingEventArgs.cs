using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    public class SetCardsDownloadingEventArgs: EventArgs
    {
        /// <summary>
        /// The declared number of cards, not considering translations).
        /// It may be slightly off, for reasons known only to the Gatherer
        /// programmers.
        /// </summary>
        public readonly int NumCards;

        /// <summary>
        /// The expansion this event is referring to.
        /// </summary>
        public readonly Set Set;

        public SetCardsDownloadingEventArgs(Set set,
            int numCards)
        {
            Set = set;
            NumCards = numCards;
        }
    }
}
