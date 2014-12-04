using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CDCatalogWindowsDesktopGUI
{
    public class BoolToZIndexConverter : IValueConverter
    {
        static BoolToZIndexConverter()
        {
            defaultZIndex = 2;
            targetZIndex = 10;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? TargetZIndex : DefaultZIndex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int target = (targetZIndex - defaultZIndex) / 2;
            Nullable<int> x = value as Nullable<int>;
            return x == null || (int)x <= target ? DefaultZIndex : TargetZIndex;
        }

        public int TargetZIndex
        {
            get { return targetZIndex++; }
            set { targetZIndex = value; }
        }

        public int DefaultZIndex
        {
            get { return defaultZIndex; }
            set { defaultZIndex = value; }
        }
        private static int targetZIndex;
        private static int defaultZIndex;
    }
}
