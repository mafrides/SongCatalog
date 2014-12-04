namespace CDCatalogModel
{
    public interface IHasArtist
    {
        int ArtistId { get; set; }
        Artist Artist { get; set; }
    }
}
