using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    /// <summary>
    /// <para>
    /// A single card.
    /// </para>
    /// <para>
    /// The unique identifier of a card is the tuple &lt;BaseMultiverseId, Part, Variant, Language&gt;.
    /// </para>
    /// <para>
    /// Mana costs are expressed with a simple markup: every mana symbol is surrounded by curly braces,
    /// and the supported mana symbols are the normal mana symbols (W, R, G, B, U), the hybrid mana
    /// symbols (WR, GU, BR, ..., and 2W, 2R, 2G, 2B, 2U), the phyrexian mana symbols (WP, RP, GP, BP, UP),
    /// and the colorless mana symbols (0, 1, 2, 3, ...).
    /// </para>
    /// <para>
    /// The text markup is plain text, except for the tags {-I} and {-/I} to indicate the beginning
    /// and end of italicized text.
    /// </para>
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The multiverse ID of the card.
        /// </summary>
        public int MultiverseId;

        /// <summary>
        /// The multiverse ID of the base version of the card (oracle).
        /// </summary>
        public int BaseMultiverseId;

        /// <summary>
        /// The name of the part (different from the name for multi-part, split and flip cards).
        /// </summary>
        public string Part;

        /// <summary>
        /// The name of the card.
        /// </summary>
        public string Name;

        /// <summary>
        /// The mana cost of the card.
        /// </summary>
        public string ManaCost;

        /// <summary>
        /// The converted mana cost.
        /// </summary>
        public int ConvertedManaCost;

        /// <summary>
        /// A list of all the types of this card.
        /// </summary>
        public ICollection<string> Types;

        /// <summary>
        /// A list of all the subtypes of this card.
        /// </summary>
        public ICollection<string> Subtypes;

        private string text = string.Empty;

        /// <summary>
        /// The text of this card.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                if (value == null)
                    value = string.Empty;
                text = value;
            }
        }

        private string flavorText = string.Empty;

        /// <summary>
        /// The flavor text of this card.
        /// </summary>
        public string FlavorText
        {
            get { return flavorText; }
            set
            {
                if (value == null)
                    value = string.Empty;
                flavorText = value;
            }
        }

        /// <summary>
        /// The power of this card.
        /// </summary>
        public string Power;

        /// <summary>
        /// The toughness of this card.
        /// </summary>
        public string Toughness;

        /// <summary>
        /// The set this card belongs to.
        /// </summary>
        public Set Set;

        /// <summary>
        /// The rarity of this card.
        /// </summary>
        public Rarity Rarity;

        /// <summary>
        /// The collector number of this card.
        /// </summary>
        public int? Number;

        /// <summary>
        /// The variant of this card.
        /// </summary>
        public char? Variant;

        /// <summary>
        /// The artist of this card.
        /// </summary>
        public string Artist;

        /// <summary>
        /// The language of this card.
        /// </summary>
        public Language Language;
    }
}
