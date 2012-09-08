using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    /// <summary>
    /// multipart cards are seriously fucked: printed versions are wrong
    /// and there are no translations. We throw this exception when a language
    /// is requested that cannot be satisfied.
    /// </summary>
    [Serializable]
    public class MultipartLanguageCompatibilityException : Exception
    {
        private const string errorMessage = "Cannot download a multi-part card in languages different from Oracle.";
        public MultipartLanguageCompatibilityException() : base(errorMessage) { }
        public MultipartLanguageCompatibilityException(Exception inner) : base(errorMessage, inner) { }
        protected MultipartLanguageCompatibilityException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
