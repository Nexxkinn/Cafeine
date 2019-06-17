using Cafeine.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Views.Wizard
{
    public sealed partial class OfflineItemWizard : ContentDialog,INotifyPropertyChanged
    {
        public bool IsCanceled { get; private set; }

        public OfflineItem Result { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _stage { get; set; }
        private int Stage {
            get => _stage;
            set {
                if(_stage != value)
                {
                    _stage = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(NotEqual));
                }
            }
        }

        private bool _isButtonLoaded { get; set; }
        private bool IsButtonLoaded {
            get => _isButtonLoaded;
            set {
                if(_isButtonLoaded != value)
                {
                    _isButtonLoaded = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isNextButtonEnabled { get; set; }
        private bool IsNextButtonEnabled {
            get => _isNextButtonEnabled;
            set {
                if (_isNextButtonEnabled != value)
                {
                    _isNextButtonEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool NotEqual(int num) => Stage != num;

        private ServiceItem Item { get; set; }
        private string Pattern { get; set; }
        private StorageFolder Folder { get; set; }


        public OfflineItemWizard(ServiceItem item,string pattern,ICollection<ContentList> lists)
        {
            this.IsButtonLoaded = true;
            this.Item = item;
            this.Pattern = pattern;
            this.InitializeComponent();
        }

        public void RaisePropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void CancelButtonClicked()
        {
            IsCanceled = true;
            this.Hide();
        }

        private void FinishedButtonClicked()
        {
            this.Hide();
        }
        private void PreviousButtonClicked()
        {
            Stage--;
        }
        private void NextButtonClicked()
        {
            Stage++;
        }
        private void Stage0_load()
        {
            this.Title = "Get Started";
            IsNextButtonEnabled = true;
        }
        private void Stage1_load()
        {
            this.Title = "Browse Folder";
            Folder = null;
            IsNextButtonEnabled = false;
        }
        private void Stage1_BrowseFolder()
        {

        }

        private void dump()
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
