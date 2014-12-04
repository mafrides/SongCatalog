using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CDCatalogModel
{
    public partial class Genre : INotifyPropertyChanged, IComparable<Genre>, IHasId
    {
        static Genre()
        {
            ignoreCaseInNameComparison = true;
            defaultStringComparison = StringComparison.CurrentCultureIgnoreCase;
        }

        public Genre()
        {
           this.Albums = new List<Album>();
           this.Songs = new List<Song>();
        }

        public int Id
        {
            get { return id; }
            set
            {
                if(id != value)
                {
                    id = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Id"));
                }
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if(name != value)
                {
                    name = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public ICollection<Album> Albums { get; set; }
        public ICollection<Song> Songs { get; set; }

        //Application Specific Validation
        //Can be used as a search term, so only one field needs to be set
        public bool IsValid
        {
            get
            {
                return (Id > 0 && (Name == null || Name.Length > 0))
                    || (Id == default(int) && !String.IsNullOrEmpty(Name));
            }
        }

        //Equality as a search term, by Id or Name
        public static bool operator ==(Genre g1, Genre g2)
        {
            return ReferenceEquals(g1, g2)
                || ((object)g1 != null && g1.Equals(g2));
        }
        public static bool operator !=(Genre g1, Genre g2)
        {
            return !(g1 == g2);
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Genre g = obj as Genre;
            return this.Equals(g);
        }
        public bool Equals(Genre g)
        {
            return (object)g != null
                && (Id == g.Id || String.Equals(Name, g.Name, defaultStringComparison));
        }

        //Hash by Name
        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : base.GetHashCode();
        }

        //Dispaly by Name
        public override string ToString()
        {
            return String.IsNullOrEmpty(Name) ? "" : Name;
        }

        //IComparable: Sort by Name
        public int CompareTo(Genre other)
        {
            return Name == null ? 1 
                : (other == null || other.Name == null) ? -1
                : String.Compare(Name, other.Name, ignoreCaseInNameComparison);
        }

        //INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Fields

        //Backing Fields
        private int id;
        private string name;

        //Name comparison
        private static readonly bool ignoreCaseInNameComparison;
        private static readonly StringComparison defaultStringComparison;

        #endregion
    }
}
