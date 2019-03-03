using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.Services.Mvvm
{
    public class ViewModelBase : IViewModel, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged ;
        public void RaisePropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public virtual Task OnNavigatedTo(NavigationEventArgs e) { return Task.CompletedTask; }
        public virtual Task OnNavigatedFrom(NavigationEventArgs e = null) { return Task.CompletedTask; }
        public virtual Task OnLoaded(object sender, RoutedEventArgs e) { return Task.CompletedTask; }
        public virtual Task OnGoingBack() { return Task.CompletedTask; }

        public NavigationService navigationService;
        public ViewModelLink eventAggregator;
        public CafeineCommand GoBack;
        public ViewModelBase()
        {
            navigationService = new NavigationService();
            eventAggregator = new ViewModelLink();
            GoBack = new CafeineCommand(async()=>await navigationService.GoBack());
        }
    }
}
