using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace GathererRipper
{
    static class DbHelper
    {
        private static void addParameter(this DbCommand command, object value)
        {
            var parameter = command.CreateParameter();
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        internal static void setParameters(this DbCommand command,
            params object[] values)
        {
            command.Parameters.Clear();
            foreach (var value in values)
                command.addParameter(value);
        }
    }
}
