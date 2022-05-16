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
    public sealed partial class ColorControl : UserControl
    {
        private readonly ColorProperty cp;

        public ColorControl(ColorProperty cp)
        {
            this.cp = cp;
            this.InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Tag)
            {
                case "Change":
                    ColorPickerDialog cpd = new ColorPickerDialog();
                    if (cp.Value != null)
                        cpd.color = ((SolidColorBrush)cp.Value).Color;
                    else
                        cpd.color = Color.FromArgb(255, 255, 255, 255);
                    cpd.ColorPicked += Cpd_ColorPicked;
                    cpd.ShowAsync();
                    break;
                case "Clear":
                    cp.Value = null;
                    UpdateColor();
                    break;
            }
        }
       
        public void UpdateColor()
        {
            if (cp.Value == null)
                canvas.Background = null;
            else
                canvas.Background = (SolidColorBrush)cp.Value;
            foreach (Line item in canvas.Children)
            {
                item.Visibility = cp.Value==null ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void Cpd_ColorPicked(object sender, Color e)
        {
            cp.Value = new SolidColorBrush(e);
            UpdateColor();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColor();
        }
    }
}
