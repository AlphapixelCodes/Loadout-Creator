using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using System.IO.Compression;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.System;
using System.Xml.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Loadout_Creator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageManagementPage : Page, INotifyPropertyChanged
    {
        public SchemaData SchemaData { get;private set; }
        public ObservableCollection<ImageData> SelectedImages=new ObservableCollection<ImageData>();
        public ImageGroupViewPage ImageGroupViewPage;
        
        private ListViewSelectionMode multiSelectMode = ListViewSelectionMode.Single;
        public ListViewSelectionMode MultiSelectMode { get => multiSelectMode;
        set
            {
                multiSelectMode = value;
                RaisePropertyChanged("MultiSelectMode");
            }
        }
        private int imageSize = 200;
        public int ImageSize  { get=>imageSize; set
            {
                imageSize = value;
                RaisePropertyChanged("ImageSize");
            }
        }
        public ImageManagementPage(SchemaData schemaData)
        {
            SchemaData = schemaData;
            this.InitializeComponent();
            
        }
        private void rootImagePage_Loaded(object sender, RoutedEventArgs e)
        {
            ImageGroupViewPage = new ImageGroupViewPage(this);
            MainImageGroupingViewFrame.Content = ImageGroupViewPage;
            MainImageGridView.ItemsSource = SchemaData.ImagesData.Images;
            GroupComboBox.ItemsSource= SchemaData.ImagesData.Groups;
            SelectedImageGrid.ItemsSource = SelectedImages;
            
            GroupComboBox.SelectionChanged += GroupComboBox_SelectionChanged;
            SelectedImages.CollectionChanged += SelectedImages_CollectionChanged;
            DeleteSelectedButton.Visibility = Visibility.Collapsed;
            SelectedImages_CollectionChanged();
            ImageGroupViewPage.Visibility = Visibility.Collapsed;
        }

        public void SelectedImages_CollectionChanged(object sender=null, System.Collections.Specialized.NotifyCollectionChangedEventArgs e=null)
        {
            var h = SelectedImageGrid.ActualHeight;
            var w = SelectedImageGrid.ActualWidth;
            var count = SelectedImageGrid.Items.Count;
            if (count == 1)
            {
                SelectedImageGrid.ItemHeight = h;
                SelectedImageGrid.DesiredWidth = w;
            }
            else if (count < 5)
            {
                SelectedImageGrid.ItemHeight = h / 2.0;
                SelectedImageGrid.DesiredWidth = w / 2.0;
            }
            else
            {
                SelectedImageGrid.ItemHeight = h / 3.0;
                SelectedImageGrid.DesiredWidth = w / 3.0;
            }
            SelectedImageCountBlock.Text = count.ToString();

            if (SelectedImages.Count == 0)
            {
                DeleteSelectedButton.Visibility= Visibility.Collapsed;
                GroupComboBox.SelectedIndex = -1;
                GroupComboBox.IsEnabled = false;
                AddGroupButton.IsEnabled= false;
                ClearGroupButton.IsEnabled = false;
            }
            else
            {
                DeleteSelectedButton.Visibility = Visibility.Visible;
                GroupComboBox.IsEnabled=true;
                AddGroupButton.IsEnabled = true;
                ClearGroupButton.IsEnabled = true;
                var firstgroup = SelectedImages.First().Group;
                GroupComboBox.SelectionChanged -= GroupComboBox_SelectionChanged;//temporarily removing the event so it doesnt change the groups
                if (firstgroup!=null && SelectedImages.All(z=>z.Group==firstgroup))//checking if they are all the first group
                {
                    GroupComboBox.SelectedItem = firstgroup;
                }
                else
                {
                    GroupComboBox.SelectedIndex = -1;
                }
                GroupComboBox.SelectionChanged += GroupComboBox_SelectionChanged;
            }    
        }

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ImageData item in SelectedImages)
            {
                item.Group = GroupComboBox.SelectedItem as string;
            }
            //removing unused groups
            foreach (var group in SchemaData.ImagesData.Groups.ToArray())
            {
                if (!SchemaData.ImagesData.Images.Any(z => z.Group == group))
                    SchemaData.ImagesData.Groups.Remove(group);
            }
            UpdateGrouping();
        }

        private async void Upload_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");


            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
            
            if (files != null && files.Count > 0)
            {
                foreach (var item in files)
                { 
                var idata= SchemaData.ImagesData.AddImage(item);
                    Debug.WriteLine(idata.ID + " Done");
                }
                UpdateGrouping();

            }
            else
            {
                Debug.WriteLine("Canceled");
            }
        }

        private void AdaptiveGridViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!GroupToggleButton.IsChecked.Value)
            {
                foreach (ImageData img in e.AddedItems)
                {
                    SelectedImages.Add(img);
                }
                foreach (ImageData img in e.RemovedItems)
                {
                    SelectedImages.Remove(img);
                }
            }
        }
        

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var b=sender as ToggleButton;
            switch (b.Content)
            {
                case "Group":
                    SelectedImages.Clear();
                    if (b.IsChecked.Value)
                    {
                        MainImageGridView.ItemsSource = null;
                        MainImageGridView.Visibility = Visibility.Collapsed;
                        ImageGroupViewPage.Visibility = Visibility.Visible;
                        ImageGroupViewPage.UpdateGrouping();
                        
                    }
                    else
                    {
                        ImageGroupViewPage.Visibility = Visibility.Collapsed;
                        ImageGroupViewPage.Clear();
                        MainImageGridView.Visibility = Visibility.Visible;
                        MainImageGridView.ItemsSource = SchemaData.ImagesData.Images;
                    }
                    break;
                case "Multi Select":
                    MultiSelectMode = (b.IsChecked.Value)? ListViewSelectionMode.Multiple:ListViewSelectionMode.Single;
                    break;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Tag)
            {
                case "Clear":
                    GroupComboBox.SelectedIndex = -1;
                    UpdateGrouping();
                    break;
                case "Add":
                    var p=new PromptDialog("Group Name");
                    p.Validation = new PromptDialog.ValidationDelegate(GroupNameValidator);
                    p.ConfirmedEvent+= (a, b) =>
                    {
                        SchemaData.ImagesData.Groups.Add(b);
                        GroupComboBox.SelectedItem = b;
                    };
                    p.ShowAsync();
                    break;
            }
        }
        public Tuple<bool,string> GroupNameValidator(string text)
        {
            if (text.Any(z => char.IsLetterOrDigit(z)))
                return new Tuple<bool, string>(!SchemaData.ImagesData.Groups.Contains(text), "A Group Already Exists By That Name");
            else
                return new Tuple<bool, string>(false, "Name Must Contain Atleast One Letter Or Digit.");
        }

        private void UpdateGrouping()
        {

            if (ImageGroupViewPage.Visibility == Visibility.Visible)
            {
                ImageGroupViewPage.UpdateGrouping();
                try
                {
                    SelectedImages.Clear();
                }
                catch
                {

                }
            }
        }

        private void DeleteSelectedGroupsButton_Click(object sender=null, RoutedEventArgs e=null)
        {
            var cd=new ConfirmDialog("Delete Images", $"Are you sure you want to delete {SelectedImages.Count} images?");
            cd.ConfirmEvent += (a, b) =>
            {
                foreach (var item in SelectedImages.ToArray())
                {
                    SchemaData.ImagesData.Images.Remove(item);
                    item.Dispose();
                }
                SelectedImages.Clear();
                GroupComboBox_SelectionChanged(null, null);
                UpdateGrouping();
                GC.Collect();
            };
            cd.ShowAsync();

        }

        private void ImageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ImageSize = new int[] { 100, 150, 200 }[(sender as ComboBox).SelectedIndex];

        private async void MoreButtonFlyout_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuFlyoutItem).Text)
            {
                case "Import Data":
                    var picker=new FileOpenPicker();
                    picker.FileTypeFilter.Add(".zip");
                    StorageFile zip=await picker.PickSingleFileAsync();
                    try
                       {
                    await Storage.DeleteFile(zip.Name,Storage.TempFolder);
                    /*if (await Storage.FileExists(zip.Name, Storage.TempFolder))
                    {
                        (await Storage.GetFile(zip.Name, Storage.TempFolder)).DeleteAsync();
                    }*/
                    var copiedzip=await zip.CopyAsync(Storage.TempFolder);
                    await Storage.DeleteFolder("TemporaryImages", Storage.TempFolder);
                    //StorageApplicationPermissions.FutureAccessList.Add(zip);
                    ZipFile.ExtractToDirectory(copiedzip.Path, Storage.TempFolder.Path + "\\TemporaryImages");
                    copiedzip.DeleteAsync();
                    var extractedFolder = await Storage.GetFolder("TemporaryImages", Storage.TempFolder);
                    var data = await FileIO.ReadTextAsync(await Storage.GetFile("ImagesData.xml",extractedFolder));
                    var xData = XElement.Parse(data);

                    string[] groups = xData.Element("Groups").Elements("Group").Select(x => x.Value).ToArray();
                    //ImageData[] images=.Select(z=>ImageData.FromXElement(z)).ToArray();
                    foreach (var xImg in xData.Element("Images").Elements("ImageData"))
                    {
                        var pngName = xImg.Attribute("id").Value + ".png";
                        if (await Storage.FileExists(pngName, extractedFolder))
                        {
                            await SchemaData.ImagesData.AddImage(xImg,await Storage.GetFile(pngName,extractedFolder));

                        }
                        else
                        {
                            Debug.WriteLine(xImg.ToString());
                        }
                    }
                    extractedFolder.DeleteAsync();
                    UpdateGrouping();
                    }
                    catch
                    {
                        new ConfirmDialog("Import Error", "Unable to import data.", true).ShowAsync();
                    }
                    //delete zip files that may have been left in storage 
                    
                    break;
                case "Export Data":
                    var folderPicker = new FolderPicker();
                    folderPicker.FileTypeFilter.Add("*");
                    StorageFolder ZipLocation =await folderPicker.PickSingleFolderAsync();

                    if (ZipLocation != null)
                    {
                        var x = SchemaData.ImagesData.GetXElement();
                        StorageFolder f=await Storage.LocalCache.CreateFolderAsync("TemporaryImages",CreationCollisionOption.ReplaceExisting);
                        var id = await f.CreateFileAsync("ImagesData.xml");
                        await FileIO.WriteTextAsync(id, x.ToString());
                        
                        foreach (var imagedata in SchemaData.ImagesData.Images.ToArray())
                        {
                            var imgfile = await f.CreateFileAsync(imagedata.ID+".png"); 
                            using (IRandomAccessStream stream = await imgfile.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                                encoder.SetSoftwareBitmap(imagedata.SoftwareBitmap);
                                await encoder.FlushAsync();
                            }
                            Debug.WriteLine(imgfile.Path);
                        
                        }

                        var savename = "Images Data.zip";
                        int i = 0;
                        while (await Storage.FileExists(savename, ZipLocation))
                        {
                            i++;
                            savename = $"Images Data ({i}).zip";
                        }

                        if (await Storage.FileExists("TempZip.zip", Storage.TempFolder))
                        {
                            await (await Storage.GetFile("TempZip.zip", Storage.TempFolder)).DeleteAsync();
                        }
                        
                        ZipFile.CreateFromDirectory(f.Path, Storage.TempFolder.Path + "\\TempZip.zip", CompressionLevel.Optimal, false);
                        var tempzip =await Storage.TempFolder.GetFileAsync("TempZip.zip");
                        await tempzip.MoveAsync(ZipLocation,savename);
                        
                        Storage.DeleteFolder("TemporaryImages", Storage.TempFolder);
                        
                        var flo=new FolderLauncherOptions();
                        flo.ItemsToSelect.Add(await Storage.GetFile(savename, ZipLocation));
                        Launcher.LaunchFolderAsync(ZipLocation,flo);
                    }
                    break;
                case "Delete All":
                    var cd = new ConfirmDialog("Delete All Images", $"Are you sure you want to delete all {SchemaData.ImagesData.Images.Count} images?");
                    cd.ConfirmEvent += (a, b) =>
                    {
                        foreach (var item in SchemaData.ImagesData.Images)
                        {
                            item.Dispose();
                        }
                        SchemaData.ImagesData.Images.Clear();
                        SelectedImages.Clear();
                        GroupComboBox_SelectionChanged(null, null);
                        UpdateGrouping();
                        GC.Collect();
                    };
                    cd.ShowAsync();
                    break;
            }
        }
    }
    
}
