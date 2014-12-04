using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CDCatalogWindowsDesktopGUI
{
    public class RadioButtonBoolToSearchByParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Converts IsChecked; ignores false
            if (System.Convert.ToBoolean(value))
            {
                try
                {
                    return (SearchByParameter)Enum.Parse(typeof(SearchByParameter), parameter.ToString(), true);
                }
                catch { }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SearchByParameter)
            {
                if (String.Equals(Enum.GetName(typeof(SearchByParameter), value),
                    parameter.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return Binding.DoNothing;
        }
    }
}
