using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCatalogDomainModel
{
    public class SongInfo
    {
        /* Title: Required, 1 copy without album, 1 copy per album
         * TrackLength: Required
         * Rating: null == unrated, ratings 1 to n
         * Url: optional, null == no url
         * Artist: required, use "Unknown" if necessary
         * Genre: required, use "Unknown" if necessary
         * Album: optional, null == no album; no Album, no TrackNumber
         * TrackNumber: optional, null == none; no Tracknumber, no Album
         */
        public const int MAX_TITLE_LENGTH = 100;
        public const int MAX_TRACK_LENGTH = 60 * 60 * 24;
        public const int MAX_RATING = 10;
        public const int MAX_URL_LENGTH = 250;
        public const int MAX_TRACK_NUMBER = 1000;

        public string Title 
        {
            get { return title; }
            set
            {
                if(value != null && value.Length <= MAX_TITLE_LENGTH)
                {
                    title = value;
                }
            }
        }

        //Unit = seconds
        public int TrackLength
        {
            get { return trackLength; }
            set
            {
                if(value > 0 && value <= MAX_TRACK_LENGTH)
                {
                    trackLength = value;
                }
            }
        }

        public int? Rating
        {
            get { return rating; }
            set
            {
                if(value == null) //allow unrated
                {
                    rating = value;
                }
                else if(value > 0 && value <= MAX_RATING)
                {
                    rating = value;
                }
            }
        }

        public string Url
        {
            get { return url; }
            set
            {
                if(value == null)
                {
                    url = value;
                }
                else if(value.Length <= MAX_URL_LENGTH)
                {
                    url = value;
                }
            }
        }

        public ArtistInfo Artist
        {
            get { return artist; }
            set
            {
                if(value != null)
                {
                    artist = value;
                }
            }
        }

        public GenreInfo Genre
        {
            get { return genre; }
            set
            {
                if(value != null)
                {
                    genre = value;
                }
            }
        }

        public AlbumInfo Album
        {
            get { return album; }
            set
            {
                album = value;
                if(value == null)
                {
                    trackNumber = null; //no album: not track in album
                }
            }
        }

        public int? TrackNumber 
        {
            get { return trackNumber; }
            set
            {
                if(value == null)
                {
                    trackNumber = value;
                    album = null; //no track number: no album
                }
                else if(value > 0 && value <= MAX_TRACK_NUMBER)
                {
                    trackNumber = value;
                }
            }
        }

        private string title;
        private int trackLength;
        private int? rating;
        private string url;
        private ArtistInfo artist;
        private GenreInfo genre;
        private AlbumInfo album;
        private int? trackNumber;
    }
}
