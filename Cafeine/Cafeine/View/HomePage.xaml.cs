using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Popups;
using Cafeine.Design;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Cafeine.Model;
using System.Collections.Generic;
using Windows.Storage;
using Newtonsoft.Json;
using System.Linq;
using Cafeine.ViewModel;

namespace Cafeine {
    public sealed partial class HomePage : Page {
        public HomeViewModel Vm => (HomeViewModel)DataContext;
        public HomePage() {
            InitializeComponent();
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null) {
                titleBar.BackgroundColor = Application.Current.Resources["SystemChromeMediumColor"] as Color?;
                titleBar.InactiveBackgroundColor = Application.Current.Resources["SystemChromeMediumColor"] as Color?;
                titleBar.ButtonBackgroundColor = Application.Current.Resources["SystemChromeMediumColor"] as Color?;
            }
        }
        //very ugly hack
        //private void TabChecked(object sender, RoutedEventArgs e) {
        //    RadioButton rb = sender as RadioButton;
        //    switch (rb.Name.ToString()) {
        //        case "Schedule":
        //        f.Navigate(typeof(Pages.Schedule));
        //        break;
        //        case "Library":
        //        f.Navigate(typeof(DirectoryExplorer));
        //        f.BackStack.Clear();
        //        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        //        break;
        //    }
        //}
        //private void SettingPage(object sender, RoutedEventArgs e) {
        //    f.Navigate(typeof(SettingsPage));
        //    Schedule.Visibility = Visibility.Collapsed;
        //    Library.Visibility = Visibility.Collapsed;
        //    SettingsTab.Visibility = Visibility.Visible;
        //}

    }
}