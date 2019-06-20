using Cafeine.Models;
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
            IsNextButtonEnabled = false;
            MatchedList = new ObservableCollection<ContentList>();
            UnmatchedList = new ObservableCollection<ContentList>();
        }

        public void Stage2_load()
        {
            Title = "Finished";
            IsButtonLoaded = false;
        }

        public async void Stage1_BrowseFolder()
        {
            var folderpicker = new FolderPicker();
            folderpicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            folderpicker.FileTypeFilter.Add("*");

            Folder = await folderpicker.PickSingleFolderAsync();
            if (Folder == null) return;

            _ = Stage1_TryProcessFolder();
        }

        public async Task Stage1_TryProcessFolder()
        {

            var files = await Folder.GetFilesAsync();
            // first pass : check if files exist
            if (files.Count == 0)
            {
                DisplayWarning("No files exists in this folder");
                return;
            }
            
            string Message = string.Empty;
            int MismatchCount = 0; 
            Regex R_eps_num = new Regex(@"(?<=\-\s)(?:\d+)", RegexOptions.IgnoreCase);
            foreach(StorageFile file in files)
            {
                if (R_eps_num.IsMatch(file.DisplayName))
                {
                    int num = Convert.ToInt32(R_eps_num.Match(file.DisplayName).Value);
                    var item = new ContentList
                    {
                        Number = num,
                        FileName = new List<string>() { file.Name }
                    };
                    MatchedList.Add(item);
                }
                else
                {
                    MismatchCount++;
                    var item = new ContentList
                    {
                        Number = -1,
                        FileName = new List<string>() { file.Name }
                    };
                    UnmatchedList.Add(item);
                }
            }
            await Task.Yield();
            MatchedList = new ObservableCollection<ContentList>(MatchedList.OrderBy(x => x.Number));
            RaisePropertyChanged(nameof(MatchedList));
            // second pass : check if parser finds at least one file
            if ( MismatchCount != 0) Message += $"{MismatchCount} file(s) are unable to identify";

            // third pass : check if there are multiple series title
            // fourth pass : check if there are "duplicate" episode numbers
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
