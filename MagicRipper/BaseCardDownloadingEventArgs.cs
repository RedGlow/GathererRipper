using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MagicRipper
{
    /// <summary>
    /// Provides some basic data for the events regarding a card.
    /// </summary>
    public class BaseCardDownloadingEventArgs: CancelEventArgs
    {
        /// <summary>
        /// The multiverse ID of the card in its base version (oracle).
        /// </summary>
        public readonly int BaseMultiverseId;

        /// <summary>
        /// Name of the part of this card (for multi-part, flip and split cards).
        /// </summary>
        public readonly string Part;

        /// <summary>
        /// Initializes a new instance of <see cref="BaseCardDownloadingEventArgs"/> class.
        /// </summary>
        /// <param name="baseMultiverseId">The multiverse ID of the card in its base version (oracle).</param>
        /// <param name="part">Name of the part of this card (for multi-part, flip and split cards).</param>
        public BaseCardDownloadingEventArgs(int baseMultiverseId, string part)
        {
            BaseMultiverseId = baseMultiverseId;
            Part = part;
        }
    }
}
