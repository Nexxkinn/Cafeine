using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Windows.UI;

namespace Cafeine
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage() {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null) {
                titleBar.BackgroundColor = Color.FromArgb(255, 47, 52, 59);
                titleBar.InactiveBackgroundColor = Color.FromArgb(255, 47, 52, 59);
                titleBar.ButtonBackgroundColor = Color.FromArgb(255, 47, 52, 59);
            }
            InitializeComponent();
        }
        
    }
}