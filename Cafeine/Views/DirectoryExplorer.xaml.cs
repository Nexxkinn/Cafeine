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
using Cafeine.Models;
using Cafeine.ViewModels;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DirectoryExplorer : Page
    {
        public DirectoryExplorer()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Receive passed data from previous page
            var DataReceived = DirectoryExplorerViewModel.DefaultDirectory((VirtualDirectory)e.Parameter);
            VirDirInterface.ItemsSource = DataReceived;


        }
        public void NavigateItemtoPage(object sender, ItemClickEventArgs e)
        {
            var SelectedItem = (VirtualDirectory)e.ClickedItem;
            if (SelectedItem.AnimeOrManga != AnimeOrManga.Directory) Frame.Navigate(typeof(CollectionLibrary), SelectedItem); //check if it has a bool value
            else Frame.Navigate(typeof(DirectoryExplorer), SelectedItem); //navigate if it doesn't.
        }
    }
}