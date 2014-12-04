using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCatalogDomainModel
{
    public class GenreInfo
    {
        public const int MAX_GENRE_LENGTH = 100;

        public string Genre
        {
            get { return genre; }
            set 
            {
                if(value != null && value.Length <= MAX_GENRE_LENGTH)
                {
                    genre = value;
                }
            }
        }

        private string genre;
    }
}
