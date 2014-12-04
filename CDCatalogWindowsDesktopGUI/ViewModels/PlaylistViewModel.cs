using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogWindowsDesktopGUI
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        public PlaylistViewModel(MainWindowViewModel parent)
        {
            parentViewModel = parent;
            createPlaylistCommandAsync = new DelegateCommandAsync(OnCreatePlaylistAsync);
            songs = new ObservableCollection<Song>();
        }

        public ICDCatalog Catalog
        {
            get { return parentViewModel.Catalog; }
        }
        public ObservableCollection<Song> Songs
        {
            get { return songs; }
            set
            {
                if(songs != value)
                {
                    songs = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Songs"));
                }
            }
        }
        public Nullable<int> Minutes
        {
            get { return minutes; }
            set
            {
                if(minutes != value)
                {
                    minutes = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Minutes"));
                }
            }
        }

        public DelegateCommandAsync CreatePlaylistCommandAsync
        {
            get { return createPlaylistCommandAsync; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private readonly MainWindowViewModel parentViewModel;
        private readonly DelegateCommandAsync createPlaylistCommandAsync;
        private ObservableCollection<Song> songs;
        private Nullable<int> minutes;

        private ObservableCollection<T> Observe<T>(List<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        private async Task OnCreatePlaylistAsync()
        {
            if (Minutes == null || Minutes <= 0)
            {
                Songs.Clear();
                return;
            }
            try
            {
                Songs = Observe(await Catalog.createPlaylistAsync((int)Minutes));
            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
    }
}
