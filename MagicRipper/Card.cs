using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    public class Card
    {
        public int MultiverseId;
        public int BaseMultiverseId;
        public string Part;
        public string Name;
        public string ManaCost;
        public int ConvertedManaCost;
        public ICollection<string> Types;
        public ICollection<string> Subtypes;

        private string text = string.Empty;

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

        public string Power;
        public string Toughness;
        public Set Set;
        public Rarity Rarity;
        public int? Number;
        public char? Variant;
        public string Artist;
        public Language Language;
    }
}
