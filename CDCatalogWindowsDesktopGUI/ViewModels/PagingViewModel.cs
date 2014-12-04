using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CDCatalogModel;
using CDCatalogDAL;

namespace CDCatalogWindowsDesktopGUI
{
    public class PagingViewModel : INotifyPropertyChanged
    {
        public PagingViewModel(MainWindowViewModel parent)
        {
            parentViewModel = parent;
            pageBackCommandAsync = new DelegateCommandAsync(OnPageBackAsync);
            pageFastBackCommandAsync = new DelegateCommandAsync(OnPageFastBackAsync);
            pageForwardCommandAsync = new DelegateCommandAsync(OnPageForwardAsync);
            pageFastForwardCommandAsync = new DelegateCommandAsync(OnPageFastForwardAsync);
        }

        public ICDCatalog Catalog
        {
            get { return parentViewModel.Catalog; }
        }

        public DelegateCommandAsync PageForwardCommandAsync
        {
            get { return pageForwardCommandAsync; }
        }
        public DelegateCommandAsync PageFastForwardCommandAsync
        {
            get { return pageFastForwardCommandAsync; }
        }
        public DelegateCommandAsync PageBackCommandAsync
        {
            get { return pageBackCommandAsync; }
        }
        public DelegateCommandAsync PageFastBackCommandAsync
        {
            get { return pageFastBackCommandAsync; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly MainWindowViewModel parentViewModel;
        private readonly DelegateCommandAsync pageForwardCommandAsync;
        private readonly DelegateCommandAsync pageFastForwardCommandAsync;
        private readonly DelegateCommandAsync pageBackCommandAsync;
        private readonly DelegateCommandAsync pageFastBackCommandAsync;

        private async Task OnPageForwardAsync()
        {
            try
            {

            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        private async Task OnPageFastForwardAsync()
        {
            try
            {

            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        private async Task OnPageBackAsync()
        {
            try
            {

            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        private async Task OnPageFastBackAsync()
        {
            try
            {

            }
            catch (CDCatalogException cex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
    }
}
