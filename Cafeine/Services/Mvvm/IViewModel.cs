using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.Services.Mvvm
{
    interface IViewModel
    {
        Task OnNavigatedTo(NavigationEventArgs e);
        Task OnNavigatedFrom(NavigationEventArgs e);
        Task OnLoaded(object sender, RoutedEventArgs e);
    }
}
