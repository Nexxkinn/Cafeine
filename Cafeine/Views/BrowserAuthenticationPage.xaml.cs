using Cafeine.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Reactive.Linq;
using Windows.Web.Http.Filters;
using Cafeine.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserAuthenticationPage : Page
    {
        public BrowserAuthenticationPage()
        {
            this.InitializeComponent();
            
        }
        public BrowserAuthenticationPageViewModel Vm {
            get {
                return DataContext as BrowserAuthenticationPageViewModel;
            }
        }
        private void CloseFrame()
        {
            this.Frame.GoBack();
            
        }
        
    }
}
