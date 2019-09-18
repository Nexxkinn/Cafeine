using Cafeine.Design;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine {
    public sealed partial class SignInDialog : ContentDialog {
        public SignInDialog() {
            this.InitializeComponent();
        }
        public string u = string.Empty;
        public string p = string.Empty;
        //public string s;
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            u = usrnm.Text;
            p = psswd.Password;
            //s= 1;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {

        }
    }
}
