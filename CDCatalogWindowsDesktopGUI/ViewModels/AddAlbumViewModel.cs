using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogWindowsDesktopGUI
{
    public class AddAlbumViewModel : INotifyPropertyChanged
    {
        public AddAlbumViewModel(MainWindowViewModel parent)
        {
            parentViewModel = parent;
            addAlbumCommandAsync = new DelegateCommandAsync(OnAddAlbumAsync);
            clearFormCommand = new DelegateCommand(clearForm);
            artist = new Artist();
            genre = new Genre();
        }

        public ICDCatalog Catalog
        {
            get { return parentViewModel.Catalog; }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if(title != value)
                {
                    title = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                }
            }
        }
        public int Year
        {
            get { return year; }
            set
            {
                if(year != value)
                {
                    year = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Year"));
                }
            }
        }
        public Nullable<double> Rating
        {
            get { return rating; }
            set
            {
                if(rating != value)
                {
                    rating = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Rating"));
                }
            }
        }
        public Artist Artist
        {
            get { return artist; }
            set
            {
                if(artist != value)
                {
                    artist = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Artist"));
                }
            }
        }
        public Genre Genre
        {
            get { return genre; }
            set
            {
                if(genre != value)
                {
                    genre = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Genre"));
                }
            }
        }
        public Nullable<bool> LastSaveSucceeded
        {
            get { return lastSaveSucceeded; }
            set
            {
                if(lastSaveSucceeded != value)
                {
                    lastSaveSucceeded = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("LastSaveSucceeded"));
                }
            }
        }
        
        public ObservableCollection<int> Years
        {
            get { return parentViewModel.Years; }
        }

        public DelegateCommandAsync AddAlbumCommandAsync
        {
            get { return addAlbumCommandAsync; }
        }
        public DelegateCommand ClearFormCommand
        {
            get { return clearFormCommand; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly MainWindowViewModel parentViewModel;
        private readonly DelegateCommandAsync addAlbumCommandAsync;
        private readonly DelegateCommand clearFormCommand;
        private string title;
        private int year;
        private Nullable<double> rating;
        private Artist artist;
        private Genre genre;
        private Nullable<bool> lastSaveSucceeded;

        private void clearForm()
        {
            this.Title = "";
            this.Year = 1999;
            this.Rating = 0.5;
            this.Artist = new Artist { Name = "" };
            this.Genre = new Genre { Name = "" };
        }

        private async Task OnAddAlbumAsync()
        {
            Album album = new Album
            {
                Title = this.Title ?? "",
                Year = this.Year,
                Rating = this.Rating == null ? null : (Nullable<int>)(2 * this.Rating),
                Artist = new Artist
                {
                    Id = this.Artist.Id,
                    Name = this.Artist.Name ?? ""
                },
                Genre = new Genre
                {
                    Id = this.Genre.Id,
                    Name = this.Genre.Name ?? ""
                }
            };
            try
            {
                Album savedAlbum = await Catalog.insertAlbumAsync(album);
                if (savedAlbum != null)
                {
                    LastSaveSucceeded = true;
                    parentViewModel.Albums.Insert(0, savedAlbum);
                    parentViewModel.GridViewModel.AlbumsAndSongs.Insert(0, savedAlbum);
                    clearForm();
                }
                else
                {
                    LastSaveSucceeded = false;
                }
            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {
                LastSaveSucceeded = false;
            }
        }
    }
}
