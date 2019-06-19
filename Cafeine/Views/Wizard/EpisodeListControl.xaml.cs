using Cafeine.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class EpisodeListControl : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler DeleteClicked {
            add => this.DeleteButton.Click += value;
            remove => this.DeleteButton.Click -= value;
        }
        private ContentList Item => this.DataContext as ContentList;
        public EpisodeListControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item)));
        }

        public bool LoadEpisodeNumber {
            get { return (bool)GetValue(LoadEpisodeNumberProperty); }
            set { SetValue(LoadEpisodeNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadEpisodeNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadEpisodeNumberProperty =
            DependencyProperty.Register("LoadEpisodeNumber", typeof(bool), typeof(EpisodeListControl), null);

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
