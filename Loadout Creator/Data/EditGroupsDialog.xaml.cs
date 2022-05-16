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
        public readonly DataSet dataSet;
        public event EventHandler GroupsChanged;

        public EditGroupsDialog(DataSet dataSet)
        {
            this.dataSet = dataSet;
            this.InitializeComponent();
            
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void editGroupsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            GroupComboBox.ItemsSource = dataSet.Groups;
            if(dataSet.Groups.Count>0)
            GroupComboBox.SelectedIndex = 0;
        }

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(GroupComboBox.SelectedIndex>-1)
                NameBox.Text=GroupComboBox.SelectedItem as string;
        }
        public bool groupAlreadyExistsByThatName() => dataSet.Groups.Contains(NameBox.Text);
     
        private void Butt_Click(object sender, RoutedEventArgs e)
        {
            var group = GroupComboBox.SelectedItem as string;
            if (group != null) {
                switch ((sender as Button).Tag)
                {
                    case "Rename":
                        if (!groupAlreadyExistsByThatName())
                        {
                            dataSet.RenameGroup(group,NameBox.Text);
                            GroupsChanged?.Invoke(null, null);
                        }
                        break;
                    case "Delete":
                        dataSet.RemoveGroup(group);
                        GroupsChanged?.Invoke(null, null);
                        break;
                }
            }
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorBlock.Text = groupAlreadyExistsByThatName() ? "Group already Exists By That Name" : "";
        }
    }
}
