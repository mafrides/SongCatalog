using System;
using System.ComponentModel;
using System.Text;

namespace CDCatalogModel
{
    public partial class Song
        : INotifyPropertyChanged, IEditableObject, IComparable<Song>, 
        IAlbumOrSong, IHasId, IHasTitle, IHasArtist, IHasGenre, IRated, 
        IHasDisplayTitle, IHasDisplayTrackLength, IHasDisplayRating
    {
        static Song()
        {
            ignoreCaseForTitleComparison = true;
        }

        public int Id
        {
            get { return id; }
            set
            {
                if(id != value)
                {
                    id = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Id"));
                }
            }
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
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
                }
            }
        }
        public int TrackLength
        {
            get { return trackLength; }
            set
            {
                if(trackLength != value)
                {
                    trackLength = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("TrackLength"));
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayTrackNumber"));
                }
            }
        }
        public Nullable<int> Rating
        {
            get { return rating; }
            set
            {
                if(rating != value)
                {
                    rating = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Rating"));
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayRating"));
                }
            }
        }
        public int ArtistId
        {
            get { return artistId; }
            set
            {
                if(artistId != value)
                {
                    artistId = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ArtistId"));
                }
            }
        }
        public int GenreId
        {
            get { return genreId; }
            set
            {
                if(genreId != value)
                {
                    genreId = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("GenreId"));
                }
            }
        }
        public Nullable<int> AlbumId
        {
            get { return albumId; }
            set
            {
                if(albumId != value)
                {
                    albumId = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("AlbumId"));
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

        public string DisplayTitle
        {
            get { return this.ToString(); }
        }
        public string DisplayTrackLength
        {
            get
            {
                int length = TrackLength;
                int hours = length / 3600;
                length %= 3600;
                int minutes = length / 60;
                length %= 60;
                int seconds = length;
                StringBuilder sb = new StringBuilder();
                if(hours != 0)
                {
                    if (hours < 10) sb.Append("0");
                    sb.Append(hours);
                    sb.Append(":");
                }
                if (minutes < 10) sb.Append("0");
                sb.Append(minutes);
                sb.Append(":");
                if (seconds < 10) sb.Append("0");
                sb.Append(seconds);
                return sb.ToString();
            }
        }
        public Nullable<double> DisplayRating
        {
            get { return Rating == null ? null : (Nullable<double>)(Rating /2); }
        }

        //Application Specific Validation:
        //Valid Id or 0 (unset, as when inserting a new Song)
        //Required Title not ""
        //Required TrackLength > 0
        //Rating 1-10 or unrated (null)
        //Required Artist or ArtistId, if both must match Id
        //Required Genre or GenreId, if both must match Id
        //Optional Album or AlbumId, if both must match Id
        //If Album then TrackNumber > 0; If no Album, then Tracknumber = null
        //Optional Url not ""
        public bool IsValid
        {
            get
            {
                return (Id >= 0)
                    && (!String.IsNullOrEmpty(Title))
                    && (TrackLength > 0)
                    && (Rating == null || (Rating >= 1 && Rating <= 10))
                    && (Url == null || Url.Length > 0)
                    && (ArtistId >= 0)
                    && (Artist == null || Artist.IsValid)
                    && (ArtistId > 0 || (Artist != null && Artist.IsValid))
                    && ((ArtistId > 0 && Artist != null && Artist.IsValid && Artist.Id > 0) ? Artist.Id == ArtistId : true)
                    && (GenreId >= 0)
                    && (Genre == null || Genre.IsValid)
                    && (GenreId > 0 || (Genre != null && Genre.IsValid))
                    && ((GenreId > 0 && Genre != null && Genre.IsValid && Genre.Id > 0) ? Genre.Id == GenreId : true)
                    && (AlbumId == null || AlbumId > 0)
                    && (Album == null || Album.IsValid)
                    && ((AlbumId != null && AlbumId > 0 && Album != null && Album.IsValid && Album.Id != 0)
                            ? AlbumId == Album.Id : true)
                    && (TrackNumber == null || TrackNumber > 0)
                    && !(TrackNumber > 0 && AlbumId == null && (Album == null || !Album.IsValid));
            }
        }
        
        //Hash by Title
        public override int GetHashCode()
        {
            return Title != null ? Title.GetHashCode() : base.GetHashCode();
        }

        //Display by Title
        public override string ToString()
        {
            return String.IsNullOrEmpty(Title) ? "" : Title;
        }

        //IComparable: Sort by Title, Album
        public int CompareTo(Song other)
        {
            if (Title == null) return 1;
            else if (other == null || other.Title == null) return -1;
            int titleComparer = String.Compare(Title, other.Title, ignoreCaseForTitleComparison);
            if (titleComparer != 0) return titleComparer;
            else
            {
                if (this.Album == null || this.Album.Title == null) return 1;
                else return this.Album.CompareTo(other.Album);
            }
        }

        //INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //IEditable
        public void BeginEdit()
        {
            temp = new Song
            {
                Id = this.id,
                Title = String.Copy(this.title),
                TrackLength = this.trackLength,
                Rating = this.rating,
                ArtistId = this.artistId,
                GenreId = this.genreId,
                AlbumId = this.albumId,
                TrackNumber = this.trackNumber,
                Url = String.Copy(this.url)
            };
            temp.Artist = this.artist == null ? null 
                : new Artist { Id = this.artist.Id, Name = String.Copy(this.artist.Name) };
            temp.Genre = this.genre == null ? null 
                : new Genre { Id = this.genre.Id, Name = String.Copy(this.genre.Name) };
            temp.Album = this.album == null ? null 
                : new Album { Id = this.album.Id, Title = String.Copy(this.album.Title) };
        }
        public void CancelEdit()
        {
            if (temp == null) return;
            this.Id = temp.id;
            this.Title = temp.title;
            this.TrackLength = temp.trackLength;
            this.Rating = temp.rating;
            this.ArtistId = temp.artistId;
            this.GenreId = temp.genreId;
            this.AlbumId = temp.albumId;
            this.TrackNumber = temp.trackNumber;
            this.Url = temp.url;
            if (temp.artist == null) this.Artist = null;
            else
            {
                this.Artist.Id = temp.artist.Id;
                this.Artist.Name = temp.artist.Name;
            }
            if (temp.genre == null) this.Genre = null;
            else
            {
                this.Genre.Id = temp.genre.Id;
                this.Genre.Name = temp.genre.Name;
            }
            if (temp.album == null) this.Album = null;
            else
            {
                this.Album.Id = temp.album.Id;
                this.Album.Title = temp.album.Title;
            }
        }
        public void EndEdit()
        {
            temp = null;
        }

        #region Fields

        //Backing Fields
        private int id;
        private string title;
        private int trackLength;
        private Nullable<int> rating;
        private int artistId;
        private int genreId;
        private Nullable<int> albumId;
        private Nullable<int> trackNumber;
        private string url;
        private Album album;
        private Artist artist;
        private Genre genre;

        //For Ieditable
        private Song temp;

        //Title Comparison
        private static readonly bool ignoreCaseForTitleComparison;

        #endregion
    }
}
