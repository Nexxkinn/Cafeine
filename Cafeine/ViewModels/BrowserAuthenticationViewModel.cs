using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Controls;

namespace Cafeine.ViewModels
{
    public class BrowserAuthenticationPageViewModel : ViewModelBase, IViewModel
    {

        public ReactiveProperty<Uri> Source;

        public ReactiveProperty<string> HeaderTitle { get; }

        public BrowserAuthenticationPageViewModel()
        {
            navigationService = new NavigationService();

            HeaderTitle = new ReactiveProperty<string>("Loading...");

            Source = new ReactiveProperty<Uri>(new Uri("https://anilist.co/api/v2/oauth/authorize?client_id=873&response_type=token"));

            GoBack = new CafeineCommand(() => {
                navigationService.GoBack();
                navigationService.ClearHistory();
            });
        }
        public async void UrlCheck(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string url = args.Uri.AbsoluteUri.ToString();
            if (url.Contains("anilist.co/api/v2/oauth/Annalihation#access_token="))
            {
                IService service = new AniListApi();

                //get the token
                Regex r = new Regex(@"(?<==).+?(?=&|$)");
                Match m = r.Match(url);
                await service.VerifyAccount(m.Value);

                var CurrentUserAccount = await service.CreateAccount(true);
                Database.AddAccount(CurrentUserAccount);

                navigationService.GoBack();
            }
            else HeaderTitle.Value = "Anilist Web Authentication";
        }
    }
}
