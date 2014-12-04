using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogWindowsDesktopGUI
{
    public class GridViewModel : INotifyPropertyChanged
    {
        public GridViewModel(MainWindowViewModel parent)
        {
            parentViewModel = parent;
            recordsPerPage = 300;
            albumsAndSongs = new ObservableCollection<IAlbumOrSong>();
            editSongCommandAsync = new DelegateCommandAsync(OnEditAlbumOrSongAsync<Song>);
            editAlbumCommandAsync = new DelegateCommandAsync(OnEditAlbumOrSongAsync<Album>);
            cancelEditSongCommand = new DelegateCommand(OnCancelEditAlbumOrSong<Song>);
            cancelEditAlbumCommand = new DelegateCommand(OnCancelEditAlbumOrSong<Album>);
        }
        public ICDCatalog Catalog
        {
            get { return parentViewModel.Catalog; }
        }
        public ObservableCollection<int> Years
        {
            get { return parentViewModel.Years; }
        }
        public ObservableCollection<IAlbumOrSong> AlbumsAndSongs
        {
            get { return albumsAndSongs; }
            set
            {
                if(albumsAndSongs != value)
                {
                    albumsAndSongs = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("AlbumsAndSongs"));
                }
            }
        }
        public IAlbumOrSong SelectedAlbumOrSong
        {
            get { return selectedAlbumOrSong; }
            set
            {
                if(selectedAlbumOrSong != value)
                {
                    selectedAlbumOrSong = value;
                    if(selectedAlbumOrSong != null)
                    {
                        selectedAlbumOrSong.BeginEdit();
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedAlbumOrSong"));
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedAlbum"));
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedSong"));
                }
            }
        }
        public Album SelectedAlbum
        {
            get
            {
                return SelectedAlbumOrSong as Album;
            }
            set
            {
                if (SelectedAlbumOrSong != value)
                {
                    SelectedAlbumOrSong = value;
                }
            }
        }
        public Song SelectedSong
        {
            get
            {
                return SelectedAlbumOrSong as Song;
            }
            set
            {
                if(SelectedAlbumOrSong != value)
                {
                    SelectedAlbumOrSong = value;
                }
            }
        }      
        public int RecordsPerPage
        {
            get { return recordsPerPage; }
            set
            {
                if(value >= 0 && recordsPerPage != value)
                {
                    recordsPerPage = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("RecordsPerPage"));
                }
            }
        }
        public DelegateCommandAsync EditSongCommandAsync
        {
            get { return editSongCommandAsync; }
        }
        public DelegateCommandAsync EditAlbumCommandAsync
        {
            get { return editAlbumCommandAsync; }
        }
        public DelegateCommand CancelEditSongCommand
        {
            get { return cancelEditSongCommand; }
        }
        public DelegateCommand CancelEditAlbumCommand
        {
            get { return cancelEditAlbumCommand; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly MainWindowViewModel parentViewModel;
        private ObservableCollection<IAlbumOrSong> albumsAndSongs;
        private IAlbumOrSong selectedAlbumOrSong;
        private int recordsPerPage = 300;

        private readonly DelegateCommandAsync editSongCommandAsync;
        private readonly DelegateCommandAsync editAlbumCommandAsync;
        private readonly DelegateCommand cancelEditSongCommand;
        private readonly DelegateCommand cancelEditAlbumCommand;

        private async Task OnEditAlbumOrSongAsync<T>() where T : IAlbumOrSong
        {
            try
            {
                if (SelectedAlbumOrSong == null)
                {
                    return;
                }

                IAlbumOrSong savedAlbumOrSong = null;
                if (typeof(T) == typeof(Song))
                {
                    savedAlbumOrSong = await Catalog.updateSongAsync(SelectedSong);
                }
                else if (typeof(T) == typeof(Album))
                {
                    savedAlbumOrSong = await Catalog.updateAlbumAsync(SelectedAlbum);
                }

                if (savedAlbumOrSong == null)
                {
                    SelectedAlbumOrSong.CancelEdit();
                    SelectedAlbumOrSong.BeginEdit();
                }
                else
                {
                    SelectedAlbumOrSong = savedAlbumOrSong;
                }
            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }
        }
        private void OnCancelEditAlbumOrSong<T>() where T : IAlbumOrSong
        {
            SelectedAlbumOrSong.CancelEdit();
            SelectedAlbumOrSong.BeginEdit();
        }
    }
}
