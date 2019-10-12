using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.Services.Mvvm
{
    public class ViewModelBase : IViewModel, INotifyPropertyChanged, IDisposable
    {

        public event PropertyChangedEventHandler PropertyChanged ;
        protected void RaisePropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            RaisePropertyChanged(name);
            return true;
        }

        public virtual Task OnNavigatedTo(NavigationEventArgs e) { return Task.CompletedTask; }
        public virtual Task OnNavigatedFrom(NavigationEventArgs e = null) { return Task.CompletedTask; }
        public virtual Task OnLoaded(object sender, RoutedEventArgs e) { return Task.CompletedTask; }
        public virtual Task OnGoingBack() { return Task.CompletedTask; }

        public virtual void Dispose() {
            navigationService = null;
            Link = null;
            GoBack = null;
        }


        public NavigationService navigationService;
        public ViewModelLink Link;
        public CafeineCommand GoBack;
        public ViewModelBase()
        {
            navigationService = new NavigationService();
            Link = new ViewModelLink();
            GoBack = new CafeineCommand(navigationService.GoBack);
        }
    }
}
