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

namespace Cafeine.ViewModels
{
    public class CollectionLibraryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ItemProperties itemproperty;

        public ItemProperties Itemproperty
        {
            get { return itemproperty; }
            
        }
        public int My_score
        {
            get { return Itemproperty.My_score; }
            set
            {
                if (Itemproperty.My_score != value)
                {
                    Itemproperty.My_score = value;
                    this.OnPropertyChanged();
                }
            }
        }
        public CollectionLibraryViewModel(ItemProperties itemProperties)
        {
            itemproperty = itemProperties;
        }

        public CollectionLibraryViewModel()
        {
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
