using Cafeine.Models;
using Cafeine.Services.Mvvm;
using Cafeine.ViewModels.Wizard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine.Views.Wizard
{
    public sealed partial class OfflineItemWizard : ContentDialog
    {
        public bool IsCanceled { get; private set; }

        public OfflineItem Result => Viewmodel.GetResult();

        private OfflineItemWizardViewModel Viewmodel => this.DataContext as OfflineItemWizardViewModel;

        public OfflineItemWizard(ServiceItem item,string pattern,ICollection<ContentList> lists)
        {
            this.DataContext = new OfflineItemWizardViewModel(item,pattern,lists);
            this.InitializeComponent();
        }

        private void CancelButtonClicked()
        {
            IsCanceled = true;
            this.Hide();
            Viewmodel.Dispose();
        }

        private void FinishedButtonClicked()
        {
            Viewmodel.Dispose();
            this.Hide();
        }

        private void EpisodeListControl_DeleteClick(object sender, RoutedEventArgs e)
        {
            Viewmodel.MatchedList.Remove((sender as Button).DataContext as ContentList);
        }

        private void EpisodeNotOnListControl_DeleteClick(object sender, RoutedEventArgs e)
        {
            Viewmodel.UnmatchedList.Remove((sender as Button).DataContext as ContentList);
        }

        private void OnList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {

        }
    }
}
