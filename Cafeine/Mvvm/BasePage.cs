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
        //TODO: remove this method in the final release
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
