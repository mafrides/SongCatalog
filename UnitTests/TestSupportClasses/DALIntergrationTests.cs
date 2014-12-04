using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CDCatalogDAL;
using CDCatalogModel;
using System.Data.Entity;

namespace CDCatalogTests
{
    [TestClass]
    class DALIntergrationTests
    {
        private static ICDCatalog catalog; 

        [ClassInitialize]
        public static void classInitializer(TestContext context)
        {
            catalog = new CDCatalogEFDisconnected();
            //RecreateDatabaseForTesting();
            throw new NotImplementedException();
        }

        #region Helper Methods

        private static int uniqueTitleCounter = 0;

        //Boring titles, easy to do
        private static string getUniqueTitle()
        {
            String title = "";
            int counter = uniqueTitleCounter;
            while(counter >= 0)
            {
                char nextChar = (char)(counter % 26 + (int)'a');
                title += nextChar;
                counter -= 26;
            }
            ++uniqueTitleCounter;
            return title;
        }

        private void RecreateDatabaseForTesting()
        {
            //need to be able to recreate indexes here
            //for testing only, should be disabled in production
            Database.SetInitializer(new DropCreateDatabaseAlways<SongCatalogContext>());
            using(var context = new SongCatalogContext())
            {
                try { context.Database.Initialize(true); }
                catch (Exception ex)
                { 
                    throw ex;
                }
            }
        }

        //value equal for testing
        private static bool songsEqual(Song a, Song b)
        {
            return (a.Id == default(int) || b.Id == default(int) || a.Id == b.Id)
                && (a.Title == b.Title)
                && (a.Rating == b.Rating)
                && (a.TrackLength == b.TrackLength)
                && (a.TrackNumber == b.TrackNumber)
                && (a.Url == b.Url)
                && (a.ArtistId == b.ArtistId
                    || (a.Artist != null && b.Artist != null
                        && (a.Artist.Id == b.Artist.Id
                            || a.Artist.Name == b.Artist.Name)))
                && (a.GenreId == b.GenreId
                    || (a.Genre != null && b.Genre != null
                        && (a.Genre.Id == b.Genre.Id
                            || a.Genre.Name == b.Genre.Name)))
                && (a.AlbumId == b.AlbumId
                    || (a.Album != null && b.Album != null
                        && (a.Album.Id == b.Album.Id
                            || a.Album.Title == b.Album.Title)));
        }

        //value equal for testing
        private static bool albumsEqual(Album a, Album b)
        {
            return (a.Id == default(int) || b.Id == default(int) || a.Id == b.Id)
                && (a.Title == b.Title)
                && (a.Year == b.Year)
                && (a.Rating == b.Rating)
                && (a.ArtistId == b.ArtistId
                    || (a.Artist != null && b.Artist != null
                        && (a.Artist.Id == b.Artist.Id
                            || a.Artist.Name == b.Artist.Name)))
                && (a.GenreId == b.GenreId
                    || (a.Genre != null && b.Genre != null
                        && (a.Genre.Id == b.Genre.Id
                            || a.Genre.Name == b.Genre.Name))); 
        }

        //value equal for testing
        private static bool artistsEqual(Artist a, Artist b)
        {
            return (a.Id == default(int) || b.Id == default(int) || a.Id == b.Id)
                && (a.Name == b.Name);
        }

        //value equal for testing
        private static bool genresEqual(Genre a, Genre b)
        {
            return (a.Id == default(int) || b.Id == default(int) || a.Id == b.Id)
                && (a.Name == b.Name);
        }

        private static Song getValidSong()
        {
            return new Song
            {
                Id = default(int),
                Title = getUniqueTitle(),
                Rating = null,
                TrackLength = 60,
                Artist = new Artist { Name = "a" },
                ArtistId = default(int),
                Genre = new Genre { Name = "a" },
                GenreId = default(int),
                Album = null,
                AlbumId = null,
                TrackNumber = null,
                Url = null
            };
        }

        private static Album getValidAlbum()
        {
            return new Album
            {
                Id = default(int),
                Title = "My Favorite Album",
                Rating = null,
                Artist = new Artist { Name = "My Favorite Artist" },
                ArtistId = default(int),
                Genre = new Genre { Name = "My Favorite Genre" },
                GenreId = default(int),
                Year = 1999,
                Songs = new List<Song>()
            };
        }

        #endregion

        #region Helper Method Tests

        [TestMethod]
        public void getValidSongReturnsValidSong()
        {
            Assert.IsTrue(getValidSong().IsValid);
        }

        [TestMethod]
        public void getValidAlbumReturnsValidAlbum()
        {
            Assert.IsTrue(getValidAlbum().IsValid);
        }

        #endregion

        
    }
}
