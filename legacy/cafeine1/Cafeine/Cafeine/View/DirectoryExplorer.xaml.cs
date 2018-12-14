﻿using System;
using System.Collections.Generic;
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
using Cafeine.Model;
using Cafeine.ViewModel;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cafeine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DirectoryExplorer : Page
    {
        public DirectoryExplorerViewModel Vm => (DirectoryExplorerViewModel)DataContext;
        public DirectoryExplorer()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Vm.directory = (VirtualDirectory)e.Parameter;
            Vm.updateitem();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            VirDirInterface.ItemsSource = null;
            GC.Collect();
        }
    }
}