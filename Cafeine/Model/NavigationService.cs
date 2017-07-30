using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cafeine.Model {
    public interface INavigationService {
        void Navigate(Type sourcePage);
        void Navigate(Type sourcePage, object parameter);
        void GoBack();
    }
    public sealed class NavigationService : INavigationService {
        public void Navigate(Type sourcePage) {
            var frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage);
        }

        public void Navigate(Type sourcePage, object parameter) {
            var frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage, parameter);
        }

        public void GoBack() {
            var frame = (Frame)Window.Current.Content;
            frame.GoBack();
        }
    }


}
