using Cafeine.Design.RemoteTorrent;
using Cafeine.Design.RemoteTorrent.qBittorent;
using Cafeine.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Cafeine.ViewModel {
    public class TorrentManagerViewModel : ViewModelBase {
        private ICafeineNavigationService _navigationservice;

        public List<TorrentModel> list = new List<TorrentModel>();

        private Visibility _isErrorVisualVisible = Visibility.Collapsed;
        public Visibility IsErrorVisualVisible {
            get {
                return _isErrorVisualVisible;
            }
            set {
                Set(ref _isErrorVisualVisible, value);
            }
        }

        private string _myProperty = string.Empty;
        public string ErrorValue {
            get {
                return _myProperty;
            }
            set {
                Set(ref _myProperty, value);
            }
        }

        public TorrentManagerViewModel(ICafeineNavigationService navigation) {
            _navigationservice = navigation;
            Fetchdata().Wait();
        }
        public async Task Fetchdata() {
            try {
                //Authentication
                //CoreApi.Authentication(8080, "admin", "adminadmin").Wait();
                var credentials = CoreApi.GetTorrentCredential();
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                string port = (string)localSettings.Values["localport"];

                //TODO : Factory Pattern. Returns "a method was called at an unexpected time" at any way.
                bool authentication = await CoreApi.Authentication(port, credentials.UserName, credentials.Password).ConfigureAwait(false);
                if (authentication) {
                    //fetch data
                    list = await Comm.GetTorrentList();
                }
            }
            catch (Exception e) {
                IsErrorVisualVisible = Visibility.Visible;
                ErrorValue = e.ToString();
            }

        }

        ///PauseItemClick
        private RelayCommand _pauseItemClick;
        public RelayCommand PauseItemClick {
            get {
                return _pauseItemClick
                    ?? (_pauseItemClick = new RelayCommand(
                    () => {

                    }));
            }
        }
        //ResumeItemClick
        private RelayCommand _resumeItemClick;
        public RelayCommand ResumeItemClick {
            get {
                return _resumeItemClick
                    ?? (_resumeItemClick = new RelayCommand(
                    () => {

                    }));
            }
        }
        ///DeleteItemClick
        private RelayCommand _deleteItemClick;
        public RelayCommand DeleteItemClick {
            get {
                return _deleteItemClick
                    ?? (_deleteItemClick = new RelayCommand(
                    () => {

                    }));
            }
        }
    }
}
