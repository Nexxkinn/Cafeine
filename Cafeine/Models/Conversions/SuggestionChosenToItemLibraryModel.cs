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
    class SuggestionChosenToItemLibraryModel : ReactiveConverter<AutoSuggestBoxSuggestionChosenEventArgs, ItemLibraryModel>
    {
        protected override IObservable<ItemLibraryModel> OnConvert(IObservable<AutoSuggestBoxSuggestionChosenEventArgs> source)
        {
            return source.Select(x => x.SelectedItem as ItemLibraryModel);
        }
    }
}
