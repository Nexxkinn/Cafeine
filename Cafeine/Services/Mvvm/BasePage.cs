using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.Services.Mvvm
{
    public class BasePage : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelBase vm = this.DataContext as ViewModelBase;
            vm.OnNavigatedTo(e);
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModelBase vm = this.DataContext as ViewModelBase;
            vm.OnNavigatedFrom(e);
            base.OnNavigatedFrom(e);
        }
    }
}
