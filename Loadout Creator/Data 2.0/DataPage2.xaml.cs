
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Xml.Linq;
using Windows.Storage;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Loadout_Creator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataPage2 : Page
    {
        private DataSetEditorPage DataSetEditor;
        public DataSets CurrentDataSets => Schema.Data;
        private DataSet CurrentDataSet => DataSetsDataGrid.SelectedItem as DataSet;
        private DataElement CurrentDataElement=>DataElementsListView.SelectedItem as DataElement;
        private SchemaData Schema;
        private bool areGroupChangesWanted = false;

        public DataPage2(SchemaData schemaData)
        {
            Schema = schemaData;
            InitializeComponent();
            
            
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            new MessageDialog("finish query").ShowAsync();
            DataSetsDataGrid.Columns.Add(new DataGridTextColumn() {Header="Name",Binding=new Binding() {Path=new PropertyPath("Name") } });
            DataSetsDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Element Count", Binding = new Binding() { Path = new PropertyPath("ElementCount") } });
            LoadSchema(Schema);

            DataSetEditor = new DataSetEditorPage();
          //  DataSetEditor.DataColumnsChanged += UpdateDataElementColumns;
            DataSetEditorGridContainer.Content = DataSetEditor;

            DataElementTableMoreButton.IsEnabled = false;
            DataElementTableAddButton.IsEnabled = false;

            DataElementPropertiesGrid.Columns.Add(new DataGridTextColumn() {Header="Name",Binding=new Binding() {Path=new PropertyPath("Name") } });
            DataElementPropertiesGrid.Columns.Add(new DataGridTextColumn() { Header = "Type", Binding = new Binding() { Path = new PropertyPath("GetTypeName") } });
            DataElementPropertiesGrid.Columns.Add(new DataGridTextColumn() { Header = "Value", Binding = new Binding() { Path = new PropertyPath("ValueDisplayString") } });

            
        }

   
        public void LoadSchema(SchemaData schema)
        {
            this.Schema = schema;
            DataSetsDataGrid.ItemsSource = CurrentDataSets.Sets;
        }

      
        private Tuple<bool,string> ValidatePromptNameDataSet(string text)
        {
            if (text.Any(z => Char.IsLetterOrDigit(z)))
                return new Tuple<bool, string>(CurrentDataSets.GetDataSet(text) == null, "A Data Set Already Exists By That Name");
            else
                return new Tuple<bool, string>(false, "Name Must Contain Atleast One Letter Or Digit.");
        }
        private async void DataSetsMore_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button)//add button
            {
                var p = new PromptDialog("Data Set Name");
                p.Validation = new PromptDialog.ValidationDelegate(ValidatePromptNameDataSet);
                p.ConfirmedEvent += (a, b) => DataSetsDataGrid.SelectedItem = CurrentDataSets.AddDataSet(b);
                p.ShowAsync();
                return;
            }
            switch((sender as MenuFlyoutItem).Text)
            {
                case "Import Data":
                    if (CurrentDataSets.Sets.Count > 0)
                    {
                        var dc=new ConfirmDialog("Delete Current Data Sets", "By Importing Data, all of your current Data Sets will be deleted, do you wish to continue?");
                        await dc.ShowAsync();
                        if (dc.Result == false)
                            return;
                    }
                    var ofp = new Windows.Storage.Pickers.FileOpenPicker();
                    ofp.FileTypeFilter.Add(".xml");
                    var of = await ofp.PickSingleFileAsync();
                    if (of != null)
                    {
                        var txt = await FileIO.ReadTextAsync(of);
                        try
                        {
                            var XDataSets = XElement.Parse(txt);
                            var dataSets = DataSets.FromXElement(XDataSets, Schema);
                            Schema.Data = dataSets;

                            LoadSchema(Schema);
                        }
                        catch (Exception z)
                        {
                            new ConfirmDialog("Unable to Import.", "An error occured during Import.", true).ShowAsync();
                        }   
                    }
                    break;
                case "Export Data":
                    var fsp=new Windows.Storage.Pickers.FileSavePicker();
                    fsp.FileTypeChoices.Add("XML File",new List<string>() {".xml" });
                    fsp.SuggestedFileName = "Data.xml";
                    var f=await fsp.PickSaveFileAsync();
                    if (f != null)
                    {
                        FileIO.WriteTextAsync(f, Schema.Data.GetXElement().ToString());
                    }
                    
                    
                    break;
                case "Clear":
                    var c = new ConfirmDialog("Clear Data Sets", "Are you sure you want to remove all Data sets?");
                    DataSetEditor.UnloadDataSet();
                    c.ConfirmEvent += (a, b) => CurrentDataSets.Sets.Clear();
                    c.ShowAsync();
                    break;
            }
        }

        private void DataSetListViewFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var mfi = sender as MenuFlyoutItem;
            var dataset= mfi.DataContext as DataSet;
            switch (mfi.Text)
            {
                case "Rename":
                    Debug.WriteLine(mfi.DataContext);
                    var p = new PromptDialog("Rename Data Set", dataset.Name) { Validation = new PromptDialog.ValidationDelegate(ValidatePromptNameDataSet) };
                    p.ConfirmedEvent+=(a,b)=>dataset.Name = b;
                    p.ShowAsync();
                    break;
                case "Duplicate":
                    var i = 0;
                    while (CurrentDataSets.GetDataSet(dataset.Name + " Copy " + i) != null)
                        i++;
                    CurrentDataSets.Sets.Add(dataset.Clone(dataset.Name + " Copy " + i));
                    break;
                case "Delete":
                    throw new NotImplementedException("idk remove groups or something in dataelements");
                    var d = new ConfirmDialog("Clear Datasets", "Are you sure you want to remove all Data sets?");
                    d.ConfirmEvent += (a, b) => CurrentDataSets.Sets.Remove(dataset);
                    d.ShowAsync();
                    break;
            }
        }

        private void DataSetsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataSetsDataGrid.SelectedItem!=null)
            {
                GroupFilterCombo.ItemsSource = CurrentDataSet.Groups;
                GroupFilterCombo.SelectedIndex = -1;

                DataSetEditor.LoadDataSet(CurrentDataSet);

                DataElementUserControl.UpdateHeaders(DataElementsHeaderGrid,CurrentDataSet);
                
             
                DataElementsListView.ItemsSource = CurrentDataSet.DataElements;

                DataElementTableMoreButton.IsEnabled = true;
                DataElementTableAddButton.IsEnabled = true;
                foreach (var item in CurrentDataSets.Sets)
                {
                    CurrentDataSet.DataElements.CollectionChanged -= DataElements_CollectionChanged;
                }
                
                CurrentDataSet.DataElements.CollectionChanged += DataElements_CollectionChanged;
            }
            else
            {
                DataElementsHeaderGrid.Children.Clear();

                GroupFilterCombo.ItemsSource = null;

                GroupListView.ItemsSource = null;
                DataSetEditor.UnloadDataSet();
                DataElementTableMoreButton.IsEnabled = false;
                DataElementTableAddButton.IsEnabled = false;
            //    DataElementsDataGrid.ItemsSource = null;
                DataElementsListView.ItemsSource = null;
            }
            GroupEditButton.IsEnabled = false;
            GroupAddButton.IsEnabled = false;
            areGroupChangesWanted = false;
            GroupListView.ItemsSource = null;
        }

        

        private Tuple<bool,string> DataElementNameValidation(string text)
        {
            if (text.Any(z => Char.IsLetterOrDigit(z)))
                return new Tuple<bool, string>(!CurrentDataSet.DataElements.Any(y => y.Name == text), "A Data Element Already Exists By That Name In This Set.");
            else
                return new Tuple<bool, string>(false, "Name Must Contain Atleast One Letter Or Digit.");
        }
        private void DataElements_Click(object sender, RoutedEventArgs e)
        {
            if (DataSetsDataGrid.SelectedIndex >= 0)
            {
                if (sender is Button)//ADD button
                {
                    var p = new PromptDialog("Data Element Name");
                    p.Validation = new PromptDialog.ValidationDelegate(DataElementNameValidation);
                    p.ConfirmedEvent += (a, b) => DataElementsListView.SelectedItem=CurrentDataSet.AddDataElement(b);
                    p.ShowAsync();
                }
                else
                {
                    switch (((MenuFlyoutItem)sender).Text)
                    {
                        case "Clear":
                            var c = new ConfirmDialog("Clear Data Elements", "Are you sure you want to remove all Data Elements in the Set?");
                            c.ConfirmEvent += (a, b) => CurrentDataSet.DataElements.Clear();
                            c.ShowAsync();
                            break;
                        case "Import Data":
                            throw new NotImplementedException();
                            break;
                        case "Export Data":
                            throw new NotImplementedException();
                            break;
                    }
                }
            }
        }

        private void DataElementsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var S= sender as MenuFlyoutItem;
            var data=S.DataContext as DataElement;
            switch (S.Text)
            {
                case "Rename":
                    var pd=new PromptDialog("Rename Element",data.Name);
                    pd.Validation = new PromptDialog.ValidationDelegate(DataElementNameValidation);
                    pd.ConfirmedEvent += (a, text) =>
                    {
                        data.Name=text;
                        SelectedDataElementName.Text = text;
                    };
                    pd.ShowAsync();
                    break;
                case "Delete":
                    var cd = new ConfirmDialog("Delete Element", $"Are you sure you want to delete Data Element \"{data.Name}\"?");
                    cd.ConfirmEvent += (a, b) => CurrentDataSet.DataElements.Remove(data);
                    cd.ShowAsync();
                    break;
            }
            Debug.WriteLine(S.DataContext);
        }

        private void DataElementsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataElementsListView.SelectedItem != null)
            {
                
                var Element = CurrentDataElement;

                var currentPropSelection = DataElementPropertiesGrid.SelectedIndex;
                DataElementPropertiesGrid.ItemsSource = Element.Properties;
                DataElementPropertiesGrid.SelectedIndex = currentPropSelection==-1? Math.Min(Element.Properties.Count,0):currentPropSelection;
                /*if(Element.Properties.Count>0)
                    DataElementPropertiesGrid.SelectedItem = Element.Properties[0];*/
                SelectedDataElementName.Text = Element.Name;
                
                GroupListView.ItemsSource = CurrentDataSet.Groups;
                GroupEditButton.IsEnabled = true;
                GroupAddButton.IsEnabled = true;
                
                UpdateDataElementGroupsSelected();
               
                GroupListView.IsEnabled = true;


            }
            else
            {
                areGroupChangesWanted = false;
                GroupListView.SelectedItems.Clear();
                GroupListView.IsEnabled = false;
                GroupEditButton.IsEnabled = false;
                GroupAddButton.IsEnabled = false;
                DataElementPropertiesGrid.ItemsSource = null;
                   SelectedDataElementName.Text = "None";
            }
            
        }
        private void UpdateDataElementGroupsSelected()
        {
            areGroupChangesWanted = false;
            GroupListView.SelectedItems.Clear();
            foreach (var group in CurrentDataElement.Groups.ToArray())
            {
                GroupListView.SelectedItems.Add(group);
            }
            areGroupChangesWanted = true;
        }


        private void PropertyButton_Click(object sender, RoutedEventArgs e)
        {

            var dataColumn = (DataElementPropertiesGrid.SelectedItem as ADataColumn);
            switch ((sender as Button).Content as string)
            {
                case "Edit Selected":
                    dataColumn.EditDataColumn(Schema);
                    break;
                case "Clear Value":
                    dataColumn.Value = null;
                    break ;
            }
        }

        private void DataElementPropertiesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataElementPropertiesGrid.SelectedItem == null)
            {
                EditSelectedPropertyButton.IsEnabled=false;
                ClearSelectedPropertyButton.IsEnabled = false;
                
            }
            else
            {
                EditSelectedPropertyButton.IsEnabled=true;
                ClearSelectedPropertyButton.IsEnabled=true;
            }
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var pd=new PromptDialog("Group Name");
            pd.Validation = (text) =>
            {
                if (!text.Any(z => char.IsLetterOrDigit(z)))
                    return new Tuple<bool, string>(false, "Group name must include atleast one letter or digit.");
                return new Tuple<bool, string>(!CurrentDataSet.Groups.Contains(text), "Group already exists by that name.");
            };
            pd.ConfirmedEvent += (a, text) => CurrentDataSet.Groups.Add(text);
            pd.ShowAsync();
            
        }

        private void GroupListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (areGroupChangesWanted)
            {
                Debug.WriteLine("GroupListView_SelectionChanged");
                //var de = DataElementsListView.SelectedItem as DataElement;
                foreach (string add in e.AddedItems)
                {
                    CurrentDataElement.Groups.Add(add);
                }
                foreach (string removed in e.RemovedItems)
                {
                    CurrentDataElement.Groups.Remove(removed);
                }
               
            }
        }

        private async void GroupMoreFlyout_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentDataSet.Groups.Count > 0)
            {
                switch ((sender as MenuFlyoutItem).Text)
                {
                    case "Bulk Select":
                        await new BulkGroupDialog(CurrentDataSet).ShowAsync();
                        UpdateDataElementGroupsSelected();
                        break;
                    case "Edit Groups":
                        await new EditGroupsDialog(CurrentDataSet).ShowAsync();
                        UpdateDataElementGroupsSelected();
                        break;
                }
            }
            else
            {
                new ConfirmDialog("No Groups", "Must have at least 1 group in the Data Set", true).ShowAsync();
            }
        }

            private void GroupFilterCombo_SelectionChanged(object sender=null, SelectionChangedEventArgs e=null)
        {
            if (CurrentDataSet != null)
            {
                if (GroupFilterCombo.SelectedItem == null)
                {
                    try
                    {
                        DataElementsListView.Items.Clear();
                        DataElementsListView.ItemsSource = CurrentDataSet.DataElements;
                    }
                    catch
                    {

                    }
                }
                else
                {
                    DataElementsListView.ItemsSource = null;
                    DataElementsListView.Items.Clear();
                    var group = GroupFilterCombo.SelectedItem as string;
                    foreach (var item in CurrentDataSet.DataElements.Where(z => z.Groups.Contains(group)))
                    {
                        DataElementsListView.Items.Add(item);
                    }

                }
            }
            else
            {
                try
                {
                    DataElementsListView.Items.Clear();
                    DataElementsListView.ItemsSource = null;
                }
                catch
                {

                }
            }
        }
        
//this is here for groupfilter
        private void DataElements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => GroupFilterCombo_SelectionChanged();

        private void ClearGroupFilterButton_Click(object sender, RoutedEventArgs e)
        {
            GroupFilterCombo.SelectedIndex = -1;
        }
    }
}
