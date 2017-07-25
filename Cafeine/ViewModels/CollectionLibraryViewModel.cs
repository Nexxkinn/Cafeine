using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafeine.Models;
using Cafeine.Services;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Cafeine.ViewModels {
    public class CollectionLibraryViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private ItemProperties itemproperty;
        public ItemProperties Itemproperty {
            get => itemproperty;

        }
        public string Image {
            get => itemproperty.Imgurl;
        }
        public string My_score {
            get { return Itemproperty.My_score.ToString(); }
            set {
                Itemproperty.My_score = Convert.ToInt32(value);
                this.OnPropertyChanged();
            }
        }
        public string My_watch {
            get { return Itemproperty.My_watch.ToString(); }
            set {
                Itemproperty.My_watch = Convert.ToInt32(value);
                this.OnPropertyChanged();
            }
        }
        public CollectionLibraryViewModel(ItemProperties itemProperties) {
            itemproperty = itemProperties;
        }

        public CollectionLibraryViewModel() {
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}