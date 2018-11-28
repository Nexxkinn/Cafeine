using Reactive.Bindings;
using Reactive.Bindings.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

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
