using Cafeine.Views;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Cafeine.Services.Mvvm
{
    public class NavigationService
    {
        public static event EventHandler<Visibility> EnableBackButton;
        public static Frame ChildPage = new Frame();

        private Frame RootFrame => Window.Current.Content as Frame;
        private bool IsHomePage => RootFrame.Content is HomePage;
        private bool CanGoBack  => IsHomePage ? ChildPage.CanGoBack : RootFrame.CanGoBack;

        public void Navigate(Type type, object parameter = null) => Navigate(type, parameter, null);
        public void Navigate(Type type, object parameter,NavigationTransitionInfo navigationtransition = null)
        {
            if(IsHomePage)
            {
                ChildPage.Navigate(type, parameter, navigationtransition);
            }
            else
            {
                // assume current page is not HomePage
                RootFrame.Navigate(type, parameter, navigationtransition);
            }
            if(CanGoBack) EnableBackButton?.Invoke(null,Visibility.Visible);
        }
        public void GoBack()
        {
            if(IsHomePage)
            {
                if (CanGoBack) ChildPage.GoBack();
            }
            else
            {
                if (RootFrame.CanGoBack) RootFrame.GoBack();
            }

            if (!CanGoBack) EnableBackButton?.Invoke(null, Visibility.Collapsed);
        }
        public void RemoveLastPage()
        {
            if (ChildPage.BackStackDepth > 1)
            {
                ChildPage.BackStack.RemoveAt(ChildPage.BackStackDepth-1);
            }
            if (!CanGoBack) EnableBackButton?.Invoke(null, Visibility.Collapsed);
        }
        public void ClearHistory()
        {
            if (IsHomePage)
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
