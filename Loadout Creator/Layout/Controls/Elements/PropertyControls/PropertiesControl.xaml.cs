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
    public sealed partial class PropertiesControl : UserControl
    {
        public PropertiesControl()
        {
            this.InitializeComponent();
        }
        public void LoadGroups(AElement element)
        {
            TitleBlock.Text = element.DisplayName;
            foreach (var pgc in MainStack.Children.OfType<PropertyGroupControl>().ToArray())
            {
                MainStack.Children.Remove(pgc);
                
            }
            //GC.Collect();
            foreach (var group in element.PropertyGroups)
            {
                MainStack.Children.Add(group);
                
            }
        }
    }
}
