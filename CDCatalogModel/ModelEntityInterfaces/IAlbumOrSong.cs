using System.ComponentModel;

namespace CDCatalogModel
{
    public interface IAlbumOrSong
        : IHasId, IHasTitle, IHasGenre, IHasArtist, IRated,
        IHasDisplayRating, IHasDisplayTitle, IHasDisplayTrackLength, IEditableObject { }
}
