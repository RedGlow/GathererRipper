using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    /// <summary>
    /// Provides data for an HTML download error.
    /// </summary>
    public class HtmlDownloadErrorEventArgs: EventArgs
    {
        /// <summary>
        /// The URL of the page which failed to download.
        /// </summary>
        public readonly string Url;

        /// <summary>
        /// The exception raised while trying to download the page.
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        /// Gets or sets a value indicating whether the error should be ignored,
        /// and the operation retried.
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Create a new HtmlDownloadErrorEventArgs.
        /// </summary>
        /// <param name="url">The URL of the page which failed to download.</param>
        /// <param name="exception">The exception raised while trying to download the page.</param>
        public HtmlDownloadErrorEventArgs(string url, Exception exception)
        {
            Url = url;
            Exception = exception;
            Ignore = false;
        }
    }
}
