using Cafeine.Models;
using Cafeine.Services;
using Cafeine.Services.FilenameParser;
using Cafeine.Services.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Cafeine.ViewModels.Wizard
{
    public class OfflineItemWizardViewModel : ViewModelBase
    {
        private OfflineItem Result;

        #region properties
        public int Stage {
            get => _stage;
            set { if (Set(ref _stage,value)) RaisePropertyChanged(nameof(NotEqual)); }
        }
        public bool IsButtonLoaded {
            get => _isButtonLoaded;
            set => Set(ref _isButtonLoaded, value);
        }
        public bool IsNextButtonEnabled {
            get => _isNextButtonEnabled;
            set => Set(ref _isNextButtonEnabled, value);
        }
        public string Title {
            get => _title;
            set => Set(ref _title, value);
        }
        public string Message {
            get => _message;
            set => Set(ref _message, value);
        }
        public bool Stage1_ViewResults {
            get => _stage1_viewresult;
            set => Set(ref _stage1_viewresult,value);
        }
        #endregion

        #region fields
        private int _stage;
        private bool _isButtonLoaded;
        private bool _isNextButtonEnabled;
        private string _title;
        private string _message;
        private bool _stage1_viewresult;
        #endregion

        public Visibility DisplayWarningVisibility;
        private bool IsDisplayWarning {
            get => this.DisplayWarningVisibility == Visibility.Visible;
            set {
                DisplayWarningVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged(nameof(DisplayWarningVisibility));
            }
        }
        public ICollection<ContentList> OnlineList { get; set; }
        public ObservableCollection<ContentList> MatchedList { get; set; }
        public ObservableCollection<ContentList> UnmatchedList { get; set; }
        public bool NotEqual(int num) => this.Stage != num;

        private ServiceItem ServiceItem { get; }
        private string Pattern { get; }
        private StorageFolder Folder { get; set; }

        public OfflineItemWizardViewModel(ServiceItem item, string pattern, ICollection<ContentList> lists)
        {
            this.ServiceItem = item;
            this.Pattern = pattern;
            this.IsButtonLoaded = true;
            this.IsDisplayWarning = false;
            this.OnlineList = lists;
            this.MatchedList = new ObservableCollection<ContentList>();
            this.UnmatchedList = new ObservableCollection<ContentList>();
        }

        public OfflineItem GetResult() => Result;

        public void PreviousButtonClicked() => Stage--;

        public void NextButtonClicked() => Stage++;

        public void Stage0_load()
        {
            Title = "Get Started";
            IsNextButtonEnabled = true;
        }

        public void Stage1_load()
        {
            Title = "Browse Folder";
            Folder = null;
            IsButtonLoaded = true;
            Stage1_ViewResults = false;
            IsNextButtonEnabled = false;
        }

        public void Stage2_load()
        {
            Title = "Combining...";
            IsNextButtonEnabled = false;
            _ = Stage2_GenerateOfflineItem();
        }


        public async void Stage1_TryProcessFolder(object sender,StorageFolder folder)
        {

            // initial setup
            Folder = folder;
            MatchedList.Clear();
            UnmatchedList.Clear();
            Stage1_ViewResults = true;

            var files = await folder.GetFilesAsync();
            
            // first pass : check if files exist
            if (files.Count == 0)
            {
                DisplayWarning("No files exists in this folder");
                return;
            }
            
            string Message = string.Empty;
            foreach(StorageFile file in files)
            {
                var  parser  = new CafeineFilenameParser(file);
                bool isNumParsed = parser.TryGetEpisodeUsingAnitomy(out int? episode_num, out string[] fingerprint, out string[] uniq);
                
                var File = new File()
                {
                    FileName = file.DisplayName,
                    Fingerprint = fingerprint,
                    Unique_Numbers = uniq
                };

                if (isNumParsed)
                {
                    var item = new ContentList
                    {
                        Number = episode_num.Value,
                        Files = new List<File>() { File }
                    };
                    MatchedList.Add(item);
                }
                else
                {
                    // consider this file as a special file.
                    // EDs, OPs, Movies, etc.
                    var item = new ContentList
                    {
                        Number = -1,
                        Title = File.FileName,
                        Files = new List<File>() { File }
                    };
                    UnmatchedList.Add(item);
                }
            }
            MatchedList = new ObservableCollection<ContentList>(MatchedList.OrderBy(x => x.Number));
            RaisePropertyChanged(nameof(MatchedList));
            if ( UnmatchedList.Count != 0) Message += $"{UnmatchedList.Count} file(s) are unable to identify";

            // final setup
            IsNextButtonEnabled = true;
        }

        public async Task Stage2_GenerateOfflineItem()
        {
            await Task.Yield();
            var CombinedContent = new List<ContentList>();
            var onlinelist = new List<ContentList>(OnlineList);
            // Notify : add unmatched file.
            Message += "Adding unmatched file(s)\n";
            CombinedContent.AddRange(UnmatchedList);

            // group matched based on number
            // Notify : Trying to group contents
            Message += "Trying to group contents\n";
            var groupedContentList = MatchedList.GroupBy(x => x.Number);
            var group = new List<ContentList>();
            foreach(var num in groupedContentList)
            {
                List<File> files = num.Select(x => x.Files[0]).ToList();
                ContentList file = new ContentList()
                {
                    Files = files,
                    Number = num.Key
                };
                Message += $"Episode {num.Key} files has been grouped\n";
                group.Add(file);

            }

            // combine matched list with online
            foreach(var m_list in group)
            {
                var onlineitem = onlinelist.FirstOrDefault(x => x.Number == m_list.Number);
                if( onlineitem == null )
                {
                    CombinedContent.Add(m_list);
                    Message += $"offline item(s) for episode {m_list.Number} has no equivalent online list. consider using its first filename as a Title.\n";
                    // Notify : episode {m_list.Number} has no equivalent online list.
                    //          consider using its first filename as a Title.
                }
                else
                {
                    var item = new ContentList()
                    {
                        Files = m_list.Files,
                        Number = onlineitem.Number,
                        Title = onlineitem.Title,
                        StreamingServices = onlineitem.StreamingServices,
                        Thumbnail = onlineitem.Thumbnail,
                    };
                    CombinedContent.Add(item);
                    onlinelist.Remove(onlineitem);
                    Message += $"offline item(s) for episode {m_list.Number} Found matched online item\n";
                    // Notify : Found matched episode with item
                    // add notification
                }
            }

            // add the rest
            Message += $"Adding rest of unmatched online list.\n";
            CombinedContent.AddRange(onlinelist);

            // done
            CombinedContent = CombinedContent.OrderBy(x => x.Number).ToList();

            // await Database.CreateOflineItem(ServiceItem, CombinedContent).ConfigureAwait(false);
            Message += $"Operation done successfully.\n";
            // final setup
            Title = "Finished";
            IsButtonLoaded = false;
        }

        public void DisplayWarning(string message)
        {
            Message = message;
            IsDisplayWarning = true;
        }
        private void EpisodeListControl_DeleteClick(object sender, RoutedEventArgs e)
        {

        }

    }
}
