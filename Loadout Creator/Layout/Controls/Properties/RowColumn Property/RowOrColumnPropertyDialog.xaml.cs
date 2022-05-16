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
    public sealed partial class RowOrColumnPropertyDialog : ContentDialog
    {
        private readonly RowColumnProperty prop;
        private List<RowColData> items;
        public RowOrColumnPropertyDialog(RowColumnProperty prop)
        {
            this.prop = prop;
            this.InitializeComponent();
            
        }

     

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var content = button.Content as string;
            if (content == "Add")
            {
                items.Add(new RowColData());
            }
            else if (RowColListView.SelectedIndex > -1)
            {
                var data = RowColListView.SelectedItem as RowColData;
                switch (content)
                {
                    case "Delete":
                        items.Remove(data);
                    break;
                    case "Edit":
                        Hide();
                        var ercdd=new EditRowColDataDialog(data,prop.isRow);
                        ercdd.Title = prop.isRow ? "Edit Row" : "Edit Column";
                        await ercdd.ShowAsync();
                        ShowAsync();
                        break;
                }
            }
            RowColListView.ItemsSource = null;
            RowColListView.ItemsSource = items;

        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            items=prop.Value as List<RowColData>;
            RowColListView.ItemsSource = items;

            Title = prop.isRow ? "Rows" : "Columns";
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var index = RowColListView.SelectedIndex;
            if (index > -1)
            {
                var og = items[index];
                switch (((MenuFlyoutItem)sender).Text)
                {
                    case "Move Up":
                        if (index > 0)
                        {
                            items.Remove(og);
                            items.Insert(index-1, og);
                        }
                        break;
                    case "Move Down":
                        if (index < items.Count - 1)
                        {
                            items.Remove(og);
                            items.Insert(index + 1, og);
                        }
                        break;
                }
                RowColListView.ItemsSource = null;
                RowColListView.ItemsSource = items;
            }
        }
    }
}
