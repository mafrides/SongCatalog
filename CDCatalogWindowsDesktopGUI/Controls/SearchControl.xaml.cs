using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CDCatalogWindowsDesktopGUI
{
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();
        }

        private void SearchByTitleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SearchViewModel s = ((SearchViewModel)this.DataContext);
            BindingOperations.ClearAllBindings(SearchComboBox);
            SearchComboBox.SetBinding(ComboBox.ItemsSourceProperty,
                new Binding("SearchedTitles") { Source = s });
            SearchComboBox.SetBinding(ComboBox.TextProperty,
                new Binding("SearchString") { Source = s, Mode = BindingMode.TwoWay });
            ((SearchViewModel)this.DataContext).SearchByParameter = SearchByParameter.Titles;
        }

        private void SearchByArtistRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel m = (MainWindowViewModel)(((MainWindow)Window.GetWindow(this)).DataContext);
            SearchViewModel s = ((SearchViewModel)this.DataContext);
            BindingOperations.ClearAllBindings(SearchComboBox);
            SearchComboBox.SetBinding(ComboBox.ItemsSourceProperty,
                new Binding("Artists") { Source = m });
            SearchComboBox.SetBinding(ComboBox.DisplayMemberPathProperty,
                new Binding("Name") { Source = SearchComboBox.SelectedItem });
            SearchComboBox.SetBinding(ComboBox.SelectedValuePathProperty,
                new Binding("Name") { Source = SearchComboBox.SelectedItem });
            SearchComboBox.SetBinding(ComboBox.TextProperty,
                new Binding("SearchString") { Source = s, Mode = BindingMode.OneWayToSource });
            ((SearchViewModel)this.DataContext).SearchByParameter = SearchByParameter.Artists;
        }

        private void SearchByGenreRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel m = (MainWindowViewModel)(((MainWindow)Window.GetWindow(this)).DataContext);
            SearchViewModel s = ((SearchViewModel)this.DataContext);
            BindingOperations.ClearAllBindings(SearchComboBox);
            SearchComboBox.SetBinding(ComboBox.ItemsSourceProperty,
                new Binding("Genres") { Source = m });
            SearchComboBox.SetBinding(ComboBox.DisplayMemberPathProperty,
                new Binding("Name") { Source = SearchComboBox.SelectedItem });
            SearchComboBox.SetBinding(ComboBox.SelectedValuePathProperty,
                new Binding("Name") { Source = SearchComboBox.SelectedItem });
            SearchComboBox.SetBinding(ComboBox.TextProperty,
                new Binding("SearchString") { Source = s, Mode = BindingMode.OneWayToSource });
            ((SearchViewModel)this.DataContext).SearchByParameter = SearchByParameter.Genres;
        }

        private void SearchByAllRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearAllBindings(SearchComboBox);
            ((SearchViewModel)this.DataContext).SearchByParameter = SearchByParameter.All;
        }
    }
}
