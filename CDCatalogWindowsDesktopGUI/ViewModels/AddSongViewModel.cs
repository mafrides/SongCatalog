using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogWindowsDesktopGUI
{
    public class AddSongViewModel : INotifyPropertyChanged
    {
        public AddSongViewModel(MainWindowViewModel parent)
        {
            parentViewModel = parent;
            addSongCommandAsync = new DelegateCommandAsync(OnAddSongAsync);
            clearFormCommand = new DelegateCommand(clearForm);
            artist = new Artist();
            genre = new Genre();
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
        public string TrackLength
        {
            get { return trackLength; }
            set
            {
                if(trackLength != value)
                {
                    trackLength = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("TrackLength"));
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
        public Nullable<int> TrackNumber
        {
            get { return trackNumber; }
            set
            {
                if(trackNumber != value)
                {
                    trackNumber = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("TrackNumber"));
                }
            }
        }
        public string Url
        {
            get { return url; }
            set
            {
                if(url != value)
                {
                    url = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Url"));
                }
            }
        }
        public Album Album
        {
            get { return album; }
            set
            {
                if(album != value)
                {
                    album = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Album"));
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
        public static int ThisYear
        {
            get { return DateTime.Today.Year; }
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

        public ICDCatalog Catalog
        {
            get {return parentViewModel.Catalog;  }
        }
        public DelegateCommandAsync AddSongCommandAsync
        {
            get { return addSongCommandAsync; }
        }
        public DelegateCommand ClearFormCommand
        {
            get { return clearFormCommand; }
        }

        public void clearForm()
        {
            Title = "";
            TrackLength = "00:00";
            Rating = 0.5;
            TrackNumber = 0;
            Url = "";
            artist = new Artist { Name = "" };
            genre = new Genre { Name = "" };
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly MainWindowViewModel parentViewModel;
        private readonly DelegateCommandAsync addSongCommandAsync;
        private readonly DelegateCommand clearFormCommand;

        private string title;
        private string trackLength;
        private Nullable<double> rating;
        private Nullable<int> trackNumber;
        private string url;
        private Album album;
        private Artist artist;
        private Genre genre;
        private Nullable<bool> lastSaveSucceeded;

        private async Task OnAddSongAsync()
        {
            Song song = new Song
            {
                Title = this.Title,
                TrackLength = convertLengthStringToSeconds(this.TrackLength),
                Rating = this.Rating == null ? null : (Nullable<int>)(2 * this.Rating),
                TrackNumber = this.TrackNumber,
                Url = this.Url,
                Album = this.Album,
                Artist = new Artist
                {
                    Id = this.Artist.Id,
                    Name = this.Artist.Name
                },
                Genre = new Genre
                {
                    Id = this.Genre.Id,
                    Name = this.Genre.Name
                }
            };
            try
            {
                Song savedSong = await Catalog.insertSongAsync(song);
                if (savedSong != null)
                {
                    LastSaveSucceeded = true;
                    parentViewModel.GridViewModel.AlbumsAndSongs.Insert(0, savedSong);
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

        private int convertLengthStringToSeconds(string length)
        {
            if (String.IsNullOrEmpty(length)) return 0;
            //try to parse it as a number of seconds if whole number
            //try to parse as number of minutes if fractional
            if(!length.Contains(':'))
            {
                int seconds;
                if(int.TryParse(length, out seconds)) return seconds;
                double minutes;
                if (double.TryParse(length, out minutes)) return (int)(minutes * 60);
                return 0;
            }
            //parse in form <min>:<seconds>
            string[] stringSections = ((string)length).Split(':');
            if (stringSections == null || stringSections.Length != 2) return 0;
            for(int i = 0; i < 2; ++i)
            {
                if (String.IsNullOrEmpty(stringSections[i])) stringSections[i] = "00";
                if (stringSections[i][0] == '0') stringSections[i] = stringSections[i].Substring(1);
            }
            try
            {
                return int.Parse(stringSections[0]) * 60 + int.Parse(stringSections[1]);
            }
            catch
            {
                return 0;
            }
        }
    }
}
