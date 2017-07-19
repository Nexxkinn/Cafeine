using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafeine.Models;
using Windows.Storage;
using System.Runtime.CompilerServices;

namespace Cafeine.ViewModels {
    class LocalDirectorySetupViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private LocalDirectorySetupLocalFoldersModel localdirectorysetupfoldermodel;
        public LocalDirectorySetupLocalFoldersModel Localdirectorysetupfoldermodel => localdirectorysetupfoldermodel;

        public LocalDirectorySetupItems LinkedFolder {
            get { return Localdirectorysetupfoldermodel.LinkedItems; }
            set {
                if (Localdirectorysetupfoldermodel.LinkedItems != value) {
                    Localdirectorysetupfoldermodel.LinkedItems = value;
                    this.OnPropertyChanged();
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
