using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Cafeine.Model;

namespace Cafeine.ViewModel {
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private string _clock = "Starting...";
        private int _counter;
        private RelayCommand _incrementCommand;
        private RelayCommand<string> _navigateCommand;
        private bool _runClock;
        private RelayCommand _sendMessageCommand;
        private RelayCommand _showDialogCommand;
        private string _welcomeTitle = string.Empty;

        public string Clock {
            get {
                return _clock;
            }
            set {
                Set(ref _clock, value);
            }
        }

        public RelayCommand IncrementCommand {
            get {
                return _incrementCommand
                    ?? (_incrementCommand = new RelayCommand(
                    () => {
                        WelcomeTitle = string.Format("Counter clicked {0} times", ++_counter);
                    }));
            }
        }

        public RelayCommand<string> NavigateCommand {
            get {
                return _navigateCommand
                       ?? (_navigateCommand = new RelayCommand<string>(
                           p => _navigationService.NavigateTo(ViewModelLocator.SignInPageKey, p),
                           p => !string.IsNullOrEmpty(p)));
            }
        }

        public RelayCommand SendMessageCommand {
            get {
                return _sendMessageCommand
                    ?? (_sendMessageCommand = new RelayCommand(
                    () => {
                        Messenger.Default.Send(
                            new NotificationMessageAction<string>(
                                "Testing",
                                reply => {
                                    WelcomeTitle = reply;
                                }));
                    }));
            }
        }

        public RelayCommand ShowDialogCommand {
            get {
                return _showDialogCommand
                       ?? (_showDialogCommand = new RelayCommand(
                           async () => {
                               var dialog = ServiceLocator.Current.GetInstance<IDialogService>();
                               await dialog.ShowMessage("Hello Universal Application", "it works...");
                           }));
            }
        }

        public string WelcomeTitle {
            get {
                return _welcomeTitle;
            }

            set {
                Set(ref _welcomeTitle, value);
            }
        }

        public MainViewModel(
            IDataService dataService,
            INavigationService navigationService) {
            _dataService = dataService;
            _navigationService = navigationService;
            Initialize();
        }

        public void RunClock() {
            _runClock = true;

            Task.Run(async () => {
                while (_runClock) {
                    try {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => {
                            Clock = DateTime.Now.ToString("HH:mm:ss");
                        });

                        await Task.Delay(1000);
                    }
                    catch (Exception) {
                    }
                }
            });
        }

        public void StopClock() {
            _runClock = false;
        }

        private async Task Initialize() {
            try {
                var item = await _dataService.GetData();
                WelcomeTitle = item.Title;
            }
            catch (Exception ex) {
                // Report error here
                WelcomeTitle = ex.Message;
            }
        }
    }
}