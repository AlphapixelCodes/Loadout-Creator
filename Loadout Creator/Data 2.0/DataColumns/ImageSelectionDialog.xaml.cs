using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Loadout_Creator
{
    public sealed partial class ImageSelectionDialog : ContentDialog
    {
        private readonly SchemaData schema;
        private ImageData selectedImageData;
        public event EventHandler<ImageData> ConfirmedEvent;
        public ImageSelectionDialog(SchemaData schema)
        {
            this.schema = schema;
            this.InitializeComponent();
            
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ConfirmedEvent?.Invoke(this, selectedImageData);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        private Expander GetExpander(string name, List<ImageData> imgs)
        {
            var ret = new Expander();
            ret.Header = name;
            var agv=new AdaptiveGridView();
            agv.ItemsSource = imgs;
            agv.ItemTemplate = Resources["dataTemplate"] as DataTemplate;
            agv.ItemHeight = 150;
            agv.DesiredWidth = 150;
            agv.SelectionChanged += Agv_SelectionChanged;
            ret.Content = agv;
            return ret;
        }

        private void Agv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedImageData = e.AddedItems[0] as ImageData;
            Img.Source = selectedImageData.Image;
            PrimaryButtonText = "Ok";
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var g = schema.ImagesData.GetGroups();
            if (g.Item1.Count > 0)
            {
                MainStack.Children.Add(GetExpander("Ungrouped", g.Item1));
            }
            foreach (var item in g.Item2.OrderBy(z=>z.Key))
            {
                MainStack.Children.Add(GetExpander(item.Key, item.Value));
            }
        }
    }
}
