using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    public struct Card
    {
        public string Name;
        public string ManaCost;
        public int ConvertedManaCost;
        public ICollection<string> Types;
        public ICollection<string> Subtypes;
        public string Text;
        public string FlavorText;
        public string Power;
        public string Toughness;
        public Expansion Expansion;
        public Rarity Rarity;
        public int Number;
        public string Artist;
        public Language Language;
    }
}
