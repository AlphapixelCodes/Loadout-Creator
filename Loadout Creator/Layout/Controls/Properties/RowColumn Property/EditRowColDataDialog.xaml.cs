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
    public sealed partial class EditRowColDataDialog : ContentDialog
    {
        public event EventHandler<RowColData> ConfirmedEvent;
        private readonly RowColData data;
        private readonly bool isRow;

        public EditRowColDataDialog(RowColData data,bool isRow)
        {
            this.data = data;
            this.isRow = isRow;
            this.InitializeComponent();
            
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var val = ValueNBox.Value;
            if(val.Equals(double.NaN))
                val = 1;
            data.Value = new GridLength(val, (GridUnitType)ValueCombo.SelectedIndex);
            data.Min = (int)MinNBox.Value;
            data.Max = (int)MaxNBox.Value;
            ConfirmedEvent?.Invoke(this,data);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

     

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ValueBlock.Text = isRow ? "Height" : "Width";

            ValueNBox.Value = data.Value.Value;
            ValueCombo.SelectedIndex = (int)data.Value.GridUnitType;

            if (data.Min >=0)
                MinNBox.Value = data.Min;

            if (data.Max >= 0)
                MaxNBox.Value = data.Max;
            

        }
    }
}
