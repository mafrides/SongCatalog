using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogWindowsDesktopGUI
{
    public enum SearchByParameter
    {
        All = 0,
        Titles,
        Artists,
        Genres
    }

    public enum SearchForParameter
    {
        None = 0,
        Songs,
        Albums,
        AlbumsAndSongs
    }

    public class Search : INotifyPropertyChanged
    {
        public Search()
        {
            SearchBy = SearchByParameter.All;
            SearchFor = SearchForParameter.AlbumsAndSongs;
            SearchString = "";
            Skip = 0;
            Take = 300;
        }

        public SearchByParameter SearchBy
        {
            get { return searchBy; }
            set
            {
                if(searchBy != value)
                {
                    searchBy = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchBy"));
                }
            }
        }
        public SearchForParameter SearchFor
        {
            get { return searchFor; }
            set
            {
                if (searchFor != value)
                {
                    searchFor = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchFor"));
                }
            }
        }
        public string SearchString
        {
            get { return searchString; }
            set
            {
                if (searchString != value)
                {
                    searchString = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchString"));
                }
            }
        }
        public int Skip
        {
            get { return skip; }
            set
            {
                if (skip != value)
                {
                    skip = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Skip"));
                }
            }
        }
        public int Take
        {
            get { return take; }
            set
            {
                if (take != value)
                {
                    take = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Take"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private SearchByParameter searchBy;
        private SearchForParameter searchFor;
        private string searchString;
        private int skip;
        private int take;
    }

    public class SearchViewModel : INotifyPropertyChanged
    {
        static SearchViewModel()
        {
            stringComparison = StringComparison.CurrentCultureIgnoreCase;
        }
        public SearchViewModel(MainWindowViewModel parent)
        {
            parentViewModel = parent;
            searchedTitles = new ObservableCollection<string>();
            searchCommandAsync = new DelegateCommandAsync(OnSearch);
            lastSearch = new Search();
        }

        public ObservableCollection<string> SearchedTitles
        {
            get { return searchedTitles; }
            set
            {
                if(searchedTitles != value)
                {
                    searchedTitles = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchedTitles"));
                }
            }
        }
        public SearchByParameter SearchByParameter
        {
            get { return searchByParameter; }
            set
            {
                if(searchByParameter != value)
                {
                    searchByParameter = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchByParameter"));
                }
            }
        }
        public bool SearchForSongs
        {
            get { return searchForSongs; }
            set
            {
                if (searchForSongs = value)
                {
                    searchForSongs = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchForSongs"));
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchFor"));
                }
            }
        }
        public bool SearchForAlbums
        {
            get { return searchForAlbums; }
            set
            {
                if (searchForAlbums != value)
                {
                    searchForAlbums = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchForAlbums"));
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchFor"));
                }
            }
        }
        public SearchForParameter SearchForParameter
        {
            get
            {
                if (SearchForSongs && SearchForAlbums) return SearchForParameter.AlbumsAndSongs;
                else if (SearchForSongs) return SearchForParameter.Songs;
                else if (SearchForAlbums) return SearchForParameter.Albums;
                else return SearchForParameter.None;
            }
        }
        public int Skip
        {
            get { return skip; }
            set
            {
                if(skip != value)
                {
                    skip = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Skip"));
                }
            }
        }
        public int Take
        {
            get { return parentViewModel.GridViewModel.RecordsPerPage; }
        }
        public string SearchString
        {
            get { return searchString; }
            set
            {
                if(!String.Equals(searchString, value, stringComparison))
                {
                    searchString = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchString"));
                    PropertyChanged(this, new PropertyChangedEventArgs("TitleSearchString"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ArtistSearchString"));
                    PropertyChanged(this, new PropertyChangedEventArgs("GenreSearchString"));
                }
            }
        }
        public string TitleSearchString
        {
            get { return SearchString; }
        }
        public Artist ArtistSearchString
        {
            get { return new Artist { Name = SearchString }; }
        }
        public Genre GenreSearchString
        {
            get { return new Genre { Name = SearchString }; }
        }
        public Nullable<bool> LastSearchSucceeded
        {
            get { return lastSearchSucceeded; }
            set
            {
                if(lastSearchSucceeded != value)
                {
                    lastSearchSucceeded = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("LastSearchSucceeded"));
                }
            }
        }
        public Search LastSearch
        {
            get { return lastSearch; }
            set
            {
                if(lastSearch != value)
                {
                    lastSearch = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("LastSearch"));
                }
            }
        }
        
        public ICDCatalog Catalog
        {
            get { return parentViewModel.Catalog; }
        }

        public DelegateCommandAsync SearchCommandAsync
        {
            get { return searchCommandAsync; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly MainWindowViewModel parentViewModel;
        private ObservableCollection<string> searchedTitles;
        private SearchByParameter searchByParameter;
        private bool searchForSongs;
        private bool searchForAlbums;
        private int skip;
        private string searchString;
        private Nullable<bool> lastSearchSucceeded;
        private Search lastSearch;
        private static readonly StringComparison stringComparison;

        private readonly DelegateCommandAsync searchCommandAsync;

        private async Task OnSearch()
        {
            if (SearchForParameter == SearchForParameter.None 
                || (SearchByParameter !=  SearchByParameter.All && String.IsNullOrEmpty(SearchString))) return;
            try
            {
                switch (SearchByParameter)
                {
                    case SearchByParameter.All:
                        switch (SearchForParameter)
                        {
                            case SearchForParameter.Songs:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.getSongsAsync(Skip, Take));
                                break;
                            case SearchForParameter.Albums:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.getAlbumsWithSongsAsync(Skip, Take));
                                break;
                            case SearchForParameter.AlbumsAndSongs:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.getAlbumsAndSongsAsync(Skip, Take));
                                break;
                            default: return;
                        }
                        break;
                    case SearchByParameter.Titles:
                        switch (SearchForParameter)
                        {
                            case SearchForParameter.Songs:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.findSongsAsync(TitleSearchString, Skip, Take));
                                break;
                            case SearchForParameter.Albums:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.findAlbumsAsync(TitleSearchString, Skip, Take));
                                break;
                            case SearchForParameter.AlbumsAndSongs:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.findAlbumsAndSongsAsync(TitleSearchString, Skip, Take));
                                break;
                            default: return;
                        }
                        if (parentViewModel.GridViewModel.AlbumsAndSongs != null
                            && parentViewModel.GridViewModel.AlbumsAndSongs.Count() > 0)
                        { 
                            SearchedTitles.Add(TitleSearchString);
                        }
                        break;
                    case SearchByParameter.Artists:
                        switch (SearchForParameter)
                        {
                            case SearchForParameter.Songs:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.findSongsAsync(ArtistSearchString, Skip, Take));
                                break;
                            case SearchForParameter.Albums:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.findAlbumsAsync(ArtistSearchString, Skip, Take));
                                break;
                            case SearchForParameter.AlbumsAndSongs:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.findAlbumsAndSongsAsync(ArtistSearchString, Skip, Take));
                                break;
                            default: return;
                        }
                        break;
                    case SearchByParameter.Genres:
                        switch (SearchForParameter)
                        {
                            case SearchForParameter.Songs:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.findSongsAsync(GenreSearchString, Skip, Take));
                                break;
                            case SearchForParameter.Albums:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.findAlbumsAsync(GenreSearchString, Skip, Take));
                                break;
                            case SearchForParameter.AlbumsAndSongs:
                                parentViewModel.GridViewModel.AlbumsAndSongs
                                    = Observable(await Catalog.findAlbumsAndSongsAsync(GenreSearchString, Skip, Take));
                                break;
                            default: return;
                        }
                        break;
                    default: return;
                }
                if (parentViewModel.GridViewModel.AlbumsAndSongs != null
                    && parentViewModel.GridViewModel.AlbumsAndSongs.Count() > 0)
                {
                    LastSearchSucceeded = true;
                    LastSearch = new Search
                    {
                        SearchBy = this.SearchByParameter,
                        SearchFor = this.SearchForParameter,
                        SearchString = this.SearchString,
                        Skip = this.Skip,
                        Take = this.Take
                    };
                }
                else LastSearchSucceeded = false;
            }
            catch (CDCatalogException cex)
            {
                LastSearchSucceeded = false;
            }
            catch (Exception ex)
            {
                LastSearchSucceeded = false;
            }
            finally
            {
                
            }
        }

        //Converters from List to Observable Collection
        private ObservableCollection<IAlbumOrSong> Observable(List<Song> collection)
        {
            return new ObservableCollection<IAlbumOrSong>(collection);
        }
        private ObservableCollection<IAlbumOrSong> Observable(List<Album> collection)
        {
            return new ObservableCollection<IAlbumOrSong>(collection);
        }
        private ObservableCollection<IAlbumOrSong> Observable(List<IAlbumOrSong> collection)
        {
            return new ObservableCollection<IAlbumOrSong>(collection);
        }
    }
}
