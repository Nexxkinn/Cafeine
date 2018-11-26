using Cafeine.Services;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Cafeine.ViewModels
{
    public class LoginPageViewModel : ViewModelBase, INavigationAware
    {
        //list accounts
        //basic login
        public ReactiveCommand Login { get; }
        public AsyncReactiveCommand LoginCheck { get; }
        private INavigationService _navigationService;
        private IEventAggregator _eventAggregator;
        public LoginPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            Login = new ReactiveCommand();
            Login.Subscribe( () => {
                    _navigationService.Navigate("BrowserAuthentication", null);
                    //_eventAggregator.GetEvent<IsKeyAvailable>().Subscribe(async x =>
                    //{
                    //    var sw = new Stopwatch();
                    //    Debug.Write("load one");
                    //    sw.Start();

                    //    var Raw_URI_Auth = await AniListApi.GetAuthenticationFromServer();
                    //    AniListApi.BuildAuthenticator(Raw_URI_Auth);

                    //    sw.Stop();
                    //    Debug.Write(sw.ElapsedMilliseconds);
                    //    sw.Reset();

                    //    Debug.Write("load two");
                    //    sw.Start();

                    //    await AniListApi.CreateAccount(true);

                    //    sw.Stop();
                    //    Debug.Write(sw.ElapsedMilliseconds);
                    //    sw.Reset();

                    //    Debug.Write("load three");
                    //    sw.Start();

                    //    var Raw_Database = await Database.CreateDBFromServices();
                    //    Database.BuildDatabase(Raw_Database);

                    //    sw.Stop();
                    //    Debug.Write(sw.ElapsedMilliseconds);
                    //    sw.Reset();

                    //    _navigationService.Navigate("Main", null);
                    //});
            });

            LoginCheck = new AsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    if (!Database.IsAccountEmpty())
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        _navigationService.Navigate("Main", null);
                    }
            });
        }
        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _navigationService.ClearHistory();
            base.OnNavigatedTo(e, viewModelState);
        }
    }
    public class IsKeyAvailable : PubSubEvent<bool> { }
}
