using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using CDCatalogModel;

namespace CDCatalogDAL
{
    public class CDCatalogEFDisconnected : CDCatalogEFDisconnected<SongCatalogContext> { }

    public class CDCatalogEFDisconnected<TContext> : ICDCatalog
        where TContext : ISongCatalogContext, new()
    {
        static CDCatalogEFDisconnected()
        {
            //For String Comparison
            //Removed these from all EF Linq calls, as some didn't work
            stringComparer = StringComparer.CurrentCultureIgnoreCase;
            stringComparison = StringComparison.CurrentCultureIgnoreCase;
            stringEqualityComparer = StringComparer.CurrentCultureIgnoreCase;

            //Playlist Values
            minPlaylistTimeMinutes = 1;
            maxPlaylistTimeMinutes = 300;
            averageSongLengthMinutes = 2.5; //Assumption, but could query this
            predictedNumberSongsNeededMultiplier = 5.0;
            minQueriedCount = 100;
            maxQueriedCount = 1500;
            maxPlaylistDeltaMinutes = 1;
            minIterations = 300;
            maxIterationsMultiplier = 2.0;
        }
        
        #region Count

        public async Task<int> getSongCountAsync()
        {
            using(var context = new TContext())
            {
                try
                {
                    return await context.Songs.CountAsync();
                }
                catch(Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<int> getAlbumCountAsync()
        {
            using (var context = new TContext())
            {
                try
                {
                    return await context.Albums.CountAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<int> getArtistCountAsync()
        {
            using(var context = new TContext())
            {
                try
                {
                    return await context.Artists.CountAsync();
                }
                catch(Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<int> getGenreCountAsync()
        {
            using (var context = new TContext())
            {
                try
                {
                    return await context.Genres.CountAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }

        #endregion

        #region Create

        public async Task<Song> insertSongAsync(Song song)
        {
            if (song == null || 
                song.Id != default(int) || 
                !song.IsValid)
            {
                return null;
            }

            using (var context = new TContext())
            {
                try
                {
                    if (!(await handleArtistAsync(context, song)) ||
                        !(await handleGenreAsync(context, song)) ||
                        !(await handleSongForInsertAlbumAndSetTrackNumberIfUnsetAsync(context, song)) ||
                         (await context.Songs.AnyAsync(s =>
                            String.Equals(s.Title.ToLower(), song.Title.ToLower()) && s.AlbumId == song.AlbumId)))
                    {
                        return null;
                    }
                    
                    context.SetAdded(song);

                    if (await context.SaveChangesAsync() == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return song;
                    }      
                }
                catch(Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }
        public async Task<Album> insertAlbumAsync(Album album)
        {
            if (album == null || 
                album.Id != default(int) || 
                !album.IsValid)
            {
                return null;
            }

            using (var context = new TContext())
            {
                try
                {
                    if (!(await handleArtistAsync(context, album)) ||
                        !(await handleGenreAsync(context, album)) ||
                         (await context.Albums.AnyAsync(a => String.Equals(a.Title.ToLower(), album.Title.ToLower()))))
                    {
                        return null;
                    }
                    
                    context.SetAdded(album);

                    if (await context.SaveChangesAsync() <= 0)
                    {
                        return null;
                    }
                    else
                    {
                        return album;
                    }
                }
                catch(Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }

        #endregion

        #region Read

        public async Task<List<Song>> createPlaylistAsync(int minutes)
        {
            if (minutes <= 0)
            {
                return new List<Song>();
            }

            //clamp minutes
            minutes = minutes > maxPlaylistTimeMinutes ? maxPlaylistTimeMinutes 
                : minutes < minPlaylistTimeMinutes ? minPlaylistTimeMinutes 
                : minutes; 

            //calculate number songs to retrieve
            int queriedCount = (int)(minutes / averageSongLengthMinutes * predictedNumberSongsNeededMultiplier + 1);
            queriedCount = queriedCount > maxQueriedCount ? maxQueriedCount 
                : queriedCount < minQueriedCount ? minQueriedCount 
                : queriedCount;
            int maxSongLengthSeconds = minutes * 60 + maxPlaylistDeltaSeconds;

            //retrieve sample set from db
            List<Song> randomSongs;
            using (var context = new TContext())
            {
                try
                {
                    //Should be okay for a few thousand rows, but
                    //Switch to this solution if our tables get large:
                    //http://msdn.microsoft.com/en-us/library/cc441928.aspx
                    randomSongs = await context.Songs
                        .Include(s => s.Genre)
                        .Include(s => s.Artist)
                        .Where(s => s.TrackLength < maxSongLengthSeconds)
                        .OrderBy(s => Guid.NewGuid())
                        .Take(queriedCount)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }

            //Build random playlist from sample set 
            //(since already randomized from db, don't need to shuffle)
            List<Song> playlist = new List<Song>();
            int playlistLengthSeconds = 0;
            int targetPlaylistLengthSeconds = minutes * 60;
            foreach(var song in randomSongs)
            {
                playlist.Add(song);
                playlistLengthSeconds += song.TrackLength;
                if (playlistAccepted(playlistLengthSeconds, targetPlaylistLengthSeconds))
                {
                    return playlist;
                }
                if (playlistTooLong(playlistLengthSeconds, targetPlaylistLengthSeconds))
                {
                    break;
                }
            }

            //Remove so no duplicates
            foreach (var song in playlist) randomSongs.Remove(song);

            //Refine Playlist
            Random rand = new Random();
            int iterations = (int)(randomSongs.Count() * maxIterationsMultiplier + 1);
            iterations = iterations < minIterations ? minIterations : iterations;
            Song songToRemove;
            Song songToAdd;
            int secondsOverTarget;
            bool over;
            int secondsToTarget;
            int maxSongLengthToRemoveSeconds;
            int maxSongLengthToAddSeconds;
            while(iterations > 0 && !playlistAccepted(playlistLengthSeconds, targetPlaylistLengthSeconds))
            {
                secondsOverTarget = playlistLengthSeconds - targetPlaylistLengthSeconds;
                over = secondsOverTarget > 0;
                secondsToTarget = Math.Abs(secondsOverTarget);
                maxSongLengthToRemoveSeconds = 2 * secondsToTarget - 60;
                maxSongLengthToAddSeconds = 2 * secondsToTarget - 60;

                if(randomSongs.Count() == 0)
                {
                    if (!over)
                    {
                        return playlist;
                    }

                    songToRemove = playlist.FirstOrDefault(s => s.TrackLength < maxSongLengthToRemoveSeconds);
                    if (songToRemove == default(Song))
                    {
                        return playlist;
                    }

                    playlist.Remove(songToRemove);
                    playlistLengthSeconds -= songToRemove.TrackLength;
                    randomSongs.Add(songToRemove);
                    continue;
                }

                if(over)
                {
                    songToRemove = playlist.FirstOrDefault(s => s.TrackLength < maxSongLengthToRemoveSeconds);
                    if (songToRemove == default(Song))
                    {
                        return playlist;
                    }
                    playlist.Remove(songToRemove);
                    playlistLengthSeconds -= songToRemove.TrackLength;
                    randomSongs.Add(songToRemove);
                }
                else if(!over)
                {
                    songToAdd = randomSongs.FirstOrDefault(s => s.TrackLength < maxSongLengthToAddSeconds);
                    if (songToAdd == default(Song))
                    {
                        return playlist;
                    }
                    playlist.Add(songToAdd);
                    playlistLengthSeconds += songToAdd.TrackLength;
                    randomSongs.Remove(songToAdd);
                }
            }
            //we've either met the acceptance criteria, or come as close as possible with the sample set
            return playlist;
        }

        public async Task<List<Song>> getSongsAsync(int skip = 0, int take = 300)
        {
            if (skip < 0 ||
                take <= 0)
            {
                return new List<Song>();
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Songs
                        .Include(s => s.Genre)
                        .Include(s => s.Artist)
                        .Include(s => s.Album)
                        .OrderBy(s => s.Title.ToLower())
                        .ThenBy(s => s.Album.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<Album>> getAlbumsWithSongsAsync(int skip = 0, int take = 300)
        {
            if (skip < 0 ||
                take <= 0)
            {
                return new List<Album>();
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Albums
                        .Include(a => a.Songs)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .OrderBy(a => a.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<IAlbumOrSong>> getAlbumsAndSongsAsync(int skip = 0, int take = 300)
        {
            if (skip < 0 ||
                take <= 0)
            {
                return new List<IAlbumOrSong>();
            }

            using (var context = new TContext())
            {
                try
                {
                    List<Song> listSongs = await context.Songs
                        .Include(s => s.Genre)
                        .Include(s => s.Artist)
                        .Include(s => s.Album)
                        .OrderBy(o => o.Title.ToLower())
                        .Take(take + skip)
                        .ToListAsync();

                    List<Album> listAlbums = await context.Albums
                        .Include(a => a.Genre)
                        .Include(a => a.Artist)
                        .Include(a => a.Songs)
                        .OrderBy(o => o.Title.ToLower())
                        .Take(take + skip)
                        .ToListAsync();

                    return listSongs.Cast<IAlbumOrSong>().Concat(listAlbums.Cast<IAlbumOrSong>())
                        .OrderBy(o => o.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToList();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<Album>> getAlbumsAsync(int skip = 0, int take = 300)
        {
            if (skip < 0 ||
                take <= 0)
            {
                return new List<Album>();
            }

            using(var context = new TContext())
            {
                try
                {
                    return await context.Albums
                        .OrderBy(a => a.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch(Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<Artist>> getArtistsAsync(int skip = 0, int take = 300)
        {
            if (skip < 0 ||
                take <= 0)
            {
                return new List<Artist>();
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Artists
                        .OrderBy(a => a.Name.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<Genre>> getGenresAsync(int skip = 0, int take = 300)
        {
            if (skip < 0 ||
                take <= 0)
            {
                return new List<Genre>();
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Genres
                        .OrderBy(g => g.Name.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }

        public async Task<Song> findSongAsync(int Id)
        {
            if (Id <= 0)
            {
                return null;
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Songs
                        .Include(s => s.Genre)
                        .Include(s => s.Artist)
                        .Include(s => s.Album)
                        .FirstOrDefaultAsync(s => s.Id == Id);
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<Album> findAlbumAsync(int Id)
        {
            if (Id <= 0)
            {
                return null;
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Albums
                        .Include(a => a.Songs)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .FirstOrDefaultAsync(a => a.Id == Id);
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<Artist> findArtistAsync(int Id)
        {
            if (Id <= 0)
            {
                return null;
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Artists.FindAsync(Id);
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<Genre> findGenreAsync(int Id)
        {
            if (Id <= 0)
            {
                return null;
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Genres.FindAsync(Id);
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }

        public async Task<List<Song>> findSongsAsync(string title, int skip = 0, int take = 300)
        {
            if (String.IsNullOrEmpty(title) ||
                skip < 0 ||
                take <= 0)
            {
                return new List<Song>();
            }

            string titleLower = title.ToLower();
            using (var context = new TContext())
            {
                try
                {
                    return await context.Songs
                        .Include(s => s.Artist)
                        .Include(s => s.Genre)
                        .Include(s => s.Album)
                        .Where(s => s.Title.ToLower().Contains(titleLower))
                        .OrderBy(s => s.Title.ToLower())
                        .ThenBy(s => s.Album.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<Album>> findAlbumsAsync(string title, int skip = 0, int take = 300)
        {
            if (String.IsNullOrEmpty(title) ||
                skip < 0 ||
                take <= 0)
            {
                return new List<Album>();
            }

            string titleLower = title.ToLower();
            using (var context = new TContext())
            {
                try
                {
                    return await context.Albums
                        .Include(a => a.Songs)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .Where(a => a.Title.ToLower().Contains(titleLower))
                        .OrderBy(a => a.Title.ToLower())
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<IAlbumOrSong>> findAlbumsAndSongsAsync(string title, int skip = 0, int take = 300)
        {
            if (String.IsNullOrEmpty(title) ||
                skip < 0 ||
                take <= 0)
            {
                return new List<IAlbumOrSong>();
            }

            string titleLower = title.ToLower();
            using (var context = new TContext())
            {
                try
                {
                    List<Song> listSongs = await context.Songs
                         .Include(s => s.Artist)
                         .Include(s => s.Genre)
                         .Include(s => s.Album)
                         .Where(s => s.Title.ToLower().Contains(titleLower))
                         .OrderBy(a => a.Title.ToLower())
                         .Take(take + skip)
                         .ToListAsync();

                    List<Album> listAlbums = await context.Albums
                        .Include(a => a.Songs)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .Where(a => a.Title.ToLower().Contains(titleLower))
                        .OrderBy(a => a.Title.ToLower())
                        .Take(take + skip)
                        .ToListAsync();

                    return listSongs.Cast<IAlbumOrSong>().Concat(listAlbums.Cast<IAlbumOrSong>())
                        .OrderBy(a => a.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToList();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }

        public async Task<List<Song>> findSongsAsync(Artist artist, int skip = 0, int take = 300)
        {
            if (artist == null ||
                !artist.IsValid ||
                skip < 0 ||
                take <= 0)
            {
                return new List<Song>();
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Songs
                        .Include(s => s.Artist)
                        .Include(s => s.Genre)
                        .Include(s => s.Album)
                        .Where(s => s.Artist.Name.ToLower() == artist.Name.ToLower())
                        .OrderBy(s => s.Title.ToLower())
                        .ThenBy(s => s.Album.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<Album>> findAlbumsAsync(Artist artist, int skip = 0, int take = 300)
        {
            if (artist == null ||
                !artist.IsValid ||
                skip < 0 ||
                take <= 0)
            {
                return new List<Album>();
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Albums
                        .Include(a => a.Songs)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .Where(a => a.Artist.Name.ToLower() == artist.Name.ToLower())
                        .OrderBy(a => a.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<IAlbumOrSong>> findAlbumsAndSongsAsync(Artist artist, int skip = 0, int take = 300)
        {
            if (artist == null ||
                !artist.IsValid ||
                skip < 0 ||
                take <= 0)
            {
                return new List<IAlbumOrSong>();
            }

            using (var context = new TContext())
            {
                try
                {
                    List<Song> listSongs = await context.Songs
                        .Include(s => s.Artist)
                        .Include(s => s.Genre)
                        .Include(s => s.Album)
                        .Where(s => s.Artist.Name.ToLower() == artist.Name.ToLower())
                        .OrderBy(a => a.Title.ToLower())
                        .Take(take + skip)
                        .ToListAsync();

                    List<Album> listAlbums = await context.Albums
                        .Include(a => a.Songs)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .Where(a => a.Artist.Name.ToLower() == artist.Name.ToLower())
                        .OrderBy(a => a.Title.ToLower())
                        .Take(take + skip)
                        .ToListAsync();

                    return listSongs.Cast<IAlbumOrSong>().Concat(listAlbums.Cast<IAlbumOrSong>())
                        .OrderBy(a => a.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToList();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }

        public async Task<List<Song>> findSongsAsync(Genre genre, int skip = 0, int take = 300)
        {
            if (genre == null ||
                !genre.IsValid ||
                skip < 0 ||
                take <= 0)
            {
                return new List<Song>();
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Songs
                        .Include(s => s.Artist)
                        .Include(s => s.Genre)
                        .Include(s => s.Album)
                        .Where(s => s.Genre.Name.ToLower() == genre.Name.ToLower())
                        .OrderBy(s => s.Title.ToLower())
                        .ThenBy(s => s.Album.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<Album>> findAlbumsAsync(Genre genre, int skip = 0, int take = 300)
        {
            if (genre == null ||
                !genre.IsValid ||
                skip < 0 ||
                take <= 0)
            {
                return new List<Album>();
            }

            using (var context = new TContext())
            {
                try
                {
                    return await context.Albums
                        .Include(a => a.Songs)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .Where(a => a.Genre.Name.ToLower() == genre.Name.ToLower())
                        .OrderBy(a => a.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }
        public async Task<List<IAlbumOrSong>> findAlbumsAndSongsAsync(Genre genre, int skip = 0, int take = 300)
        {
            if (genre == null ||
                !genre.IsValid ||
                skip < 0 ||
                take <= 0)
            {
                return new List<IAlbumOrSong>();
            }

            using (var context = new TContext())
            {
                try
                {
                    List<Song> listSongs = await context.Songs
                        .Include(s => s.Artist)
                        .Include(s => s.Genre)
                        .Include(s => s.Album)
                        .Where(s => s.Genre.Name.ToLower() == genre.Name.ToLower())
                        .OrderBy(a => a.Title.ToLower())
                        .Take(take + skip)
                        .ToListAsync();

                    List<Album> listAlbums = await context.Albums
                        .Include(a => a.Songs)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .Where(a => a.Genre.Name.ToLower() == genre.Name.ToLower())
                        .OrderBy(a => a.Title.ToLower())
                        .Take(take + skip)
                        .ToListAsync();

                    return listSongs.Cast<IAlbumOrSong>().Concat(listAlbums.Cast<IAlbumOrSong>())
                        .OrderBy(a => a.Title.ToLower())
                        .Skip(skip)
                        .Take(take)
                        .ToList();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALReadException(ex);
                }
            }
        }

        #endregion

        #region Update

        public async Task<Song> updateSongAsync(Song song)
        {
            if (song == null ||
                song.Id == default(int) ||
                !song.IsValid)
            {
                return null;
            }

            using (var context = new TContext())
            {
                try
                {
                    var existingSong = await context.Songs.FindAsync(song.Id);
                    if (existingSong == null)
                    {
                        return null;
                    }

                    if (!(await handleArtistAsync(context, song)) ||
                        !(await handleGenreAsync(context, song)) ||
                        !(await handleSongForUpdateAlbumAndSetTrackNumbersIfUnsetAsync(context, song)))
                    {
                        return null;
                    }

                    if (!String.Equals(existingSong.Title.ToLower(), song.Title.ToLower()) ||
                        existingSong.AlbumId != song.AlbumId)
                    {
                        if (!(await handleSongForUpdateTitleAndAlbumUpdatesAsync(context, song)))
                        {
                            return null;
                        }
                    }

                    context.SetModified(song);

                    if (await context.SaveChangesAsync() == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return song;
                    }
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }
        public async Task<Album> updateAlbumAsync(Album album)
        {
            if (album == null ||
                album.Id == default(int) ||
                !album.IsValid)
            {
                return null;
            }

            using (var context = new TContext())
            {
                try
                {
                    var existingAlbum = await context.Albums.FindAsync(album.Id);
                    if (existingAlbum == null)
                    {
                        return null;
                    }

                    if (!String.Equals(existingAlbum.Title.ToLower(), album.Title.ToLower()))
                    {
                        if (!(await handAlbumForUpdateTitleUpdatesAsync(context, album)))
                        {
                            return null;
                        }
                    }

                    if (!(await handleArtistAsync(context, album)) ||
                        !(await handleGenreAsync(context, album)))
                    {
                        return null;
                    }

                    context.SetModified(album);

                    if (await context.SaveChangesAsync() <= 0)
                    {
                        return null;
                    }
                    else
                    {
                        return album;
                    }
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }

        #endregion

        #region Delete

        public async Task removeSongAsync(Song song)
        {
            if (song == null ||
                song.Id <= 0)
            {
                return;
            }

            using (var context = new TContext())
            {
                try
                {
                    song = await context.Songs.FindAsync(song.Id);
                    if (song == null)
                    {
                        return;
                    }

                    context.Songs.Remove(song);
                    await context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }
        public async Task removeSongsAsync(List<Song> songs)
        {
            if (songs == null)
            {
                return;
            }

            foreach (var song in songs)
            {
                if (song.Id <= 0)
                {
                    songs.Remove(song);
                }
            }

            if (songs.Count == 0)
            {
                return;
            }

            using (var context = new TContext())
            {
                try
                {
                    songs = await context.Songs
                        .Where(s => songs.Any(i => i.Id == s.Id))
                        .ToListAsync();
                    context.Songs.RemoveRange(songs);
                    await context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }
        public async Task removeAlbumAsync(Album album)
        {
            if (album == null ||
                album.Id <= 0)
            {
                return;
            }

            using (var context = new TContext())
            {
                try
                {
                    album = await context.Albums
                        .Include(a => a.Songs)
                        .FirstOrDefaultAsync(a => a.Id == album.Id);
                    if (album == default(Album))
                    {
                        return;
                    }

                    if (album.Songs != null)
                    {
                        foreach (var song in album.Songs)
                        {
                            //if there is a loose (no album) song with matching title, delete the duplicate
                            if (await context.Songs.AnyAsync(s => 
                                    s.AlbumId == null &&
                                    s.Title == song.Title))
                            {
                                context.SetDeleted(song);
                            }
                            //else cut the song loose
                            else
                            {
                                song.AlbumId = null;
                                song.Album = null;
                                song.TrackNumber = null;
                                context.SetModified(song);
                            }
                        }
                    }

                    context.SetDeleted(album);
                    await context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }
        public async Task removeAlbumWithSongsAsync(Album album)
        {
            if (album == null ||
                album.Id <= 0)
            {
                return;
            }

            using (var context = new TContext())
            {
                try
                {
                    album = await context.Albums
                        .Include(a => a.Songs)
                        .FirstOrDefaultAsync(a => a.Id == album.Id);
                    if (album == default(Album))
                    {
                        return;
                    }

                    if (album.Songs != null &&
                        album.Songs.Count > 0)
                    {
                        context.Songs.RemoveRange(album.Songs);
                    }

                    context.Albums.Remove(album);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }
        public async Task removeAlbumsWithSongsAsync(List<Album> albums)
        {
            if (albums == null)
            {
                return;
            }

            foreach (var album in albums)
            {
                if (album.Id <= 0)
                {
                    albums.Remove(album);
                }
            }

            if (albums.Count == 0)
            {
                return;
            }

            using (var context = new TContext())
            {
                try
                {
                    albums = await context.Albums
                        .Include(a => a.Songs)
                        .Where(a => albums.Any(i => i.Id == a.Id))
                        .ToListAsync();
                    if (albums.Count == 0)
                    {
                        return;
                    }

                    foreach (var album in albums)
                    {
                        if (album.Songs != null &&
                            album.Songs.Count > 0)
                        {
                            context.Songs.RemoveRange(album.Songs);
                        }
                    }

                    context.Albums.RemoveRange(albums);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }

        public async Task removeAlbumsWithoutSongsAsync()
        {
            using (var context = new TContext())
            {
                try
                {
                    var orphanedAlbums = await context.Albums
                        .Include(a => a.Songs)
                        .Where(a => a.Songs == null ||
                               a.Songs.Count() == 0)
                        .ToListAsync();
                    if (orphanedAlbums.Count == 0)
                    {
                        return;
                    }

                    context.Albums.RemoveRange(orphanedAlbums);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }
        public async Task removeArtistsWithoutSongsAsync()
        {
            using (var context = new TContext())
            {
                try
                {
                    var orphanedArtists = await context.Artists
                        .Include(a => a.Songs)
                        .Include(a => a.Albums)
                        .Where(a => (a.Songs == null ||
                                     a.Songs.Count() == 0) &&
                                    (a.Albums == null ||
                                     a.Albums.Count() == 0))
                        .ToListAsync();
                    if (orphanedArtists.Count == 0)
                    {
                        return;
                    }

                    context.Artists.RemoveRange(orphanedArtists);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }
        public async Task removeGenresWithoutSongsAsync()
        {
            using (var context = new TContext())
            {
                try
                {
                    var orphanedGenres = await context.Genres
                        .Include(g => g.Songs)
                        .Include(g => g.Albums)
                        .Where(g => (g.Songs == null ||
                                     g.Songs.Count() == 0) &&
                                    (g.Albums == null ||
                                     g.Albums.Count() == 0))
                        .ToListAsync();
                    if (orphanedGenres.Count == 0)
                    {
                        return;
                    }

                    context.Genres.RemoveRange(orphanedGenres);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new CDCatalogDALWriteException(ex);
                }
            }
        }

        #endregion

        #region Insert/Update Helpers

        private async Task<bool> handleArtistAsync<T>(ISongCatalogContext context, T entity, List<Artist> cache = null)
            where T : IHasArtist
        {
            //Existing artist is set through foreign key:
            if (entity.ArtistId != default(int))
            {
                entity.Artist = null;
            }

            //New or existing Artist is set through navigation property:
            else if (entity.Artist != null)
            {
                //Existing artist set through navigation property Id
                if (entity.Artist.Id != default(int))
                {
                    entity.ArtistId = entity.Artist.Id;
                    entity.Artist = null;
                }
                //New or existing Artist set by Name
                else if (!String.IsNullOrEmpty(entity.Artist.Name))
                {
                    Artist artist;
                    //check cache first
                    if (cache != null)
                    {
                        artist = cache
                            .FirstOrDefault(a => String.Equals(a.Name, entity.Artist.Name, stringComparison));
                        if (artist != default(Artist))
                        {
                            //Artist already added, but not saved
                            //don't add twice
                            entity.Artist = artist;
                            return true;
                        }
                    }
                    //check Db
                    artist = await context.Artists
                        .FirstOrDefaultAsync(a => String.Equals(a.Name.ToLower(), entity.Artist.Name.ToLower()));
                    //new artist
                    if (artist == default(Artist))
                    {
                        context.SetAdded(entity.Artist);
                        //cache Artist
                        if (cache != null)
                        {
                            cache.Add(entity.Artist);
                        }
                    }
                    //existing artist, already verified in Db
                    else
                    {
                        entity.Artist = null;
                        entity.ArtistId = artist.Id;
                        //cache Artist
                        if (cache != null)
                        {
                            cache.Add(artist);
                        }
                    }
                    return true;
                }
                //invalid Artist Name
                else
                {
                    return false;
                }
            }
            //artist required
            else
            {
                return false;
            }

            //check if ArtistId already exists
            //check cache first
            if (cache != null &&
                cache.Any(a => a.Id == entity.ArtistId))
            {
                return true;
            }

            //check Db
            var existingArtist = await context.Artists.FindAsync(entity.ArtistId);
            if (existingArtist == default(Artist))
            {
                return false;
            }

            //cache Artist
            if (cache != null)
            {
                cache.Add(existingArtist);
            }

            return true;
        }
        private  async Task<bool> handleGenreAsync<T>(ISongCatalogContext context, T entity, List<Genre> cache = null)
            where T : IHasGenre
        {
            //Existing Genre is set through foreign key:
            if (entity.GenreId != default(int))
            {
                entity.Genre = null;
            }

            //New or existing Genre is set through navigation property:
            else if (entity.Genre != null)
            {
                //Existing Genre set through navigation property Id
                if (entity.Genre.Id != default(int))
                {
                    entity.GenreId = entity.Genre.Id;
                    entity.Genre = null;
                }
                //New or existing Genre set by Name
                else if (!String.IsNullOrEmpty(entity.Genre.Name))
                {
                    Genre genre;
                    //check cache first
                    if (cache != null)
                    {
                        genre = cache
                            .FirstOrDefault(g => String.Equals(g.Name, entity.Genre.Name, stringComparison));
                        if (genre != default(Genre))
                        {
                            //Genre already added, but not saved
                            //don't add twice
                            entity.Genre = genre;
                            return true;
                        }
                    }
                    //check Db
                    genre = await context.Genres
                        .FirstOrDefaultAsync(g => String.Equals(g.Name.ToLower(), entity.Genre.Name.ToLower()));
                    //New Genre
                    if (genre == default(Genre))
                    {
                        context.SetAdded(entity.Genre);
                        //cache genre
                        if (cache != null)
                        {
                            cache.Add(entity.Genre);
                        }
                    }
                    //Existing Genre, already verified in Db
                    else
                    {
                        entity.Genre = null;
                        entity.GenreId = genre.Id;
                        //cache genre
                        if (cache != null)
                        {
                            cache.Add(genre);
                        }
                    }
                    return true;
                }
                //invalid Genre name
                else
                {
                    return false;
                }
            }
            //required genre
            else
            {
                return false;
            }

            //check if GenreId already exists
            //check cache first
            if (cache != null
                && cache.Any(g => g.Id == entity.GenreId))
            {
                return true;
            }

            //check Db
            var existingGenre = await context.Genres.FindAsync(entity.GenreId);
            if (existingGenre == default(Genre))
            {
                return false;
            }

            //cache genre
            if (cache != null)
            {
                cache.Add(existingGenre);
            }

            return true;
        }
        private async Task<bool> handleSongForInsertAlbumAndSetTrackNumberIfUnsetAsync(ISongCatalogContext context, Song song)
        {
            Album album = null;

            if (song.AlbumId != null ||
                (song.Album != null &&
                 song.Album.Id != default(int)))
            {
                if (!(await handleSongForInsertAlbumSetByIdAsync(context, song, album)))
                {
                    return false;
                }
            }
            else if (song.Album != null)
            {
                if (!(await handleSongForInsertAlbumSetByTitleAsync(context, song, album)))
                {
                    return false;
                }
            }

            if (!handleSongForInsertTracknumberWhenExistingAlbum(song, album))
            {
                return false;
            }

            return true;
        }
        private async Task<bool> handleSongForInsertAlbumSetByIdAsync(ISongCatalogContext context, Song song, Album album)
        {
            if (song.AlbumId == null)
            {
                song.AlbumId = song.Album.Id;
            }
            song.Album = null;

            album = await context.Albums
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == song.AlbumId);
            if (album == default(Album))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private async Task<bool> handleSongForInsertAlbumSetByTitleAsync(ISongCatalogContext context, Song song, Album album)
        {
            if (String.IsNullOrEmpty(song.Album.Title))
            {
                return false;
            }

            album = await context.Albums
                .FirstOrDefaultAsync(a => String.Equals(a.Title.ToLower(), song.Album.Title.ToLower()));
            if (album == default(Album))
            {
                return false;
            }

            song.Album = null;
            song.AlbumId = album.Id;
            return true;
        }
        private bool handleSongForInsertTracknumberWhenExistingAlbum(Song song, Album existingAlbum)
        {
            if (existingAlbum != null)
            {
                //if TrackNumber not set, set to max track + 1
                if (song.TrackNumber == null)
                {
                    if (existingAlbum.Songs == null ||
                        existingAlbum.Songs.Count == 0)
                    {
                        song.TrackNumber = 1;
                    }
                    else
                    {
                        song.TrackNumber = existingAlbum.Songs.Max(s => s.TrackNumber) + 1;
                    }
                }
                //no duplicate track numbers
                else if (existingAlbum.Songs.Any(s => s.TrackNumber == song.TrackNumber))
                {
                    return false;
                }
            }

            return true;
        }
        //private bool validateAlbumForInsertAndSetTrackNumbersIfUnset(Album album)
        //{
        //    //Valid album, has a Song list with valid Songs
        //    if (album == null || album.Id != default(int) || !album.IsValid) return false;
        //    if (album.Songs == null || album.Songs.Count == 0) return false;
        //    if (!album.Songs.All(s => s.IsValid)) return false;

        //    //if all Songs have TrackNumber, TrackNumber must be unique
        //    if (album.Songs.Select(s => s.TrackNumber).All(t => t != null))
        //    {
        //        if (album.Songs.Select(s => s.TrackNumber).Distinct().Count() != album.Songs.Count) return false;
        //    }
        //    //if no song has a TrackNumber, set to ordinal in Songs List
        //    else if (album.Songs.All(s => s.TrackNumber == null))
        //    {
        //        setAlbumForInsertSongTrackNumbersIfUnset(album);
        //    }
        //    else return false;

        //    //Song titles not duplicate
        //    if (album.Songs.Select(s => s.Title).Distinct(stringEqualityComparer).Count() != album.Songs.Count) return false;

        //    return true;
        //}
        //private void setAlbumForInsertSongTrackNumbersIfUnset(Album album)
        //{
        //    int counter = 1;
        //    foreach (var song in album.Songs)
        //    {
        //        song.TrackNumber = counter;
        //        ++counter;
        //    }
        //}
        private async Task<bool> handleSongForUpdateAlbumAndSetTrackNumbersIfUnsetAsync(ISongCatalogContext context, Song song)
        {
            Album album = null;

            if (song.AlbumId != null ||
                (song.Album != null &&
                 song.Album.Id != default(int)))
            {
                if (!(await handleSongForUpdateAlbumSetByIdAsync(context, song, album)))
                {
                    return false;
                }
            }
            else if (song.Album != null)
            {
                if (!(await handleSongForUpdateAlbumSetByTitleAsync(context, song, album)))
                {
                    return false;
                }
            }

            if (album != null)
            {
                if (!handleSongForUpdateWithAlbumTracknumber(context, song, album))
                {
                    return false;
                }
            }

            return true;
        }
        private async Task<bool> handleSongForUpdateAlbumSetByIdAsync(ISongCatalogContext context, Song song, Album album)
        {
            if (song.AlbumId == null)
            {
                song.AlbumId = song.Album.Id;
            }
            song.Album = null;

            album = await context.Albums
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == song.AlbumId);
            if (album == default(Album))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private async Task<bool> handleSongForUpdateAlbumSetByTitleAsync(ISongCatalogContext context, Song song, Album album)
        {
            if (String.IsNullOrEmpty(song.Album.Title))
            {
                return false;
            }

            album = await context.Albums
                .FirstOrDefaultAsync(a => String.Equals(a.Title.ToLower(), song.Album.Title.ToLower()));
            if (album == default(Album))
            {
                return false;
            }
            else
            {
                song.Album = null;
                song.AlbumId = album.Id;
                return true;
            }
        }
        private bool handleSongForUpdateWithAlbumTracknumber(ISongCatalogContext context, Song song, Album album)
        {
            //if TrackNumber not set, set to max track + 1, else TrackNumbers must be unique
            if (song.TrackNumber == null)
            {
                if (album.Songs.Count == 0)
                {
                    song.TrackNumber = 1;
                }
                else
                {
                    song.TrackNumber = album.Songs.Max(s => s.TrackNumber) + 1;
                }
            }
            else if (album.Songs.Any(s => s.TrackNumber == song.TrackNumber))
            {
                return false;
            }

            return true;
        }
        private async Task<bool> handleSongForUpdateTitleAndAlbumUpdatesAsync(ISongCatalogContext context, Song song)
        {
            if (await context.Songs.AnyAsync(s =>
                                String.Equals(s.Title.ToLower(), song.Title.ToLower()) &&
                                s.AlbumId == song.AlbumId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private async Task<bool> handAlbumForUpdateTitleUpdatesAsync(ISongCatalogContext context, Album album)
        {
            if (await context.Albums.AnyAsync(a => String.Equals(a.Title.ToLower(), album.Title.ToLower())))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Playlist Helpers and Fields

        //For Playlist Generation
        private readonly static int minPlaylistTimeMinutes;
        private static int minPlaylistTimeSeconds { get { return minPlaylistTimeMinutes * 60; } }
        private readonly static int maxPlaylistTimeMinutes;
        private static int maxPlaylistTimeSeconds { get { return maxPlaylistTimeMinutes * 60; } }
        private readonly static double averageSongLengthMinutes;
        private static int averageSongLengthSeconds { get { return (int)(averageSongLengthMinutes * 60 + 1); } }
        private readonly static double predictedNumberSongsNeededMultiplier;
        private readonly static int minQueriedCount;
        private readonly static int maxQueriedCount;
        private readonly static int maxPlaylistDeltaMinutes;
        private static int maxPlaylistDeltaSeconds { get { return maxPlaylistDeltaMinutes * 60; } }
        private readonly static int minIterations;
        private readonly static double maxIterationsMultiplier;

        private static bool playlistAccepted(int playlistLengthSeconds, int targetSeconds)
        {
            return (playlistLengthSeconds >= (targetSeconds - maxPlaylistDeltaSeconds) &&
                    playlistLengthSeconds <= (targetSeconds + maxPlaylistDeltaSeconds));
        }
        private static bool playlistTooLong(int playlistLengthSeconds, int targetSeconds)
        {
            return (playlistLengthSeconds > (targetSeconds + maxPlaylistDeltaSeconds));
        }
        private static bool playlistTooShort(int playlistLengthSeconds, int targetSeconds)
        {
            return (playlistLengthSeconds < (targetSeconds - maxPlaylistDeltaSeconds));
        }

        #endregion

        #region String Comparison Fields

        //For String Comparison
        private static readonly StringComparer stringComparer;
        private static readonly StringComparison stringComparison;
        private static readonly IEqualityComparer<string> stringEqualityComparer;
 
        #endregion
    }
}
