using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using Windows.UI.Xaml.Controls;

namespace Cafeine.ViewModels
{
    public sealed class BrowserAuthenticationPageViewModel : ViewModelBase, INavigationAware
    {
        private INavigationService _navigationService;

        public ReactiveProperty<Uri> Source;

        public ReactiveCommand<string> urlcheck;

        public ReactiveCommand GoBack { get; }

        public ReactiveProperty<string> HeaderTitle { get; }

        public IEventAggregator _eventAggregator;

        public BrowserAuthenticationPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            HeaderTitle = new ReactiveProperty<string>("Loading...");

            Source = new ReactiveProperty<Uri>(new Uri("https://anilist.co/api/v2/oauth/authorize?client_id=873&response_type=token"));

            GoBack = new ReactiveCommand();
            GoBack.Subscribe(_ =>
            {
                _navigationService.GoBack();
                _navigationService.ClearHistory();
            });
            urlcheck = new ReactiveCommand<string>();
            urlcheck.Subscribe(x =>
            {
                if (x.Contains("anilist.co/api/v2/oauth/Annalihation#access_token="))
                {
                    _navigationService.GoBack();
                    _eventAggregator.GetEvent<IsKeyAvailable>().Publish(true);
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
