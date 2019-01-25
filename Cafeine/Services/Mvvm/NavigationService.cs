using Cafeine.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cafeine.Services.Mvvm
{
    public class NavigationService
    {
        private HomePage page => Window.Current.Content as HomePage;
        private Frame childpage => page.Vm.ChildFrame;
        public void Navigate(Type type, object parameter)
        {
            childpage.Navigate(type, parameter);
        }

        public void NavigateWithState(Type type, int windowState)
        {

        }
        public bool CanGoBack()
        {
            return childpage.CanGoBack;
        }
        public void GoBack()
        {
            if (CanGoBack()) childpage.GoBack();
        }
        public void RemoveLastPage()
        {
            childpage.BackStack.RemoveAt(childpage.BackStackDepth);
        }
        public void ClearHistory()
        {
            childpage.BackStack.Clear();
        }
    }
}
