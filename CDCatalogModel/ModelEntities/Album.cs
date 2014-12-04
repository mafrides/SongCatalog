using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CDCatalogModel
{
    public partial class Album : INotifyPropertyChanged, IEditableObject, IComparable<Album>,
        IAlbumOrSong, IHasId, IHasTitle, IHasArtist, IHasGenre, IRated, IHasDisplayTitle, IHasDisplayTrackLength, IHasDisplayRating
    {
        static Album()
        {
            ignoreCaseForTitleComparison = true;
            defaultStringComparison = StringComparison.CurrentCultureIgnoreCase;
        }

        public Album()
        {
            this.Songs = new List<Song>();
            
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
        public int Year
        {
            get { return year; }
            set
            {
                if(year != value)
                {
                    year = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Year"));
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
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
        public ICollection<Song> Songs { get; set; }
    
        public string DisplayTitle
        {
            get { return this.ToString(); }
        }
        public string DisplayTrackLength
        {
            get
            {
                if (Songs == null || Songs.Count == 0) return null;
                int length = 0;
                foreach (var song in Songs) length += song.TrackLength;
                int seconds = length % 60;
                length /= 60;
                int minutes = length % 60;
                int hours = length / 60;
                StringBuilder sb = new StringBuilder();
                if(hours > 0)
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
            get { return Rating == null ? null : (Nullable<double>)(Rating / 2); }
        }
        //Application Specific Validation
        //Valid Id or 0 (unset, as when inserting a new Song)
        //Required Title
        //Required Year
        //Rating 1-10 or unrated
        //Required Artist
        //Required Genre
        public bool IsValid
        {
            get
            {
                return (Id >= 0)
                    && (!String.IsNullOrEmpty(Title))
                    && (Year >= 1600 && Year <= DateTime.Now.Year)
                    && (Rating == null || (Rating >= 1 && Rating <= 10))
                    && ArtistId >= 0 
                    && (Artist == null || Artist.IsValid)
                    && (ArtistId > 0 || (Artist != null && Artist.IsValid))
                    && ((ArtistId > 0 && Artist != null && Artist.IsValid && Artist.Id > 0) ? Artist.Id == ArtistId : true)
                    && GenreId >= 0 
                    && (Genre == null || Genre.IsValid)
                    && (GenreId > 0 || (Genre != null && Genre.IsValid))
                    && ((GenreId > 0 && Genre != null && Genre.IsValid && Genre.Id > 0) ? Genre.Id == GenreId : true);
            }
        }

        //Equality as a search term, by Id or Title
        public static bool operator==(Album a1, Album a2)
        {
            return ReferenceEquals(a1, a2)
                || ((object)a1 != null && a1.Equals(a2));
        }
        public static bool operator!=(Album a1, Album a2)
        {
            return !(a1 == a2);
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Album a = obj as Album;
            return this.Equals(a);
        }
        public bool Equals(Album a)
        {
            return (object)a != null
                && (Id == a.Id || String.Equals(Title, a.Title, defaultStringComparison));             
        }

        //Hash by Title
        public override int GetHashCode()
        {
            return Title != null ? Title.GetHashCode() : base.GetHashCode();
        }

        //Display by Title
        public override string ToString()
        {
            return String.IsNullOrEmpty(Title) ? "" 
                : Year == default(int) ? Title
                : String.Concat(Title, " (", Year, ")");
        }

        //IComparable: Sort by Title
        public int CompareTo(Album other)
        {
            return Title == null ? 1
                : (other == null || other.Title == null) ? -1
                : String.Compare(Title, other.Title, ignoreCaseForTitleComparison);
        }

        //INotifyProperyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //IEditableObject
        public void BeginEdit()
        {
            temp = new Album
            {
                Id = this.id,
                Title = String.Copy(this.title),
                Year = this.year,
                Rating = this.rating,
                ArtistId = this.artistId,
                GenreId = this.genreId,
            };
            temp.Artist = this.artist == null ? null 
                : new Artist { Id = this.artist.Id, Name = String.Copy(this.artist.Name) };
            temp.Genre = this.genre == null ? null
                : new Genre { Id = this.genre.Id, Name = String.Copy(this.genre.Name) };
            temp.Songs = this.Songs;
        }
        public void CancelEdit()
        {
            if (temp == null) return;
            this.Id = temp.id;
            this.Title = temp.title;
            this.Year = temp.year;
            this.Rating = temp.rating;
            this.ArtistId = temp.artistId;
            this.GenreId = temp.genreId;
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
            this.Songs = temp.Songs;
        }
        public void EndEdit()
        {
            temp = null;
        }

        #region Fields

        //Backing Fields
        private int id;
        private string title;
        private int year;
        private Nullable<int> rating;
        private int artistId;
        private int genreId;
        private Artist artist;
        private Genre genre;

        //For IEditableObject
        private Album temp;

        //Title Comparison
        private static readonly bool ignoreCaseForTitleComparison;
        private static readonly StringComparison defaultStringComparison;

        #endregion
    }
}