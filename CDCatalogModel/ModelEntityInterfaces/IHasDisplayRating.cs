using System;

namespace CDCatalogModel
{
    public interface IHasDisplayRating
    {
        Nullable<double> DisplayRating { get; }
    }
}
