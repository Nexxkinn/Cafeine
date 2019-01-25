using Cafeine.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace Cafeine.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePageViewModel Vm {
            get {
                return DataContext as HomePageViewModel;
            }
            set { }
        }

        public HomePage()
        {
            this.InitializeComponent();
        }
    }
    public static class FocusExtension
    {
        public static bool GetIsFocused(Control obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(Control obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
            "IsFocused", typeof(bool), typeof(FocusExtension),
            new PropertyMetadata(false, OnIsFocusedPropertyChanged));

        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;

            if ((bool)e.NewValue != (bool)e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    control.Focus(FocusState.Programmatic);
                    control.LostFocus += Control_LostFocus;
                }
                else
                {
                    control.GotFocus += Control_GotFocus;
                }
            }
        }

        private static void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            var control = (Control)sender;
            control.SetValue(IsFocusedProperty, true);
            control.GotFocus -= Control_GotFocus;
        }

        private static void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            var control = (Control)sender;
            control.SetValue(IsFocusedProperty, false);
            control.LostFocus -= Control_LostFocus;
        }
    }
}
