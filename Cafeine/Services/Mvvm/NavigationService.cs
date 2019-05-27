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
        public static event EventHandler<Visibility> EnableBackButton;
        private Frame RootFrame => Window.Current.Content as Frame;
        private Frame ChildPage => Page.Vm.ChildFrame;
        private HomePage Page => RootFrame.Content as HomePage;

        // Default cacheMode as enabled, as it was intended for controlling cache. 
        public void Navigate(Type type, object parameter = null) => Navigate(type, parameter, null);
        public void Navigate(Type type, object parameter,NavigationTransitionInfo navigationtransition = null)
        {
            if(Page != null)
            {
                ChildPage.Navigate(type, parameter, navigationtransition);
            }
            else
            {
                // assume current page is not HomePage
                RootFrame.Navigate(type, parameter, navigationtransition);
            }
            if(CanGoBack()) EnableBackButton?.Invoke(null,Visibility.Visible);
        }
        public bool CanGoBack()
        {
            if (Page == null) return RootFrame.CanGoBack;
            else return ChildPage.CanGoBack;
        }
        public void GoBack()
        {
            if(Page != null)
            {
                if (CanGoBack()) ChildPage.GoBack();
            }
            else
            {
                if (RootFrame.CanGoBack) RootFrame.GoBack();
            }

            if (!CanGoBack()) EnableBackButton?.Invoke(null, Visibility.Collapsed);
        }
        public void RemoveLastPage()
        {
            if (ChildPage.BackStackDepth > 1)
            {
                ChildPage.BackStack.RemoveAt(ChildPage.BackStackDepth-1);
            }
            if (!CanGoBack()) EnableBackButton?.Invoke(null, Visibility.Collapsed);
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

            EnableBackButton?.Invoke(null, Visibility.Collapsed);
        }
    }
}
