using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    /// <summary>
    /// Multipart cards are seriously fucked: printed versions are wrong
    /// and there are no translations. We throw this exception when a language
    /// is requested that cannot be satisfied.
    /// </summary>
    [Serializable]
    internal class MultipartLanguageCompatibilityException : Exception
    {
        private const string errorMessage = "Cannot download a multi-part card in languages different from Oracle.";

        /// <summary>
        /// Instantiates a new instance of the class <see cref="MultipartLanguageCompatibilityException"/>.
        /// </summary>
        public MultipartLanguageCompatibilityException() : base(errorMessage) { }

        /// <summary>
        /// Instantiates a new instance of the class <see cref="MultipartLanguageCompatibilityException"/>
        /// with the specified exception.
        /// </summary>
        /// <param name="inner">A reference to the inner exception that is the cause of this exception.</param>
        public MultipartLanguageCompatibilityException(Exception inner) : base(errorMessage, inner) { }

        /// <summary>
        /// Instantiates a new instance of the class <see cref="MultipartLanguageCompatibilityException"/>
        /// with the given <see cref="System.Runtime.Serialization.SerializationInfo.SerializationInfo"/> and
        /// <see cref="System.Runtime.Serialization.StreamingContext"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MultipartLanguageCompatibilityException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
