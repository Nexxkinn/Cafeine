using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Cafeine.Views.Wizard
{
    public sealed partial class BrowseButton : UserControl, INotifyPropertyChanged
    {
        private bool _isFolderAssigned;
        private string _foldername;
        private StorageFolder StorageFolder;

        public bool IsFolderAssigned {
            get => _isFolderAssigned;
            private set => Set(ref _isFolderAssigned,value);
        }
        public string FolderName {
            get => _foldername;
            set => Set(ref _foldername,value);
        }
        public event EventHandler<StorageFolder> StorageFolderSelected;
        public event PropertyChangedEventHandler PropertyChanged;

        public BrowseButton()
        {
            this.InitializeComponent();
        }

        private void BrowseButton_DragEnter(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;
            e.DragUIOverride.Caption = "Place here";
            e.DragUIOverride.IsCaptionVisible = true;
        }
        private async void BrowseButton_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();

                var Folder = items.OfType<StorageFolder>().FirstOrDefault();
                if (Folder != null) SetStorageFolder(Folder);
            }
        }

        private async void BrowseButton_Clicked(object sender, PointerRoutedEventArgs e)
        {
            var folderpicker = new FolderPicker();
            folderpicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            folderpicker.FileTypeFilter.Add("*");

            var Folder = await folderpicker.PickSingleFolderAsync();
            if (Folder != null) SetStorageFolder(Folder);
        }

        private void SetStorageFolder(StorageFolder folder)
        {
            IsFolderAssigned = true;
            StorageFolder = folder;
            FolderName = folder.DisplayName;
            StorageFolderSelected?.Invoke(null, StorageFolder);
        }

        private void RaisePropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        private bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            RaisePropertyChanged(name);
            return true;
        }
    }
}
