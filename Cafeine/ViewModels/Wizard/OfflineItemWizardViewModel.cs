using Cafeine.Models;
using Cafeine.Services.FilenameParser;
using Cafeine.Services.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Cafeine.ViewModels.Wizard
{
    public class OfflineItemWizardViewModel : ViewModelBase
    {
        private OfflineItem Result;

        private int _stage;
        public int Stage {
            get => _stage;
            set { if (Set(ref _stage,value)) RaisePropertyChanged(nameof(NotEqual)); }
        }

        private bool _isButtonLoaded;
        public bool IsButtonLoaded {
            get => _isButtonLoaded;
            set => Set(ref _isButtonLoaded, value);
        }

        private bool _isNextButtonEnabled;
        public bool IsNextButtonEnabled {
            get => _isNextButtonEnabled;
            set => Set(ref _isNextButtonEnabled, value);
        }

        private string _title;
        public string Title {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _message;
        public string Message {
            get => _message;
            set => Set(ref _message, value);
        }

        private bool _stage1_viewresult;
        public bool Stage1_ViewResults {
            get => _stage1_viewresult;
            set => Set(ref _stage1_viewresult,value);
        }

        public Visibility DisplayWarningVisibility;
        private bool IsDisplayWarning {
            get => this.DisplayWarningVisibility == Visibility.Visible;
            set {
                DisplayWarningVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged(nameof(DisplayWarningVisibility));
            }
        }

        public ObservableCollection<ContentList> MatchedList { get; set; }
        public ObservableCollection<ContentList> UnmatchedList { get; set; }
        public bool NotEqual(int num) => this.Stage != num;

        private ServiceItem Item { get; }
        private string Pattern { get; }
        private StorageFolder Folder { get; set; }

        public OfflineItemWizardViewModel(ServiceItem item, string pattern, ICollection<ContentList> lists)
        {
            this.Item = item;
            this.Pattern = pattern;
            this.IsButtonLoaded = true;
            this.IsDisplayWarning = false;

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
            Stage1_ViewResults = false;
            IsNextButtonEnabled = false;
        }

        public void Stage2_load()
        {
            Title = "Finished";
            IsButtonLoaded = false;
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
                    var item = new ContentList
                    {
                        Number = -1,
                        Files = new List<File>() { File }
                    };
                    UnmatchedList.Add(item);
                }
            }
            MatchedList = new ObservableCollection<ContentList>(MatchedList.OrderBy(x => x.Number));
            RaisePropertyChanged(nameof(MatchedList));
            if ( UnmatchedList.Count != 0) Message += $"{UnmatchedList.Count} file(s) are unable to identify";
        }
        public void DisplayWarning(string message)
        {
            Message = message;
            IsDisplayWarning = true;
        }
        private void EpisodeListControl_DeleteClick(object sender, RoutedEventArgs e)
        {

        }
        public void dump()
        {
            // (?<=\-\s)(?:\d+)
            //string NamePattern;
            //foreach (var file in localfiles)
            //{
            //    int number = Convert.ToInt32(Regex.Match(file.DisplayName, @"(?<=\-\s)(?:\d+)", RegexOptions.IgnoreCase).Value);
            //    if (files.TryGetValue(number, out StorageFile storageFile))
            //    {
            //        // display warning
            //        // display "Use <title> for next file?

            //    }

            //}
        }

    }
}
