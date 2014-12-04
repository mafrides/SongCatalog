using System;
using System.Threading.Tasks;
using System.ComponentModel;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogWindowsDesktopGUI
{
    public class DeleteAlbumOrSongViewModel : INotifyPropertyChanged
    {
        public DeleteAlbumOrSongViewModel(MainWindowViewModel parent)
        {
            parentViewModel = parent;
            deleteCommandAsync = new DelegateCommandAsync(OnDeleteAsync);
        }

        public ICDCatalog Catalog
        {
            get { return parentViewModel.Catalog; }
        }

        public DelegateCommandAsync DeleteCommandAsync
        {
            get { return deleteCommandAsync; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly MainWindowViewModel parentViewModel;
        private readonly DelegateCommandAsync deleteCommandAsync;

        private async Task OnDeleteAsync()
        {
            try
            {
                Song selectedSong = parentViewModel.GridViewModel.SelectedSong;
                if (selectedSong != null)
                {
                    parentViewModel.GridViewModel.AlbumsAndSongs.Remove(selectedSong);
                    await Catalog.removeSongAsync(selectedSong);
                }
                else
                {
                    Album selectedAlbum = parentViewModel.GridViewModel.SelectedAlbum;
                    if (selectedAlbum != null)
                    {
                        parentViewModel.GridViewModel.AlbumsAndSongs.Remove(selectedAlbum);
                        await Catalog.removeAlbumAsync(selectedAlbum);
                    }
                }
            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }
        }
    }
}
