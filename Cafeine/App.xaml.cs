using Cafeine.Views;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cafeine
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Page rootpage = Window.Current.Content as Page;

            if (rootpage == null)
            {
                HomePage page= new HomePage();

                Frame rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    await Services.ImageCache.CreateImageCacheFolder();
                    CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

                    ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                }

                page.Vm.SetFrame(rootFrame);

                Window.Current.Content = page;
                
                page.Vm.ChildFrame.Navigate(typeof(LoginPage), e.Arguments);

                Window.Current.Activate();
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootpage == null)
                {
                    // Place the frame in the current Window
                }
            }
        }
        
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
        
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
