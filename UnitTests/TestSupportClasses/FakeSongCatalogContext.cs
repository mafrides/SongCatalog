using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogTests
{
    public class FakeSongCatalogContext : ISongCatalogContext
    {
        public FakeSongCatalogContext()
        {
            Albums = new FakeDbSet<Album>();
            Artists = new FakeDbSet<Artist>();
            Genres = new FakeDbSet<Genre>();
            Songs = new FakeDbSet<Song>();
            addEntitiesForTesting();
        }

        public DbSet<Album> Albums { get; private set; }
        public DbSet<Artist> Artists { get; private set; }
        public DbSet<Genre> Genres { get; private set; }
        public DbSet<Song> Songs { get; private set; }

        //test version: always succeeds
        public int SaveChanges() { return 1; }
        //test version: always succeeds
        public async Task<int> SaveChangesAsync() { return 1; }
        //performs update to FakeDbSet when called
        //throw InvalidOperationException if Id does not exist
        public void SetModified(object entity)
        {
            if(entity is Song)
            {
                Song song = entity as Song;
                Song oldSong = Songs.Find(song.Id);
                if (oldSong == null) 
                    throw new InvalidOperationException("Invalid Update: Song not found.");
                Songs.Remove(oldSong);
                Songs.Add(song);
            }
            else if(entity is Album)
            {
                Album album = entity as Album;
                Album oldAlbum = Albums.Find(album.Id);
                if (oldAlbum == null)
                    throw new InvalidOperationException("Invalid Update: Album not found.");
                Albums.Remove(oldAlbum);
                Albums.Add(album);
            }
            else if(entity is Artist)
            {
                Artist artist = entity as Artist;
                Artist oldArtist = Artists.Find(artist.Id);
                if (oldArtist == null)
                    throw new InvalidOperationException("Invalid Update: Artist not found.");
                Artists.Remove(oldArtist);
                Artists.Add(artist);
            }
            else if(entity is Genre)
            {
                Genre genre = entity as Genre;
                Genre oldGenre = Genres.Find(genre.Id);
                if (oldGenre == null)
                    throw new InvalidOperationException("Invalid Update: Genre not found.");
                Genres.Remove(oldGenre);
                Genres.Add(genre);
            }
            throw new InvalidOperationException("Invalid Update: Type not in FakeSongCatalogContext.");
        }

        //performs insert to FakeDbSet when called
        //throw InvalidOperationException if duplicate Id
        public void SetAdded(object entity)
        {
            if (entity is Song)
            {
                Song song = entity as Song;
                if (Songs.Any(s => s.Id == song.Id))
                    throw new InvalidOperationException("Invalid Insert: duplicate Song.");
                Songs.Add(song);
            }
            else if (entity is Album)
            {
                Album album = entity as Album;
                if(Albums.Any(a => a.Id == album.Id))
                    throw new InvalidOperationException("Invalid Insert: duplicate Album.");
                Albums.Add(album);
            }
            else if (entity is Artist)
            {
                Artist artist = entity as Artist;
                if(Artists.Any(a => a.Id == artist.Id))
                    throw new InvalidOperationException("Invalid Insert: duplicate Artist.");
                Artists.Add(artist);
            }
            else if (entity is Genre)
            {
                Genre genre = entity as Genre;
                if(Genres.Any(g => g.Id == genre.Id))
                    throw new InvalidOperationException("Invalid Insert: duplicate Genre.");
                Genres.Add(genre);
            }
            throw new InvalidOperationException("Invalid Insert: Type not in FakeSongCatalogContext.");
        }

        //performs delete from FakeDbSet when called
        //throw InvalidOperationException if Id does not exist
        public void SetDeleted(object entity)
        {
            if (entity is Song)
            {
                Song song = entity as Song;
                Song oldSong = Songs.Find(song.Id);
                if (oldSong == null)
                    throw new InvalidOperationException("Invalid Delete: Song not found.");
                Songs.Remove(oldSong);
            }
            else if (entity is Album)
            {
                Album album = entity as Album;
                Album oldAlbum = Albums.Find(album.Id);
                if (oldAlbum == null)
                    throw new InvalidOperationException("Invalid Delete: Album not found.");
                Albums.Remove(oldAlbum);
            }
            else if (entity is Artist)
            {
                Artist artist = entity as Artist;
                Artist oldArtist = Artists.Find(artist.Id);
                if (oldArtist == null)
                    throw new InvalidOperationException("Invalid Delete: Artist not found.");
                Artists.Remove(oldArtist);
            }
            else if (entity is Genre)
            {
                Genre genre = entity as Genre;
                Genre oldGenre = Genres.Find(genre.Id);
                if (oldGenre == null)
                    throw new InvalidOperationException("Invalid Delete: Genre not found.");
                Genres.Remove(oldGenre);
            }
            throw new InvalidOperationException("Invalid Delete: Type not in FakeSongCatalogContext.");
        }

        public void Dispose() { }

        //Since testing disconnected repo, need something with an Id
        //in each collection
        //
        //FakeSongCatalogContext Test Set:
        //Artist1.Songs = song1, song1a; Artist1.Albums = album1
        //Artist2.Songs = song2; Artist2.Albums album2
        //Genre1.Songs = song1, song1a; Genre1.Albums = album1
        //Genre2.Songs = song2; Genre2.Albums = album2
        //Album1.Songs = song1a, song2
        //song1 has no album
        //song1, song1a, album2 have same title = "Song1"
        private void addEntitiesForTesting()
        {
            Artist artist1 = new Artist
            {
                Id = 1,
                Name = "Artist1",
                Songs = new List<Song>(),
                Albums = new List<Album>()
            };
            Artist artist2 = new Artist
            {
                Id = 2,
                Name = "Artist2",
                Songs = new List<Song>(),
                Albums = new List<Album>()
            };
            Genre genre1 = new Genre
            {
                Id = 1,
                Name = "Genre1",
                Songs = new List<Song>(),
                Albums = new List<Album>()
            };
            Genre genre2 = new Genre
            {
                Id = 2,
                Name = "Genre2",
                Songs = new List<Song>(),
                Albums = new List<Album>()
            };
            Song song1 = new Song
            {
                Id = 1,
                Title = "Song1",
                TrackLength = 60,
                TrackNumber = null,
                AlbumId = null,
                Album = null,
                ArtistId = 1,
                Artist = artist1,
                GenreId = 1,
                Genre = genre1,
                Rating = null,
                Url = null
            };
            Song song1a = new Song
            {
                Id = 2,
                Title = "Song1",
                TrackLength = 60,
                TrackNumber = 1,
                AlbumId = 1,
                ArtistId = 1,
                Artist = artist1,
                GenreId = 1,
                Genre = genre1,
                Rating = null,
                Url = null
            };
            Song song2 = new Song
            {
                Id = 3,
                Title = "Song2",
                TrackLength = 60,
                TrackNumber = 2,
                AlbumId = 1,
                ArtistId = 2,
                Artist = artist2,
                GenreId = 2,
                Genre = genre2,
                Rating = null,
                Url = null
            };
            Album album1 = new Album
            {
                Id = 1,
                Title = "Album1",
                ArtistId = 1,
                Artist = artist1,
                GenreId = 1,
                Genre = genre1,
                Rating = null,
                Year = 1999,
                Songs = new List<Song>()
            };
            Album album2 = new Album
            {
                Id = 2,
                Title = "Song1",
                ArtistId = 2,
                Artist = artist2,
                GenreId = 2,
                Genre = genre2,
                Rating = null,
                Year = 2000,
                Songs = new List<Song>()
            };

            song1a.Album = album1;
            song2.Album = album1;
            album1.Songs.Add(song1a);
            album1.Songs.Add(song2);
            artist1.Songs.Add(song1);
            artist1.Songs.Add(song1a);
            artist2.Songs.Add(song2);
            artist1.Albums.Add(album1);
            artist2.Albums.Add(album2);
            genre1.Songs.Add(song1);
            genre1.Songs.Add(song1a);
            genre2.Songs.Add(song2);
            genre1.Albums.Add(album1);
            genre2.Albums.Add(album2);
        }
    }
}
