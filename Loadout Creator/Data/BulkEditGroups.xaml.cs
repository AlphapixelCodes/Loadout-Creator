using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public sealed partial class BulkEditGroups : ContentDialog
    {
        public DataSet DataSet { get; }

        public BulkEditGroups(DataSet dataSet)
        {

            this.InitializeComponent();
            DataSet = dataSet;
        }

        private DataElement GetDataElementByName(string name)
        {
            return DataSet.Data.First(e => e.Name == name);
        }
        private void UpdateSelectedGroups() {
            var currentGroup = GroupCombo.SelectedItem as string;
            DataListView.SelectedItems.Clear();
            foreach(DataElement item in DataListView.Items)
            {
                if (item.Groups.Contains(currentGroup))
                    DataListView.SelectedItems.Add(item);
            }
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            GroupCombo.ItemsSource = DataSet.Groups;
            foreach (var item in DataSet.Data)
            {
                DataListView.Items.Add(item);
            }
            
            GroupCombo.SelectedIndex = 0;
        }

        private void GroupCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedGroups();
        }

        private void DataListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(e.AddedItems);
            var g = GroupCombo.SelectedItem as string;
            foreach (DataElement item in e.AddedItems)
            {
                item.AddGroup(g);
            }
            foreach (DataElement item in e.RemovedItems)
            {
                item.RemoveGroup(g);
            }
        }
    }
}
