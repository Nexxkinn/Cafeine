using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Cafeine.Model;

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

        /// <summary>
        /// This property can be used to force the application to run with design time data.
        /// </summary>
        public static bool UseDesignTimeData {
            get {
                return false;
            }
        }

        static ViewModelLocator() {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var nav = new NavigationService();
            nav.Configure(SignInPageKey, typeof(SignInDialog));
            nav.Configure("HomePage", typeof(HomePage));
            SimpleIoc.Default.Register<INavigationService>(() => nav);

            var F = new CafeineNavigationService();
            F.Configure("VirDir", typeof(DirectoryExplorer));
            F.Configure("Collection", typeof(CollectionLibraryFrame));
            SimpleIoc.Default.Register<ICafeineNavigationService>(() => F);

            SimpleIoc.Default.Register<IDialogService, DialogService>();
            
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<SignInDialogViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<LocalDirectorySetupViewModel>();
            SimpleIoc.Default.Register<CollectionLibraryViewModel>();
            SimpleIoc.Default.Register<DirectoryExplorerViewModel>();
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
        public CollectionLibraryViewModel CollectionFrame => ServiceLocator.Current.GetInstance<CollectionLibraryViewModel>();
        public DirectoryExplorerViewModel DirectoryExFrame => ServiceLocator.Current.GetInstance<DirectoryExplorerViewModel>();
    }
}
