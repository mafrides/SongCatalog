using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogWindowsDesktopGUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        static MainWindowViewModel()
        {
            searchTermSkipValue = skipNone;
            searchTermTakeValue = takeAll;
        }
        public MainWindowViewModel()
        {
            catalog = new CDCatalogEFDisconnected();

            searchViewModel = new SearchViewModel(this);
            gridViewModel = new GridViewModel(this);
            addAlbumOrSongViewModel = new AddAlbumOrSongViewModel(this);
            deleteAlbumOrSongViewModel = new DeleteAlbumOrSongViewModel(this);
            playListViewModel = new PlaylistViewModel(this);
            pagingViewModel = new PagingViewModel(this);

            Albums = new ObservableCollection<Album>();
            Artists = new ObservableCollection<Artist>();
            Genres = new ObservableCollection<Genre>();

            minYear = 1600;
            initializeYears();

            refreshAlbumsCommandAsync = new DelegateCommandAsync(OnRefreshAlbums);
            refreshArtistsCommandAsync = new DelegateCommandAsync(OnRefreshArtists);
            refreshGenresCommandAsync = new DelegateCommandAsync(OnRefreshGenres);
            cleanArtistsAndGenresCommandAsync = new DelegateCommandAsync(OnClean);
        }

        public ICDCatalog Catalog
        {
            get { return catalog; }
        }

        public SearchViewModel SearchViewModel
        {
            get { return searchViewModel; }
        }
        public GridViewModel GridViewModel
        {
            get { return gridViewModel; }
        }
        public AddAlbumOrSongViewModel AddAlbumOrSongViewModel
        {
            get { return addAlbumOrSongViewModel; }
        }
        public DeleteAlbumOrSongViewModel DeleteAlbumOrSongViewModel
        {
            get { return deleteAlbumOrSongViewModel; }
        }
        public PlaylistViewModel PlaylistViewModel
        {
            get { return playListViewModel; }
        }
        public PagingViewModel PagingViewModel
        {
            get { return pagingViewModel; }
        }
        public ObservableCollection<Album> Albums
        {
            get { return albums; }
            set
            {
                if(albums != value)
                {
                    albums = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Albums"));
                }
            }
        }
        public ObservableCollection<Artist> Artists
        {
            get { return artists; }
            set
            {
                if(artists != value)
                {
                    artists = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Artists"));
                }
            }
        }
        public ObservableCollection<Genre> Genres
        {
            get { return genres; }
            set
            {
                if(genres != value)
                {
                    genres = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Genres"));
                }
            }
        }

        public ObservableCollection<int> Years
        {
            get { return years; }
        }
        public static int CurrentYear
        {
            get { return DateTime.Today.Year; }
        }
        public readonly int minYear;

        public DelegateCommandAsync RefreshAlbumsCommandAsync
        {
            get { return refreshAlbumsCommandAsync; }
        }
        public DelegateCommandAsync RefreshArtistsCommandAsync
        {
            get { return refreshArtistsCommandAsync; }
        }
        public DelegateCommandAsync RefreshGenresCommandAsync
        {
            get { return refreshGenresCommandAsync; }
        }
        public DelegateCommandAsync CleanArtistsAndGenresCommandAsync
        {
            get { return cleanArtistsAndGenresCommandAsync; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ICDCatalog catalog;

        private readonly SearchViewModel searchViewModel;
        private readonly GridViewModel gridViewModel;
        private readonly AddAlbumOrSongViewModel addAlbumOrSongViewModel;
        private readonly DeleteAlbumOrSongViewModel deleteAlbumOrSongViewModel;
        private readonly PlaylistViewModel playListViewModel;
        private readonly PagingViewModel pagingViewModel;

        private ObservableCollection<Album> albums;
        private ObservableCollection<Artist> artists;
        private ObservableCollection<Genre> genres;
        private ObservableCollection<int> years;

        private readonly DelegateCommandAsync refreshAlbumsCommandAsync;
        private readonly DelegateCommandAsync refreshArtistsCommandAsync;
        private readonly DelegateCommandAsync refreshGenresCommandAsync;
        private readonly DelegateCommandAsync cleanArtistsAndGenresCommandAsync;

        private static readonly int searchTermSkipValue;
        private static readonly int searchTermTakeValue;

        private const int skipNone = 0;
        private const int takeAll = Int32.MaxValue;

        private ObservableCollection<T> Observable<T>(List<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        private void initializeYears()
        {
            years = new ObservableCollection<int>();
            for (int i = CurrentYear; i >= minYear; --i)
            {
                years.Add(i);
            }
        }

        private async Task OnRefreshAlbums()
        {
            try
            {
                Albums = Observable(await Catalog.getAlbumsAsync(searchTermSkipValue, searchTermTakeValue));
                Albums.Insert(0, null);
            }
            catch(CDCatalogException cex)
            {

            }
            catch(Exception ex)
            {

            }
        }
        private async Task OnRefreshArtists()
        {
            try
            {
                Artists = Observable(await Catalog.getArtistsAsync(searchTermSkipValue, searchTermTakeValue));
            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }
        }
        private async Task OnRefreshGenres()
        {
            try
            {
                Genres = Observable(await Catalog.getGenresAsync(searchTermSkipValue, searchTermTakeValue));
            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }          
        }
        private async Task OnClean()
        {
            try
            {
                Task[] cleanTasks = 
                {
                    Catalog.removeArtistsWithoutSongsAsync(),
                    Catalog.removeGenresWithoutSongsAsync()
                };
                await Task.WhenAll(cleanTasks);
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
