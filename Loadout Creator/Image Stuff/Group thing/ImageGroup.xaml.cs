using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public sealed partial class ImageGroup : UserControl
    {
        public string GroupName;
        private List<ImageData> images;
        private readonly ImageManagementPage imageManagementPage;
        

        public ImageGroup(string groupname ,List<ImageData> images, ImageManagementPage imageManagementPage)
        {
            this.GroupName = groupname;
            this.images = images;
            this.imageManagementPage = imageManagementPage;
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GridView.ItemsSource= images;
            images = null;
            NameBlock.Text = GroupName == null ? "Ungrouped":GroupName;

            GridView.SetBinding(ListViewBase.SelectionModeProperty, new Binding() {Source=imageManagementPage,Path=new PropertyPath("MultiSelectMode") });
            GridView.SetBinding(AdaptiveGridView.DesiredWidthProperty, new Binding() { Source = imageManagementPage, Path = new PropertyPath("ImageSize") });
            GridView.SetBinding(AdaptiveGridView.ItemHeightProperty, new Binding() { Source = imageManagementPage, Path = new PropertyPath("ImageSize") });
        }

   
        public void UpdateGroup(List<ImageData> images)
        {
            GridView.ItemsSource = images;
        }
        public void Rename(string name)
        {
            if (GroupName != null)
                imageManagementPage.SchemaData.ImagesData.Groups.Remove(GroupName);
            GroupName = name;
            NameBlock.Text = GroupName;
            foreach (var item in GridView.ItemsSource as List<ImageData>)
            {
                item.Group = name;
            }
            imageManagementPage.SchemaData.ImagesData.Groups.Add(GroupName);
            
        }

      

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imageManagementPage.MultiSelectMode == ListViewSelectionMode.Single)
            {
                foreach (var item in imageManagementPage.ImageGroupViewPage.ImageGroups)
                {
                    if (item != this && item.GroupName!=GroupName)
                    {
                        item.ClearSelected();
                    }
                }
                imageManagementPage.SelectedImages.CollectionChanged -= imageManagementPage.SelectedImages_CollectionChanged;
                imageManagementPage.SelectedImages.Clear();
                imageManagementPage.SelectedImages.CollectionChanged += imageManagementPage.SelectedImages_CollectionChanged;
            }
            foreach (ImageData item in e.AddedItems)
            {
                if(!imageManagementPage.SelectedImages.Contains(item))
                    imageManagementPage.SelectedImages.Add(item);
            }
            foreach (ImageData item in e.RemovedItems)
            {
                imageManagementPage.SelectedImages.Remove(item);
            }
        }

        private void ClearSelected()
        {
            GridView.SelectedIndex = -1;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            switch((sender as MenuFlyoutItem).Text)
            {
                case "Rename":
                    var pd=new PromptDialog("Rename Group", GroupName) { Validation= imageManagementPage.GroupNameValidator};
                    pd.ConfirmedEvent += (a, text) => Rename(text);
                    pd.ShowAsync();
                    break;
            } 
        }
    }
}
