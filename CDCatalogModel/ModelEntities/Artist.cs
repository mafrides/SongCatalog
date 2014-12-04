using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CDCatalogModel
{
    public partial class Artist : INotifyPropertyChanged, IComparable<Artist>, IHasId
    {
        static Artist()
        {
            ignoreCaseInNameComparison = true;
            defaultStringComparison = StringComparison.CurrentCultureIgnoreCase;
        }

        public Artist()
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

        //Equality as Search Term, by Id or Name
        public static bool operator==(Artist a1, Artist a2)
        {  
            return ReferenceEquals(a1, a2) 
                || ((object)a1 != null && a1.Equals(a2));
        }
        public static bool operator!=(Artist a1, Artist a2)
        {
            return !(a1 == a2);
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Artist a = obj as Artist;
            return this.Equals(a);
        }
        public bool Equals(Artist a)
        {
            return (object)a != null
                && (Id == a.Id || String.Equals(Name, a.Name, defaultStringComparison));
        }

        //Hash by Name
        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : base.GetHashCode();
        }

        //Display by Name
        public override string ToString()
        {
            return String.IsNullOrEmpty(Name) ? "" : Name;
        }

        //IComparable: Sort by Name
        public int CompareTo(Artist other)
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

        //Name Comparison
        private static readonly bool ignoreCaseInNameComparison;
        private static readonly StringComparison defaultStringComparison;

        #endregion
    }
}
