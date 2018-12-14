using Windows.UI.Xaml.Controls;
using Cafeine.ViewModel;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine {
    public sealed partial class LocalDirectorySetup : ContentDialog {
        public LocalDirectorySetupViewModel Vm => (LocalDirectorySetupViewModel)DataContext;
        public LocalDirectorySetup() {
            this.InitializeComponent();
        }
    }
}
