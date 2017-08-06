using Cafeine.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Cafeine.Model {
    class AutoSuggestBoxTextChanged : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            // cast value to whatever EventArgs class you are expecting here
            var args = (AutoSuggestBoxQuerySubmittedEventArgs)value;
            // return what you need from the args
            return (GroupedSearchResult)args.ChosenSuggestion;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
    class LinkedItemSuggestionTextChanged : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            // cast value to whatever EventArgs class you are expecting here
            var args = (AutoSuggestBoxQuerySubmittedEventArgs)value;
            // return what you need from the args
            return (LocalDirectorySetupItems)args.ChosenSuggestion;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
    class LinkedFolderItemClicked : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var args = (ItemClickEventArgs)value;
            return (localDirectorySetup)args.ClickedItem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
    class ItemClicked : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var args = (ItemClickEventArgs)value;
            return (CollectionLibrary)args.ClickedItem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
