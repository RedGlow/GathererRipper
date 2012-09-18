using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MagicRipper
{
    /// <summary>
    /// Provide data for the <see cref="Ripper.CardDownloading"/> event.
    /// </summary>
    public class CardDownloadingEventArgs: BaseCardDownloadingEventArgs
    {
        /// <summary>
        /// The multiverse id of this card.
        /// </summary>
        public readonly int MultiverseId;

        /// <summary>
        /// The language of this card.
        /// </summary>
        public readonly Language Language;

        /// <summary>
        /// Creates a new CardDownloadingEventArgs.
        /// </summary>
        /// <param name="multiverseId">The multiverse id of this card.</param>
        /// <param name="baseMultiverseId">The multiverse ID of the card in its base version (oracle).</param>
        /// <param name="part">Name of the part of this card (for multi-part, flip and split cards).</param>
        /// <param name="language">The language of this card.</param>
        public CardDownloadingEventArgs(int multiverseId,
            int baseMultiverseId, string part, Language language)
            : base(baseMultiverseId, part)
        {
            MultiverseId = multiverseId;
            Language = language;
        }
    }
}
