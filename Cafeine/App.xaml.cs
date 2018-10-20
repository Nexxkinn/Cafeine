using System.Threading.Tasks;
using Cafeine.Views;
using Prism.Events;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Unity;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cafeine
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : PrismUnityApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Container.RegisterInstance(NavigationService);
            Container.RegisterInstance<IEventAggregator>(new EventAggregator());
            return base.OnInitializeAsync(args);
        }
        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = Container.Resolve<HomePage>();
            shell.Vm.SetFrame(rootFrame);
            return shell;
        }
        protected override IDeviceGestureService OnCreateDeviceGestureService()
        {
            var svc = base.OnCreateDeviceGestureService();
            svc.UseTitleBarBackButton = false;
            return svc;
        }
        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            NavigationService.Navigate("Login", null);
            return Task.FromResult(true);
        }
    }
}
