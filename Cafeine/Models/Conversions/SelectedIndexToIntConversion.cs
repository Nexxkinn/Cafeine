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
    public class SelectedIndexToIntConversion : ReactiveConverter<SelectionChangedEventArgs, int>
    {
        protected override IObservable<int> OnConvert(IObservable<SelectionChangedEventArgs> source)
        {
            return source.Select(x => (x.OriginalSource as Pivot).SelectedIndex);
        }
    }
}
