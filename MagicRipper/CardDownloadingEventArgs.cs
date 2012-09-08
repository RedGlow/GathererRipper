using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MagicRipper
{
    public class CardDownloadingEventArgs: BaseCardDownloadingEventArgs
    {
        public readonly int MultiverseId;

        public readonly Language Language;

        public CardDownloadingEventArgs(int multiverseId,
            int baseMultiverseId, string part, Language language)
            : base(baseMultiverseId, part)
        {
            MultiverseId = multiverseId;
            Language = language;
        }
    }
}
