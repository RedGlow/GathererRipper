using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    /// <summary>
    /// Rarity of cards.
    /// </summary>
    public enum Rarity
    {
        /// <summary>
        /// Is a basic land.
        /// </summary>
        BasicLand,

        /// <summary>
        /// Common rarity.
        /// </summary>
        Common,

        /// <summary>
        /// Uncommon rarity.
        /// </summary>
        Uncommon,

        /// <summary>
        /// Rare rarity.
        /// </summary>
        Rare,

        /// <summary>
        /// Mythic rare rarity.
        /// </summary>
        MythicRare,

        /// <summary>
        /// Special (promo cards, planes, ...).
        /// </summary>
        Special,

        /// <summary>
        /// No rarity has been found.
        /// </summary>
        Unset
    }
}
