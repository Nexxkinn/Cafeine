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
        private async Task Fetchdata() {
            try {
                //Authentication
                var credentials = CoreApi.GetTorrentCredential();
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                int port = (int)localSettings.Values["localport"];
                bool authentication = await CoreApi.Authentication(port, credentials.UserName, credentials.Password);
                if (authentication) {
                    //fetch data
                    list = await Comm.GetTorrentList();
                }
            }
            catch (Exception e) {
                IsErrorVisualVisible = Visibility.Visible;
                ErrorValue = e.GetType().ToString();
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
