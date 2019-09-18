using Cafeine.Design;
using Cafeine.Model;
using Cafeine.ViewModel;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine
{
    public sealed partial class ExpandItemDetails : ContentDialog
    {
        public ExpandItemDialogViewModel Vm => (ExpandItemDialogViewModel)DataContext;
        public ExpandItemDetails()
        {
            this.InitializeComponent();
        }
    }
}
