using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Reactive.Bindings.Interactivity;
using Windows.UI.Xaml.Controls;
using Reactive.Bindings;

namespace Cafeine.Models.Conversions
{
    public class ItemClickToItemLibraryModel : ReactiveConverter<ItemClickEventArgs, ItemLibraryModel>
    {
        protected override IObservable<ItemLibraryModel> OnConvert(IObservable<ItemClickEventArgs> source)
        {
            return source.Select(x => x.ClickedItem as ItemLibraryModel);
        }
    }
}
