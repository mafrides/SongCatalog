using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using CDCatalogModel;

namespace CDCatalogWindowsDesktopGUI
{
    public class AlbumToVisibilityConverter : IValueConverter
    {
        static AlbumToVisibilityConverter()
        {
            visibleOnNullOrEmpty = false;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility v;
            if(value is Album)
            {
                Album a = value as Album;
                if (a == null || String.IsNullOrEmpty(a.Title)) v = Visibility.Collapsed;
                else v = Visibility.Visible;
            }
            else if(value is string)
            {
                string s = value as string;
                if (s == null || String.IsNullOrEmpty(s)) v = Visibility.Collapsed;
                else v = Visibility.Visible;   
            }
            else
            {
                v = Visibility.Collapsed;
            }
            if (VisibleOnNullOrEmpty) v = toggleVisibility(v);
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public bool VisibleOnNullOrEmpty
        {
            get { return visibleOnNullOrEmpty; }
            set { visibleOnNullOrEmpty = value; }
        }

        private Visibility toggleVisibility(Visibility v)
        {
            return v == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private static bool visibleOnNullOrEmpty;
    }
}
