using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.Services.Mvvm
{
    public class BasePage : Page
    {
        private ViewModelBase Vm => this.DataContext as ViewModelBase;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await Vm.OnNavigatedTo(e);
            base.OnNavigatedTo(e);
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await Vm.OnNavigatedFrom(e);
            base.OnNavigatedFrom(e);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            bool b = e.NavigationMode == NavigationMode.Forward;
            if (b)
            {
                e.Cancel = true;
            }
        }

    }
}
