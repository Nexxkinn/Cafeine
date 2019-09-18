using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafeine.Model;
using Windows.Storage;
using System.Runtime.CompilerServices;

namespace Cafeine.ViewModels {
    class LocalDirectorySetupViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private LocalDirectorySetupLocalFoldersModel localdirectorysetupfoldermodel;
        public LocalDirectorySetupLocalFoldersModel Localdirectorysetupfoldermodel => localdirectorysetupfoldermodel;
        public int LinkedItemID {
            get { return Localdirectorysetupfoldermodel.ItemID; }
            set {
                if (Localdirectorysetupfoldermodel.ItemID != value) {
                    Localdirectorysetupfoldermodel.ItemID = value;
                    this.OnPropertyChanged();
                }
            }
        }
        public string LinkedFolderName {
            get { return Localdirectorysetupfoldermodel.FolderName; }
            set {
                if (Localdirectorysetupfoldermodel.FolderName != value) {
                    Localdirectorysetupfoldermodel.FolderName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string LinkedItemName {
            get { return Localdirectorysetupfoldermodel.LinkedItem; }
            set {
                if (Localdirectorysetupfoldermodel.LinkedItem != value) {
                    Localdirectorysetupfoldermodel.LinkedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public LocalDirectorySetupViewModel(LocalDirectorySetupLocalFoldersModel localdirectorysetupmodels) {
            localdirectorysetupfoldermodel = localdirectorysetupmodels;
        }

        public LocalDirectorySetupViewModel() {
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
