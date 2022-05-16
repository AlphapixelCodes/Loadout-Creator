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
    public sealed partial class RowOrColumnPropertyControl : UserControl
    {
        private readonly RowColumnProperty rcp;

        public RowOrColumnPropertyControl(RowColumnProperty rcp)
        {
            this.rcp = rcp;
            this.InitializeComponent();
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await new RowOrColumnPropertyDialog(rcp).ShowAsync();
            UserControl_Loaded(null, null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (rcp.Value != null)
                CountBlock.Text = "Count: " + ((List<RowColData>)rcp.Value).Count;
            else
            {
                CountBlock.Text = "Count: 0";
                rcp.Value = new List<RowColData>();
            }

        }
    }
}
