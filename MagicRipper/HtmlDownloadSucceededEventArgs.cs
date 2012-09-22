using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    /// <summary>
    /// Provides data for a <see cref="Ripper.HtmlDownloadSucceeded"/> event.
    /// </summary>
    public class HtmlDownloadSucceededEventArgs: EventArgs
    {
        /// <summary>
        /// The URL that was successfully downloaded.
        /// </summary>
        public readonly string Url;

        /// <summary>
        /// Instantiates a new instance of the <see cref="HtmlDownloadSucceededEventArgs"/> class.
        /// </summary>
        /// <param name="url">The URL that was successfully downloaded.</param>
        public HtmlDownloadSucceededEventArgs(string url)
        {
            Url = url;
        }
    }
}
