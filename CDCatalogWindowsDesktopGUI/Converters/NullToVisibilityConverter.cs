using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using CDCatalogModel;

namespace CDCatalogWindowsDesktopGUI
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is IAlbumOrSong) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible ? null : Binding.DoNothing;
        }
    }
}
