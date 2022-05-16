using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Loadout_Creator
{
    public sealed partial class PropertyGroupControl : UserControl
    {
        private bool collapsed;
        private bool hasloaded;

        public PropertyGroupControl(string title, AProperty[] properties)
        {
            Title = title;
            Properties = properties;
            this.InitializeComponent();
        }

        public string Title { get; }
        public AProperty[] Properties { get; }

        private void SymbolIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            collapsed = !collapsed;
            ArrowRotation.Rotation = collapsed ? 90 : -90;
            PropertyStack.Visibility=collapsed? Visibility.Collapsed : Visibility.Visible;
        }
        private static SolidColorBrush Gray = new SolidColorBrush(Colors.Gray);
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!hasloaded)
            {
                TitleBlock.Text = Title;
                foreach (var property in Properties)
                {
                    var c = property.GetControl();
                    c.HorizontalAlignment = HorizontalAlignment.Stretch;
                    PropertyStack.Children.Add(c);
                    if (property.AddLineAfter)
                    {
                        PropertyStack.Children.Add(new Rectangle() {Height=1,Margin=new Thickness(5),Fill=Gray });
                         //< Rectangle Fill = "Gray" Height = "2" Margin = "5" />
                    }

                }
                hasloaded = true;
            }
           
        }

     
    }
}
