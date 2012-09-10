using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Shell;

namespace GathererRipper
{
    [ValueConversion(typeof(bool), typeof(TaskbarItemProgressState))]
    class BooleanToProgressStateConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = (bool)value;
            return b ? TaskbarItemProgressState.Normal : TaskbarItemProgressState.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var t = (TaskbarItemProgressState)value;
            return t == TaskbarItemProgressState.Normal ? true : false;
        }
    }
}
