using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CDCatalogModel;

namespace CDCatalogWindowsDesktopGUI
{
    public class IAlbumOrSongToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null ||
                !(value is IAlbumOrSong))
            {
                return Visibility.Collapsed;
            }

            Visibility result = (value is Song) ? Visibility.Visible : Visibility.Collapsed;

            bool visibleForSong = System.Convert.ToBoolean(parameter);
            return visibleForSong ? result : toggleVisibility(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot convert from visibility to IAlbumOrSong");
        }

        private static Visibility toggleVisibility(Visibility v)
        {
            return v == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
