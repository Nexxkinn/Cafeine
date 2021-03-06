﻿using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Cafeine.Models
{
    public class ItemDetailsModel : ViewModelBase
    {
        private UserItem _item { get; set; }

        public UserItem Item {
            get { return _item; }
            set {
                if (_item != value)
                {
                    _item = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ItemLibraryModel _itembase;

        public ItemLibraryModel ItemBase {
            get { return _itembase; }
            set {
                if (_itembase != value)
                {
                    _itembase = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Episode> Episodelist { get; set; }

        #region mvvm setup properties

        public CafeineCommand PlusOneTotalSeenTextBlock { get; }

        public CafeineCommand EpisodeListsClicked { get; }

        public CafeineCommand EpisodeSettingsClicked { get; }

        public CafeineCommand UserItemDetailsClicked { get; }

        public AsyncReactiveCommand AddButtonClicked { get; }

        public AsyncReactiveCommand DeleteButtonClicked { get; }

        public CafeineProperty<double> ScorePlaceHolderRating { get; set; }

        public CafeineProperty<string> TitleTextBlock { get; }

        public CafeineProperty<string> StatusTextBlock { get; }

        public CafeineProperty<string> ScoreTextBlock { get; }

        public CafeineProperty<string> DescriptionTextBlock { get; }

        public ReactiveProperty<bool> LoadItemDetails { get; }

        public CafeineProperty<Visibility> LoadEpisodesListConfiguration { get; }

        public CafeineProperty<Visibility> LoadEpisodeLists { get; }

        public CafeineProperty<bool> LoadEpisodeSettings { get; }

        public CafeineProperty<bool> LoadEpisodeNotFound { get; }

        public CafeineProperty<bool> ItemDetailsProgressRing { get; }

        public CafeineProperty<bool> SetAddButtonLoad { get; }

        public ReactiveProperty<bool> SetDeleteButtonLoad { get; }

        public ReactiveProperty<bool> IsPaneOpened { get; }

        public CafeineProperty<Brush> PaneBackground { get; }

        public CafeineProperty<StorageFile> ImageSource { get; }
        #endregion

        #region mvvm TwoWay properties
        public CafeineProperty<int> TotalSeenTextBox { get; }

        public CafeineProperty<int> UserStatusComboBox { get; }
        #endregion

        public ItemLibraryModel SetItem(ItemLibraryModel item)
        {
            ItemBase = item;
            Item = ItemBase.Item;
            return item;
        }

        public ItemDetailsModel()
        {
            ItemBase = new ItemLibraryModel();
            Item = new UserItem();

            PaneBackground = new CafeineProperty<Brush>(new SolidColorBrush(Windows.UI.Colors.Transparent));
            IsPaneOpened = new ReactiveProperty<bool>(false);
            IsPaneOpened.Subscribe((ipo) =>
            {
                PaneBackground.Value = ipo
                    ? Application.Current.Resources["SystemControlBackgroundChromeMediumLowBrush"] as SolidColorBrush
                    : new SolidColorBrush(Windows.UI.Colors.Transparent);
            });

            #region Set initial value

            ImageSource = new CafeineProperty<StorageFile>();
            ItemDetailsProgressRing = new CafeineProperty<bool>();
            LoadItemDetails = new ReactiveProperty<bool>(false);
            LoadItemDetails.Subscribe(x => ItemDetailsProgressRing.Value = !x);

            TitleTextBlock = new CafeineProperty<string>();
            ScorePlaceHolderRating = new CafeineProperty<double>();
            StatusTextBlock = new CafeineProperty<string>();
            DescriptionTextBlock = new CafeineProperty<string>();
            ScoreTextBlock = new CafeineProperty<string>();

            Episodelist = new ObservableCollection<Episode>();
            LoadEpisodeLists = new CafeineProperty<Visibility>(Visibility.Collapsed);
            LoadEpisodeSettings = new CafeineProperty<bool>(false);
            LoadEpisodeNotFound = new CafeineProperty<bool>(false);
            LoadEpisodesListConfiguration = new CafeineProperty<Visibility>(Visibility.Visible);

            SetAddButtonLoad = new CafeineProperty<bool>(false);
            SetDeleteButtonLoad = new ReactiveProperty<bool>(true);
            SetDeleteButtonLoad.Subscribe(sdb =>
            {
                SetAddButtonLoad.Value = !sdb;
            });
            #endregion

            #region TwoWay Initial Value
            TotalSeenTextBox = new CafeineProperty<int>();
            UserStatusComboBox = new CafeineProperty<int>();
            #endregion

            PlusOneTotalSeenTextBlock = new CafeineCommand(() => TotalSeenTextBox.Value += 1);

            EpisodeListsClicked = new CafeineCommand(() =>
            {
                if (ItemBase.Episodes.Count == 0)
                {
                    LoadEpisodeNotFound.Value = true;
                    LoadEpisodeLists.Value = Visibility.Collapsed;
                }
                else
                {
                    LoadEpisodeNotFound.Value = false;
                    LoadEpisodeLists.Value = Visibility.Visible;
                }
                LoadEpisodeSettings.Value = false;
                LoadEpisodesListConfiguration.Value = Visibility.Visible;
            });

            EpisodeSettingsClicked = new CafeineCommand(() =>
            {
                LoadEpisodeLists.Value = Visibility.Collapsed;
                LoadEpisodeNotFound.Value = false;
                LoadEpisodeSettings.Value = true;
                LoadEpisodesListConfiguration.Value = Visibility.Collapsed;
            });

            UserItemDetailsClicked = new CafeineCommand(() => IsPaneOpened.Value = !IsPaneOpened.Value);

            AddButtonClicked = new AsyncReactiveCommand();
            AddButtonClicked.Subscribe(async _ =>
            {
                await Database.AddItem(ItemBase);
                Item = ItemBase.Item;
                RaisePropertyChanged(nameof(Item));
                SetDeleteButtonLoad.Value = true;
            });

            DeleteButtonClicked = new AsyncReactiveCommand();
            DeleteButtonClicked.Subscribe(async _ =>
            {
                MessageDialog popup = new MessageDialog($"You are going to delete {Item.Title} from your list.\n" +
                    $"Removing this item will also unlink your local directory in this item.\n" +
                    $"Do you really want to Remove it?", $"Remove this item?");
                popup.Commands.Add(new UICommand("Remove") { Id = 0 });
                popup.Commands.Add(new UICommand("Cancel") { Id = 1 });
                popup.DefaultCommandIndex = 0;
                popup.CancelCommandIndex = 1;
                var result = await popup.ShowAsync();
                if ((int)result.Id == 0)
                {
                    await Database.DeleteItem(ItemBase);
                    await navigationService.GoBack();
                }
            });

        }

        public ItemDetailsModel(ItemLibraryModel Item) : base()
        {
        }
    }
}
