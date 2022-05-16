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
    public sealed partial class DataReferenceSetPicker : ContentDialog
    {
        private readonly DataSet currentSet;
        private readonly DataSetReference_DataColumn column;
        public event EventHandler ConfirmedEvent;

        public DataReferenceSetPicker(DataSet set,DataSetReference_DataColumn column)
        {
            this.InitializeComponent();
            this.currentSet = set;
            this.column = column;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        
            column.ReferencedDataSet=Combo.SelectedItem as DataSet;
            ConfirmedEvent?.Invoke(null, null);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Combo.ItemsSource = currentSet.Parent.Sets.Where(s => s != currentSet);
            Combo.SelectedIndex = 0;
        }
    }
}
