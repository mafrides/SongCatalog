using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CDCatalogWindowsDesktopGUI
{
    public class AddAlbumOrSongViewModel : INotifyPropertyChanged
    {
        public AddAlbumOrSongViewModel(MainWindowViewModel parent)
        {
            parentViewModel = parent;
            addAlbumControlViewModel = new AddAlbumViewModel(parent);
            addSongControlViewModel = new AddSongViewModel(parent);
        }

        public AddSongViewModel AddSongControlViewModel
        {
            get { return addSongControlViewModel; }
        }
        public AddAlbumViewModel AddAlbumControlViewModel
        {
            get { return addAlbumControlViewModel; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly MainWindowViewModel parentViewModel;
        private readonly AddAlbumViewModel addAlbumControlViewModel;
        private readonly AddSongViewModel addSongControlViewModel;
    }
}
