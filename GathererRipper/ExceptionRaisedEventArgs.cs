using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GathererRipper
{
    public class ExceptionRaisedEventArgs: EventArgs
    {
        public readonly AggregateException Exception;

        public ExceptionRaisedEventArgs(AggregateException exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            Exception = exception;
        }
    }
}
