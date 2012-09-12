using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    public class HtmlDownloadSucceededEventArgs: EventArgs
    {
        public readonly string Url;

        public HtmlDownloadSucceededEventArgs(string url)
        {
            Url = url;
        }
    }
}
