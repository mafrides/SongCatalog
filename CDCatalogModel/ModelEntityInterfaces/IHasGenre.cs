namespace CDCatalogModel
{
    public interface IHasGenre
    {
        int GenreId { get; set; }
        Genre Genre { get; set; }
    }
}
