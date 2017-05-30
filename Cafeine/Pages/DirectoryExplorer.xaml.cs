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
using Cafeine.Properties;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Pages
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
            var DataReceived = (VirtualDirectory)e.Parameter;

            List<VirtualDirectory> Dir = new List<VirtualDirectory>();
            try
            {
                switch (DataReceived.DirectoryType)
                {
                    case 1:
                    {
                        Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 4, DirectoryTitle = "Watching" });
                        Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 5, DirectoryTitle = "On Hold" });
                        Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 6, DirectoryTitle = "Planned to Watch" });
                        Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 7, DirectoryTitle = "Completed" });
                        Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 8, DirectoryTitle = "Dropped" });
                        break;
                    }
                    case 2:
                    {
                        Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 4, DirectoryTitle = "Reading" });
                        Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 5, DirectoryTitle = "On Hold" });
                        Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 6, DirectoryTitle = "Planned to Read" });
                        Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 7, DirectoryTitle = "Completed" });
                        Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 8, DirectoryTitle = "Dropped" });
                        break;
                    }
                }
            }
            catch(NullReferenceException)
            {
                Dir.Add(new VirtualDirectory { DirectoryType = 1, DirectoryTitle = "Anime" });
                Dir.Add(new VirtualDirectory { DirectoryType = 2, DirectoryTitle = "Manga" });
            }
            VirDirInterface.ItemsSource = Dir;

            //Title.Text = DataReceived.Item_Title;
            //User_Rating.Text = DataReceived.My_score.ToString();
            //BitmapImage bitmapImage = new BitmapImage() { UriSource = new Uri(BaseUri, DataReceived.Imgurl) };
            //image.Source = bitmapImage;

            /// Proof of Concept - Parse Data Straight from MyAnimelist page
            /// Requirement : RetreiveItemDetail(Item Id, AnimeOrManga)
            /// 
            ///Task.Run(async () => await ExpandDetail(DataReceived.Id));

        }
        public void NavigateItemtoPage(object sender, ItemClickEventArgs e)
        {
            var SelectedItem = (VirtualDirectory)e.ClickedItem;
            if (SelectedItem.AnimeOrManga.HasValue) Frame.Navigate(typeof(CollectionLibrary), SelectedItem); //check if it has a bool value
            else Frame.Navigate(typeof(DirectoryExplorer), SelectedItem); //navigate if it doesn't.
        }
    }
}
