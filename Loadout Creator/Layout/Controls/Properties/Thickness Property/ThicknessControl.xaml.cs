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
    public sealed partial class ThicknessControl : UserControl
    {
        public ThicknessControl(ThicknessProperty thick)
        {
            Thick = thick;
            this.InitializeComponent();
            
        }

        public ThicknessProperty Thick { get; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Thick.ShowIcons)
            {
                foreach (var item in MainGrid.Children.OfType<SymbolIcon>())
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
            if (Thick.Value == null)
                Thick.Value = new Thickness();
            var t=(Thickness)Thick.Value;
            LeftThick.Text = t.Left.ToString();
            RightThick.Text = t.Right.ToString();
            DownThick.Text = t.Bottom.ToString();
            UpThick.Text = t.Top.ToString();

            UpThick.TextChanged += _TextChanged;
            DownThick.TextChanged += _TextChanged;
            LeftThick.TextChanged += _TextChanged;
            RightThick.TextChanged += _TextChanged;

        }

        private void _TextChanged(object sender, TextChangedEventArgs e)
        {
            
            Thick.Value = new Thickness(double.Parse(LeftThick.Text),double.Parse(UpThick.Text),double.Parse(RightThick.Text),double.Parse(DownThick.Text));
        }

        private void _BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {

            args.Cancel = (!double.TryParse(args.NewText, out _)) || (!Thick.AllowNegatives && args.NewText.Contains("-"));
        }
    }
}
