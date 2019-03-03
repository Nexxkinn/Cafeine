using Cafeine.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Cafeine.Services.Mvvm
{
    public class NavigationService
    {
        private HomePage Page => (Window.Current.Content as Frame).Content as HomePage;
        private Frame ChildPage => Page.Vm.ChildFrame;
        private Frame RootFrame => Window.Current.Content as Frame;

        // Default cacheMode as enabled, as it was intended for controlling cache. 
        public void Navigate(Type type, object parameter = null) => Navigate(type, parameter, null);
        public void Navigate(Type type, object parameter,NavigationTransitionInfo navigationtransition = null)
        {
            if(Page != null)
            {
                Page.Vm.SetWindowState(type.Name);
                ChildPage.Navigate(type, parameter, navigationtransition);
            }
            else
            {
                // assume current page is not HomePage
                RootFrame.Navigate(type, parameter, navigationtransition);
            }
        }
        public bool CanGoBack()
        {
            return ChildPage.CanGoBack;
        }
        public async Task GoBack()
        {
            // run ongoingback first
            await ((ChildPage.Content as BasePage).DataContext as ViewModelBase).OnGoingBack();
            if(Page != null)
            {
                if (CanGoBack()) ChildPage.GoBack();
                Page.Vm.SetWindowState(ChildPage.CurrentSourcePageType.Name);
            }
            else
            {
                if (RootFrame.CanGoBack) RootFrame.GoBack();
            }
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
            if (Page != null)
            {
                // assume current page is HomePage
                ChildPage.BackStack.Clear();
            }
            else
            {
                // assume current page is not HomePage
                RootFrame.BackStack.Clear();
            }
        }
    }
}
