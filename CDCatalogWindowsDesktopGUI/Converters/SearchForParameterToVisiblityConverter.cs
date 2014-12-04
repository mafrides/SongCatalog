using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CDCatalogWindowsDesktopGUI
{
    //Collapse for Albums Only
    public class SearchForParameterToVisiblityConverter : IValueConverter
    {
        public SearchForParameterToVisiblityConverter()
        {
            VisibleForMatch = false;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility result = Visibility.Collapsed;
            if (value is SearchForParameter &&
                parameter is string)
            {
                if((SearchForParameter)value == 
                    (SearchForParameter)Enum.Parse(value.GetType(), parameter.ToString(), true))
                {
                    result = Visibility.Visible;
                }
            }
            return VisibleForMatch ? result : toggleVisibility(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot convert Visibility to SearchForParameter");
        }

        public bool VisibleForMatch
        {
            get { return visibleForMatch; }
            set { visibleForMatch = value; }
        }

        private bool visibleForMatch;

        private Visibility toggleVisibility(Visibility v)
        {
            return v == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
