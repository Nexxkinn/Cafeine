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
        // Frame.IsLoaded is totally unrealiable 
        private bool PageisLoaded = false;
        private ViewModelBase Vm => this.DataContext as ViewModelBase;

        protected async void OnLoaded(object sender, RoutedEventArgs e) {
            if (!PageisLoaded)
            {
                await Vm.OnLoaded(sender, e);
                PageisLoaded = true;
            }
            else
            {
                if(NavigationCacheMode == NavigationCacheMode.Disabled)
                {
                    await Vm.OnLoaded(sender, e);
                }
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame.Loaded += OnLoaded;
            await Vm.OnNavigatedTo(e);
            base.OnNavigatedTo(e);
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            Frame.Loaded -= OnLoaded;
            await Vm.OnNavigatedFrom(e);
            base.OnNavigatedFrom(e);
        }

    }
}
