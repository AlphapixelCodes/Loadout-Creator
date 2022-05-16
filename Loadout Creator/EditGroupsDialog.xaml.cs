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
    public sealed partial class EditGroupsDialog : ContentDialog
    {
        private readonly DataSet currentSet;
        private string currentGroup => GroupCombo.SelectedItem as string;
        public EditGroupsDialog(DataSet currentSet)
        {
            this.InitializeComponent();
            this.currentSet = currentSet;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            GroupCombo.ItemsSource=currentSet.Groups;
            if(currentSet.Groups.Count>0)
                GroupCombo.SelectedIndex = 0;
            
        }

        private void GroupCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var g = currentGroup;
            ErrorBlock.Visibility = Visibility.Collapsed;
            if (g == null)
                NameBox.Text = "";
            else
                NameBox.Text = g;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var g = currentGroup;
            if (g != null)
            {
                currentSet.Groups.Remove(g);
                foreach (DataElement item in currentSet.DataElements)
                {
                    item.Groups.Remove(g);
                }
            }
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            var newName = NameBox.Text;
            if (currentSet.Groups.Contains(newName))
            {
                ErrorBlock.Visibility=Visibility.Visible;
            }
            else
            {
                ErrorBlock.Visibility = Visibility.Collapsed;
                var ogname = currentGroup;
                foreach (var item in currentSet.DataElements)
                {
                    if (item.Groups.Remove(ogname))
                        item.Groups.Add(newName);
                }
                currentSet.Groups.Remove(ogname);
                currentSet.Groups.Add(newName);
                GroupCombo.SelectedItem = newName;
            }
        }
    }
}
