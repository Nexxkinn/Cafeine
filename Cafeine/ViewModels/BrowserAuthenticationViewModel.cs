using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Interactivity;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Controls;

namespace Cafeine.ViewModels
{
    public class BrowserAuthenticationPageViewModel : ViewModelBase, IViewModel
    {
        private NavigationService _navigationService;

        public ReactiveProperty<Uri> Source;

        public ReactiveCommand<string> urlcheck;

        public ReactiveCommand GoBack { get; }

        public ReactiveProperty<string> HeaderTitle { get; }

        public BrowserAuthenticationPageViewModel()
        {
            _navigationService = new NavigationService();

            HeaderTitle = new ReactiveProperty<string>("Loading...");

            Source = new ReactiveProperty<Uri>(new Uri("https://anilist.co/api/v2/oauth/authorize?client_id=873&response_type=token"));

            GoBack = new ReactiveCommand();
            GoBack.Subscribe(_ =>
            {
                _navigationService.GoBack();
                _navigationService.ClearHistory();
            });
            urlcheck = new ReactiveCommand<string>();
            urlcheck.Subscribe(async x =>
            {
                if (x.Contains("anilist.co/api/v2/oauth/Annalihation#access_token="))
                {
                    IService service = new AniListApi();

                    //get the token
                    Regex r = new Regex(@"(?<==).+?(?=&|$)");
                    Match m = r.Match(x);
                    await service.VerifyAccount(m.Value);

                    var CurrentUserAccount = await service.CreateAccount(true);
                    Database.AddAccount(CurrentUserAccount);

                    _navigationService.GoBack();
                }
                else HeaderTitle.Value = "Anilist Web Authentication";
            });
        }
    }

    public class NavigationCompletedReactiveConverter : ReactiveConverter<WebViewNavigationCompletedEventArgs, string>
    {
        protected override IObservable<string> OnConvert(IObservable<WebViewNavigationCompletedEventArgs> source)
        {
            return source.Select(x => x.Uri.AbsoluteUri.ToString());
        }
    }
}
