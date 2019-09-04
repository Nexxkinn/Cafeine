﻿using Cafeine.Services;
using Cafeine.Services.Mvvm;
using Cafeine.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
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
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace Cafeine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserAuthenticationPage : BasePage
    {
        public BrowserAuthenticationPage()
        {
            this.InitializeComponent();
        }
    }
}
