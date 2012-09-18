using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicRipper
{
    /// <summary>
    /// An expansion set.
    /// </summary>
    public class Set
    {
        /// <summary>
        /// Name of the set.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Create a new <c>Set</c> object.
        /// </summary>
        /// <param name="name">Name of the set.</param>
        public Set(string name)
        {
            Name = name;
        }
    }
}
