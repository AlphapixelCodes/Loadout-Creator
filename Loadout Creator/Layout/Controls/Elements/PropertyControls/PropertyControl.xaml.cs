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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Loadout_Creator
{
    public sealed partial class PropertyControl : UserControl
    {
        private readonly AProperty prop;
        private readonly FrameworkElement fe;
        private bool loaded=false;
        public PropertyControl(AProperty prop, FrameworkElement e)
        {
            this.prop = prop;
            this.fe = e;
            this.InitializeComponent();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                NameBlock.Text = prop.Name;
                fe.SetValue(Grid.ColumnProperty, 1);
                MainGrid.Children.Add(fe);
                NameBlock.Margin = new Thickness(prop.TabSize, 0, 0, 0);
                loaded= true;
            }
        }
  
    }
}
