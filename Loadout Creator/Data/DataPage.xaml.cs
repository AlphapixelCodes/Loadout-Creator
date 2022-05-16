using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Loadout_Creator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataPage : Page
    {
        public ObservableCollection<DataSet> dataSets => StaticData.DataSets;
        public DataSet currentDataSet => DataSetsListBox.SelectedIndex == -1 ? null : (DataSetsListBox.SelectedItem as DataSet);
        public DataPage()
        {
            this.InitializeComponent();
        }

        private async void DataSetsButtons_Click(object sender, RoutedEventArgs e)
        {
            var tag = "";
            if (sender is Button)
                tag = ((Button)sender).Tag.ToString();
            else
                tag = ((MenuFlyoutItem)sender).Tag.ToString();

            switch (tag) {
                case "Add":
                    var pd = new PromptDialog("Data Set Name");
                    pd.Validation = (b) => new Tuple<bool, string>(!StaticData.DataSets.Any(z => z.Name == b), "Dataset already exists by that name.");
                    pd.ConfirmedEvent += (a, b) => DataSetsListBox.SelectedItem=StaticData.AddDataSet(b); 
                    
                    pd.ShowAsync();
                    break;
                case "Import":
                    
                    var pdImport = new PromptDialog("Paste Data") { MultiLine = true };
                    pdImport.ConfirmedEvent += (a, text) =>
                    {
                        try
                        {
                            pdImport.Hide();
                            var x = XElement.Parse(text);
                            foreach (var item in x.Elements("DataSet"))
                            {
                                var newDataSet = new DataSet(item);
                                var query = StaticData.DataSets.FirstOrDefault(z => z.Name == newDataSet.Name);
                                if (query != null)
                                        StaticData.DataSets.Remove(query);
                                StaticData.DataSets.Add(newDataSet);
                            }
                         }
                        catch
                        {
                            pdImport.Closed += (z, b) => new ConfirmDialog("Data Import Failed", "XML Error", true).ShowAsync();

                            return;
                        }
                        pdImport.Closed+=(z,b)=> new ConfirmDialog("Data Import Successfull", "", true).ShowAsync();
                        UpdateGroupCheckBoxes();
                    };
                    pdImport.ShowAsync();
                    break;
                case "Export":
                    new PromptDialog("Export (Copy)", string.Join("\n", "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + StaticData.GetXElement().ToString())) { MultiLine = true, PrimaryButtonText = "", SecondaryButtonText = "Close" }.ShowAsync();
                    break;

            }
        }
        private async void DataSetsFlyout_Click(object sender, RoutedEventArgs e)
        {
            var fly = sender as MenuFlyoutItem;
            var DS = fly.DataContext as DataSet;
            switch (fly.Tag)
            {
                case "Rename":
                    if (DataSetsListBox.SelectedItem != null)
                    {
                        var selected = DataSetsListBox.SelectedItem as DataSet;
                        var p = new PromptDialog("Rename Data Set", selected.Name);
                        p.Validation = (b) => new Tuple<bool, string>(!StaticData.DataSets.Any(z => z.Name == b), "Dataset already exists by that name.");
                        p.ConfirmedEvent += (a, b) => {
                            selected.Name = b;
                        };

                        p.ShowAsync();
                    }
                    break;
                case "Delete":
                    if (DataSetsListBox.SelectedItem != null)
                    {
                        var selected = DataSetsListBox.SelectedItem as DataSet;
                        var cd = new ConfirmDialog("Delete Data Set", "Are you sure you want to delete " + selected.Name + "?");
                        await cd.ShowAsync();
                        if (cd.Result)
                        {
                            StaticData.DataSets.Remove(selected);
                            DataElementsView.ItemsSource = currentDataSet;
                        }
                    }
                    break;
            }
        }

        private void DataSetListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataset = (DataSetsListBox.SelectedItem as DataSet);
            if (dataset != null)
            {
                DataElementsView.ItemsSource = dataset.Data;
                CurrentDataSetGroupFilterComboBox.ItemsSource = dataset.Groups;
                NoDataSetSelectedGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoDataSetSelectedGrid.Visibility=Visibility.Visible;
            }
            UpdateGroupCheckBoxes();
        }
        private void UpdateGroupCheckBoxes()
        {
            GroupsCheckboxStack.Children.Clear();
            if (currentDataSet != null)
            {
                foreach (var group in currentDataSet.Groups.OrderBy(z => z))
                {
                    var checkbox = new CheckBox();
                    checkbox.Content = group;
                    checkbox.Checked += (a, b) =>
                    {
                        DataElement selected = DataElementsView.SelectedItem as DataElement;
                        if (selected != null && !selected.Groups.Contains(group))
                        {
                            selected.AddGroup(group);
                        }
                    };
                    checkbox.Unchecked += (a, b) =>
                    {
                        DataElement selected = DataElementsView.SelectedItem as DataElement;
                        if (selected != null && selected.Groups.Contains(group))
                        {
                            selected.RemoveGroup(group);
                        }
                    };
                    GroupsCheckboxStack.Children.Add(checkbox);
                }
            }
            UpdateGroupCheckBoxChecked();
        }
        /// <summary>
        /// Updates the group checkboxes to match the dataElement's groups
        /// </summary>
        private void UpdateGroupCheckBoxChecked()
        {
            DataElement selected = DataElementsView.SelectedItem as DataElement;
            if (selected!=null)
            {
                foreach (var checkbox in GroupsCheckboxStack.Children.OfType<CheckBox>())
                {
                    checkbox.IsChecked = selected.Groups.Contains((string)checkbox.Content);
                }
            }
        }
        private void DataClassView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGroupCheckBoxChecked();
            DataElement selected=DataElementsView.SelectedItem as DataElement;
            if (selected != null)
            {
                NoDataElementSelectedGrid.Visibility = Visibility.Collapsed;
                DataElementNameBox.Text =selected.Name;
                DataElementNameBox.IsEnabled = true;
            }
            else
            {
                NoDataElementSelectedGrid.Visibility = Visibility.Visible;
                
                DataElementNameBox.Text = "";
                DataElementNameBox.IsEnabled = false;
            }
            
            
            
           
        }
        private void GroupButtons_Click(object sender,RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (currentDataSet != null)
            {
                switch (button.Tag)
                {
                    case "Add":
                        var pd = new PromptDialog("Group Name");
                        pd.Validation = (txt) => new Tuple<bool, string>(!currentDataSet.Groups.Contains(txt), "A Group already exists by that name.");
                        pd.ConfirmedEvent += (a, txt) =>
                        {
                            currentDataSet.Groups.Add(txt);
                            UpdateGroupCheckBoxes();
                        };
                        pd.ShowAsync();
                        break;
                    case "Edit":
                        var egd = new EditGroupsDialog(currentDataSet);
                        egd.GroupsChanged += (a, b) => UpdateGroupCheckBoxes();
                        egd.ShowAsync();
                        break;
                    case "BulkEdit":
                        if (currentDataSet.Groups.Count > 0)
                            new BulkEditGroups(currentDataSet).ShowAsync();
                        else
                            new ConfirmDialog("No Groups in Data Set", "There must be groups in the Data Set to Bulk Edit.", true).ShowAsync();
                        break;
                }
            }
            else
            {
                new ConfirmDialog("No Data Set Selected","Must Select a data set.",true).ShowAsync();
            }
        }

        private void SaveDataClassButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RenameDataElement_Click(object sender, RoutedEventArgs e)
        {
            if (DataElementsView.SelectedItem != null) {
                var SelectedElement = DataElementsView.SelectedItem as DataElement;
                var p = new PromptDialog("Rename Data Element", SelectedElement.Name);
                p.Validation = (txt) => new Tuple<bool, string>(!currentDataSet.Data.Any(z => z.Name == txt),"Name is already taken in data set.");
                p.ConfirmedEvent += (a, txt) => { 
                    SelectedElement.Name = txt;
                    DataElementNameBox.Text = txt;
                };
                p.ShowAsync();
            }
        }
      
        private void DataElementsButtons_Click(object sender, RoutedEventArgs e)
        {

            if (DataSetsListBox.SelectedIndex > -1)
            {
                switch ((sender as Button).Tag)
                {
                    case "Add":
                        var pd = new PromptDialog("Data Element Name");
                        pd.Validation = (txt) => new Tuple<bool, string>(!currentDataSet.Data.Any(z => z.Name == txt), "Name is already taken in data set.");
                        pd.ConfirmedEvent += (a, txt) => currentDataSet.Data.Add(new DataElement() { Name = txt });
                        pd.ShowAsync();
                        break;
                    case "Import":
                        var pdImport=new PromptDialog("Paste Data") { MultiLine=true};
                        pdImport.ConfirmedEvent += (a, text) =>
                        {
                            try
                            {
                                var x=XElement.Parse(text);
                                foreach (var xDE in x.Elements("DataElement"))
                                {
                                    var de = new DataElement(xDE);   
                                    foreach (var g in de.Groups.Where(z => !currentDataSet.Groups.Contains(z)).ToArray())
                                    currentDataSet.Groups.Add(g);
                                    if (currentDataSet.Data.Any(z => z.Name == de.Name))
                                        currentDataSet.Data.Remove(currentDataSet.Data.First(b => b.Name == de.Name));
                                    currentDataSet.Data.Add(de);
                                }
                            }
                            catch
                            {
                                pdImport.Hide();
                                new ConfirmDialog("Data Import Failed", "XML Error", true).ShowAsync();
                            }
                            pdImport.Closed += (x, y) => new ConfirmDialog("Data Import Successfull", "", true).ShowAsync();
                            pdImport.Hide();
                            UpdateGroupCheckBoxes();
                        };
                        pdImport.ShowAsync();
                        break;
                    case "Export":
                        new PromptDialog("Export (Copy)", string.Join("\n", "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"+ currentDataSet.GetXElement().ToString())) { MultiLine = true,PrimaryButtonText="",SecondaryButtonText="Close" }.ShowAsync();
                        break;
                }
            }
        }


        private void CurrentDataSetGroupFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DataElementsSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void dataPage_Loaded(object sender, RoutedEventArgs e)
        {
            NoDataElementSelectedGrid.Visibility = Visibility.Visible;
            NoDataSetSelectedGrid.Visibility = Visibility = Visibility.Visible;
            new MessageDialog("add like a default value or something to dataelements").ShowAsync();
        }

        private void DataElementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var f = sender as MenuFlyoutItem;
            var dE=f.DataContext as DataElement;
            switch (f.Tag)
            {
                case "Rename":
                    
                    var p = new PromptDialog("Rename Data Element", dE.Name);
                    p.Validation = (txt) => new Tuple<bool, string>(!currentDataSet.Data.Any(z => z.Name == txt), "Name is already taken in data set.");
                    p.ConfirmedEvent += (a, txt) => {
                        dE.Name = txt;
                        if (DataElementsView.SelectedItem == dE)
                            DataElementNameBox.Text = txt;
                    };
                    p.ShowAsync();
                break;
                case "Delete":
                    
                    var cd = new ConfirmDialog("Delete Data Element", "Are you sure you want to delete " + dE.Name + "?");
                    cd.ConfirmEvent += (a, delete) =>
                    {
                        if (delete)
                            currentDataSet.Data.Remove(dE);
                    };
                    cd.ShowAsync();
                break;
            }
        }

     
    }
}
