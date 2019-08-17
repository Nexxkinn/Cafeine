using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Cafeine.Views.Resources
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var sv = sender as ScrollViewer;
            var item = sv.FindName("CControl") as Grid;
            var scrollpropertyset = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(sv);
            var composition = scrollpropertyset.Compositor;
            var offset = composition.CreateExpressionAnimation("-america.Translation.Y");
            offset.SetReferenceParameter("america", scrollpropertyset);

            var itemoption = ElementCompositionPreview.GetElementVisual(item);
            itemoption.StartAnimation("Offset.Y", offset);
        }
    }
}
