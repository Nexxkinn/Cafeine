using Cafeine.Services.Mvvm;
using System;
using Windows.UI.Xaml.Controls;

namespace Cafeine.ViewModels
{
    /// <summary>
    /// Consider using this method as the last resort. <br/>
    /// Other than that, this method is deprecated.
    /// </summary>
    public class BrowserAuthViewModel : ViewModelBase
    {
        private string _headertitle;

        public Uri Source;

        public string HeaderTitle {
            get => _headertitle;
            set => Set(ref _headertitle, value);
        }

        public BrowserAuthViewModel()
        {
            HeaderTitle = "Loading...";

            // Add other service authentication here.
            Source = new Uri("");

            GoBack = new CafeineCommand(() => 
                {
                    navigationService.GoBack();
                    navigationService.ClearHistory();
                });
        }
        public void UrlCheck(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            // do URL check and verification here.
        }
    }
}
