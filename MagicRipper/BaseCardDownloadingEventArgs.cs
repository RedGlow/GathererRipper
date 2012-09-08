using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MagicRipper
{
    public class BaseCardDownloadingEventArgs: CancelEventArgs
    {
        public readonly int BaseMultiverseId;
        public readonly string Part;

        public BaseCardDownloadingEventArgs(int baseMultiverseId, string part)
        {
            BaseMultiverseId = baseMultiverseId;
            Part = part;
        }
    }
}
