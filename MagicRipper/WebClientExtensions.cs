using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace MagicRipper
{
    static class WebClientExtensions
    {
        public static string DownloadString(this WebClient webClient,
            string address, Encoding encoding)
        {
            var bytes = webClient.DownloadData(address);
            return encoding.GetString(bytes);
        }
    }
}
