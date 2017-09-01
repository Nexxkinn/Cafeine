using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Cafeine.Model;
using System;

namespace Cafeine.ViewModel {
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator {
        public const string SignInPageKey = "SignInPage";
        private string _currentScanVMKey;
        
        static ViewModelLocator() {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var nav = new NavigationService();
            nav.Configure(SignInPageKey, typeof(LoginPage));
            nav.Configure("HomePage", typeof(HomePage));
            SimpleIoc.Default.Register<INavigationService>(() => nav);

            var F = new CafeineNavigationService();
            F.Configure("SettingsPage", typeof(SettingsPage));
            F.Configure("VirDir", typeof(DirectoryExplorer));
            F.Configure("Collection", typeof(CollectionLibraryFrame));
            F.Configure("ExpandItem", typeof(ExpandItemDetails));
            F.Configure("TorentManager", typeof(TorrentManager));
            //F.Configure(SignInPageKey, typeof(SignInDialog));
            SimpleIoc.Default.Register<ICafeineNavigationService>(() => F);

            SimpleIoc.Default.Register<IDialogService, DialogService>();
            
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<SignInDialogViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<LocalDirectorySetupViewModel>();
            SimpleIoc.Default.Register<CollectionLibraryViewModel>();
            SimpleIoc.Default.Register<DirectoryExplorerViewModel>();
            SimpleIoc.Default.Register<ExpandItemDialogViewModel>();
            SimpleIoc.Default.Register<TorrentManagerViewModel>();
        }
        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();
        public HomeViewModel Home => ServiceLocator.Current.GetInstance<HomeViewModel>();
        public LocalDirectorySetupViewModel LDSetup => ServiceLocator.Current.GetInstance<LocalDirectorySetupViewModel>();
        public CollectionLibraryViewModel CollectionFrame {
            get {
                if (!string.IsNullOrEmpty(_currentScanVMKey))
                    SimpleIoc.Default.Unregister(_currentScanVMKey);
                _currentScanVMKey = Guid.NewGuid().ToString();
                return ServiceLocator.Current.GetInstance<CollectionLibraryViewModel>(_currentScanVMKey);
            }
        }
        public DirectoryExplorerViewModel DirectoryExFrame => ServiceLocator.Current.GetInstance<DirectoryExplorerViewModel>();
        public ExpandItemDialogViewModel ExpandDialog => ServiceLocator.Current.GetInstance<ExpandItemDialogViewModel>();
        public TorrentManagerViewModel TorrentManager => ServiceLocator.Current.GetInstance<TorrentManagerViewModel>();
    }
}
