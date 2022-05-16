using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class BulkGroupDialog : ContentDialog
    {
        private DataSet Set;
        private bool recordChanges = false;
        
        public BulkGroupDialog(DataSet set)
        {
            Set = set;
            this.InitializeComponent();
        }

      
     
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            
           DataElementsListView.ItemsSource = Set.DataElements;
            GroupsCombo.ItemsSource = Set.Groups;
            GroupsCombo.SelectedIndex = 0;
            

        }

        private void GroupsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recordChanges = false;
            DataElementsListView.SelectedItems.Clear();
            var selectedGroup = GroupsCombo.SelectedItem as string;
            foreach (var item in Set.DataElements.Where(z => z.Groups.Contains(selectedGroup)))
            {
                DataElementsListView.SelectedItems.Add(item);
            }
            recordChanges = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DataElementsListView.SelectedItems.Clear();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            DataElementsListView.SelectAll();
        }

        private void DataElementsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (recordChanges)
            {
                var selectedGroup = GroupsCombo.SelectedItem as string;
                foreach (DataElement de in e.AddedItems)
                {
                    de.Groups.Add(selectedGroup);
                }
                foreach (DataElement de in e.RemovedItems)
                {
                    de.Groups.Remove(selectedGroup);
                }
            }
        }
    }
}
