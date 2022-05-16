using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class DataReferenceDialog : ContentDialog
    {
    //    private readonly SchemaData data;
        private readonly DataSetReference_DataColumn column;
        private List<ListView> ListViews = new List<ListView>();
        private List<DataElement> SelectedElements = new List<DataElement>();

        public DataReferenceDialog(SchemaData data,DataSetReference_DataColumn column)
        {
            //this.data = data;
            this.column = column;
            this.InitializeComponent();
            Title = "Data Reference";
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
            column.Value = SelectedElements;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        private void AddPivot(string title, string group = null)
        {
            DataElement[] elements;
            if (group == null)
            {
                elements = column.ReferencedDataSet.DataElements.Where(e => e.Groups.Count == 0).ToArray();
            }
            else
            {
                elements = column.ReferencedDataSet.DataElements.Where(e => e.Groups.Contains(group)).ToArray();
            }
            if (elements.Count() > 0)
            {
                var p = new PivotItem() { Header = title };
                var v = new ListView();
                v.SelectionMode = ListViewSelectionMode.Multiple;
                v.ItemTemplate = Resources["DataElement"] as DataTemplate;
                v.IsItemClickEnabled = true;
                v.ItemClick += ListView_ItemClick;
                p.Content = v;
                ListViews.Add(v);
                v.ItemsSource = elements;
                GroupPivot.Items.Add(p);

            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DataElement;
            if (SelectedElements.Contains(item))
                SelectedElements.Remove(item);
            else
                SelectedElements.Add(item);
            UpdateSelectedItems((ListView) sender);
        }
        public void UpdateSelectedItems(ListView current=null)
        {
            var lists = ListViews;
            if (current != null)
                lists = ListViews.Where(e=>e!=current).ToList();
            foreach (var listview in lists)
            {
                foreach (var item in listview.Items)
                {
                    var de = item as DataElement;
                    if (SelectedElements.Contains(de))
                    {
                        if(!listview.SelectedItems.Contains(de))
                            listview.SelectedItems.Add(de);
                    }
                    else //if(listview.SelectedItems.Contains(de)) removed because ICollection.remove wont throw an error if it doesnt contain it
                    {
                        listview.SelectedItems.Remove(de);
                    }
                }
            }
        }
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            AddPivot("Ungrouped");
            foreach (var group in column.ReferencedDataSet.Groups)
            {
                AddPivot(group, group);
            }
            if (column.Value != null)
                SelectedElements = (column.Value as List<DataElement>).ToList();
           
            UpdateSelectedItems();

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
          SelectedElements.Clear();
            UpdateSelectedItems();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedElements = column.ReferencedDataSet.DataElements.ToList();
            UpdateSelectedItems();
        }

       
    }
}
