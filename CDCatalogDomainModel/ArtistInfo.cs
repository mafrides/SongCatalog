using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCatalogDomainModel
{
    public class ArtistInfo
    {
        public const int MAX_ARTIST_LENGTH = 100;

        public string Artist 
        { 
            get { return Artist; }
            set
            {
                if (value != null && value.Length <= MAX_ARTIST_LENGTH)
                {
                    artist = value;
                }
            }
        }

        private string artist;
    }
}
