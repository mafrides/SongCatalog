using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CDCatalogModel;
using CDCatalogDAL;
using System.Threading.Tasks;

namespace CDCatalogWindowsDesktopGUI
{
    public partial class MainWindow : Window
    {
        static MainWindow()
        {
            zIndexCounter = 11;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task[] t = 
            {
                ((MainWindowViewModel)this.DataContext).RefreshAlbumsCommandAsync.ExecuteAsync(new object()),
                ((MainWindowViewModel)this.DataContext).RefreshArtistsCommandAsync.ExecuteAsync(new object()),
                ((MainWindowViewModel)this.DataContext).RefreshGenresCommandAsync.ExecuteAsync(new object())
            };
            await Task.WhenAll(t);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            toggleVisibility(AddControl);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            toggleVisibility(EditControl);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            toggleVisibility(DeleteControl);
        }

        private void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            toggleVisibility(Playlist);
        }

        private void toggleVisibility(UIElement element)
        {
            if (element.Visibility == Visibility.Collapsed)
            {
                element.Visibility = Visibility.Visible;
                Panel.SetZIndex(element, NextZIndex);
            }
            else if (element.Visibility == Visibility.Visible)
            {
                if (Panel.GetZIndex(element) < CurrentZIndex)
                {
                    Panel.SetZIndex(element, NextZIndex);
                }
                else
                {
                    element.Visibility = Visibility.Collapsed;
                    --zIndexCounter;
                }
            }
        }

        private static int zIndexCounter;

        private int NextZIndex
        {
            get { return ++zIndexCounter; }
        }

        private int CurrentZIndex
        {
            get { return zIndexCounter; }
        }       
    }
}