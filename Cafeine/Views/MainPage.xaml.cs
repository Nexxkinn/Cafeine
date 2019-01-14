using Cafeine.Models;
using Cafeine.Services;
using Cafeine.ViewModels;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http.Filters;

//using Html2Markdown;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace Cafeine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel Vm {
            get {
                return DataContext as MainPageViewModel;
            }
        }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
