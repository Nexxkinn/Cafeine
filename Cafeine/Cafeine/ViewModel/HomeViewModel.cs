using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Cafeine.ViewModel {
    public class HomeViewModel : ViewModelBase {
        private INavigationService _navigationservice;
        private Frame _frame = new Frame();

        /// <summary>
        /// Sets and gets the F property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Frame F {
            get {
                return _frame;
            }
            set {
                Set(() => F, ref _frame, value);
            }
        }
        public HomeViewModel(INavigationService navigationservice) {
            _navigationservice = navigationservice;
            F.Navigate(typeof(DirectoryExplorer));
        }
    }
}
