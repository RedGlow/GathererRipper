using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    public class HtmlDownloadErrorEventArgs: EventArgs
    {
        public readonly string Url;
        public readonly Exception Exception;

        public bool Ignore { get; set; }

        public HtmlDownloadErrorEventArgs(string url, Exception exception)
        {
            Url = url;
            Exception = exception;
            Ignore = false;
        }
    }
}
