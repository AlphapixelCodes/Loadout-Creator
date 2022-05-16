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
    public sealed partial class NewQueryDialog : ContentDialog
    {
        private readonly SchemaData schema;
        private readonly Query query;
        private DataSet ComboSet=> DataSetCombo.SelectedItem as DataSet;
        private DataSetReference_DataColumn ComboReference=> DataReferenceCombo.SelectedItem as DataSetReference_DataColumn;

        public NewQueryDialog(SchemaData schema, Query query=null)
        {
            this.schema = schema;
            this.query = query;
            this.InitializeComponent();
           
        }
      
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var name = NameBox.Text;
            if (name.Length == 0)
            {
                ErrorBlock.Text = "Name Cannot Be Empty";
                args.Cancel = true;
            }else if (schema.Queries.QueryData.Any(z => z.Name == name))
            {
                ErrorBlock.Text = "A Querry Already Exists By That Name";
                args.Cancel = true;
            }
            else if (!name.Any(z => Char.IsLetterOrDigit(z))){
                ErrorBlock.Text = "Name Must Contain at Least One Letter Or Digit";
                args.Cancel = true;
            }
            else
            {
                var ordered = GroupsListView.SelectedItems.Cast<String>().OrderBy(x => x);
                var first = schema.Queries.QueryData.FirstOrDefault(z => z.Groups.OrderBy(x => x).SequenceEqual(ordered));
                if (first!=null) {
                    ErrorBlock.Text = $"\"{first.Name}\" Query already exists with this same parameters";
                    args.Cancel = true;
                }
                else
                {

                }
            }

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            DataSetCombo.ItemsSource = schema.Data.Sets;
            if (query != null)
            {

            }
            else
            {
                DataSetCombo.SelectedIndex = 0;
            }
        }

        private void DataSetCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataReferenceCombo.ItemsSource = ComboSet.Properties.Where(z => z is DataSetReference_DataColumn);
            DataReferenceCombo_SelectionChanged();
        }

        private void DataReferenceCombo_SelectionChanged(object sender=null, SelectionChangedEventArgs e=null)
        {
            if (DataReferenceCombo.SelectedIndex > -1) {
                GroupsListView.ItemsSource = ComboReference.ReferencedDataSet.Groups;
            }
            else
            {
                GroupsListView.ItemsSource = ComboSet.Groups;
            }
        }

        private void ClearDataSetReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            DataReferenceCombo.SelectedIndex = -1;
        }
    }
}
