using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafeine.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Command;

namespace Cafeine.ViewModel
{
    public class DirectoryExplorerViewModel : ViewModelBase
    {
        private VirtualDirectory _directory = null;
        public VirtualDirectory directory{
            get { return _directory; }
            set { Set(ref _directory, value); }
        }

        private List<VirtualDirectory> _dataReceived = new List<VirtualDirectory>();
        public List<VirtualDirectory> DataReceived {
            get {
                return _dataReceived;
            }
            set {
                Set(ref _dataReceived, value);
            }
        }
        private ICafeineNavigationService navigationservice;

        public DirectoryExplorerViewModel(ICafeineNavigationService nav) {
            navigationservice = nav;
        }
        public void updateitem() {
            DataReceived = DefaultDirectory(directory);
        }
        public static List<VirtualDirectory> DefaultDirectory(VirtualDirectory e)
        {
            List<VirtualDirectory> Dir = new List<VirtualDirectory>();
            try
            {
                switch (e.DirectoryType)
                {
                    case 1:
                        {
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 4, DirectoryTitle = "Watching" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 6, DirectoryTitle = "On Hold" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 9, DirectoryTitle = "Planned to Watch" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 5, DirectoryTitle = "Completed" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 7, DirectoryTitle = "Dropped" });
                            break;
                        }
                    case 2:
                        {
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 4, DirectoryTitle = "Reading" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 6, DirectoryTitle = "On Hold" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 9, DirectoryTitle = "Planned to Read" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 5, DirectoryTitle = "Completed" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 7, DirectoryTitle = "Dropped" });
                            break;
                        }
                }
            }
            catch (NullReferenceException)
            {
                Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.Directory, DirectoryType = 1, DirectoryTitle = "Anime" });
                Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.Directory, DirectoryType = 2, DirectoryTitle = "Manga" });
                //Dir.Add for custom folder ( DirectoryType = 3 ), and always navigate to CollectionLibrary filtered.
            }
            return Dir;
        }
        private RelayCommand<VirtualDirectory> _directoryItemClick;
        public RelayCommand<VirtualDirectory> DirectoryItemClick {
            get {
                return _directoryItemClick
                    ?? (_directoryItemClick = new RelayCommand<VirtualDirectory>(
                    p => {
                        if (p.AnimeOrManga != AnimeOrManga.Directory) navigationservice.NavigateTo("Collection",p); //check if it has a bool value
                        else navigationservice.NavigateTo("VirDir", p); //navigate if it doesn't.
                    }));
            }
        }

    }
}