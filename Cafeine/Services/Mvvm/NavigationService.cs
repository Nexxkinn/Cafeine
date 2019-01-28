using Cafeine.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.Services.Mvvm
{
    public class NavigationService
    {
        private HomePage Page => Window.Current.Content as HomePage;
        private Frame ChildPage => Page.Vm.ChildFrame;

        // Default cacheMode as enabled, as it was intended for controlling cache. 
        public void Navigate(Type type, object parameter = null )
        {
            Page.Vm.SetWindowState(type.Name);
            ChildPage.Navigate(type, parameter);
        }
        public bool CanGoBack()
        {
            return ChildPage.CanGoBack;
        }
        public void GoBack()
        {
            if (CanGoBack()) ChildPage.GoBack();
            Page.Vm.SetWindowState(ChildPage.CurrentSourcePageType.Name);
        }
        public void RemoveLastPage()
        {
            if (ChildPage.BackStackDepth > 1)
            {
                ChildPage.BackStack.RemoveAt(ChildPage.BackStackDepth-1);
            }
        }
        public void ClearHistory()
        {
            ChildPage.BackStack.Clear();
        }
    }
}
