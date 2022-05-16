using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class DataSetEditorPage : Page
    {
        DataSet currentDataSet;
        public event EventHandler DataColumnsChanged;
        public DataSetEditorPage()
        {
            this.InitializeComponent();
        }
        public void LoadDataSet(DataSet dataSet)
        {
            currentDataSet = dataSet;
            PropertyDataGrid.ItemsSource = dataSet.Properties;
            AddButton.IsEnabled = true;
            MoreButton.IsEnabled = true;
        }
        public void UnloadDataSet()
        {
            currentDataSet = null;
            PropertyDataGrid.ItemsSource = null;
            AddButton.IsEnabled = false;
            MoreButton.IsEnabled = false;
        }
        private Tuple<bool,string> Validation(string Text)
        {
            if (Text.Any(z => Char.IsLetterOrDigit(z)))
                return new Tuple<bool, string>(!currentDataSet.Properties.Any(g => g.Name == Text), "A Data Column Already Exists By That Name");
            else
                return new Tuple<bool, string>(false, "Name Must Contain Atleast One Letter Or Digit.");
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UnloadDataSet();
            PropertyDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Type", Binding = new Binding() { Path = new PropertyPath("GetTypeName") } });
            PropertyDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Name", Binding = new Binding() { Path = new PropertyPath("Name") } });
            var F = new MenuFlyout();
            foreach (var dataColumnType in ADataColumn.ColumnTypes)
            {
                var mfi = new MenuFlyoutItem() {Text=dataColumnType.Key};
                F.Items.Add(mfi);
                mfi.Click += async (a, b) => {
                    var p=new PromptDialog(dataColumnType.Key + " Name");
                    p.Validation= new PromptDialog.ValidationDelegate(Validation);;
                    
                 /*   p.ConfirmedEvent +=async (l, text) =>
                    {
                        
                        var adatacolumn = (ADataColumn)Activator.CreateInstance(dataColumnType.Value);
                        if (dataColumnType.Value == typeof(DataSetReference_DataColumn))
                        {
                            if (currentDataSet.Parent.Sets.Count > 1) {
                                var setPicker=new DataReferenceSetPicker(currentDataSet,adatacolumn as DataSetReference_DataColumn);
                                setPicker.ConfirmedEvent += (f, g) =>
                                {
                                    adatacolumn.Name = text;
                                    currentDataSet.ModifySchema(DataSet.SchemaModificationType.Add, adatacolumn);
                                    DataColumnsChanged?.Invoke(this, null);
                                };
                               
                                    Thread.Sleep(20);
                               await setPicker.ShowAsync();
                            }
                            else
                            {
                                
                                    Thread.Sleep(20);
                                await new ConfirmDialog("Add a Data Set","There must be more than 1 Data Set to add a Data Set Reference.",true).ShowAsync();
                                return;
                            }
                        }
                        else
                        {
                            adatacolumn.Name = text;
                            currentDataSet.ModifySchema(DataSet.SchemaModificationType.Add, adatacolumn);
                            DataColumnsChanged?.Invoke(this, null);
                        }
                        
                       
                    };*/
                    await p.ShowAsync();
                    if (p.Result != null)
                    {
                        var adatacolumn = (ADataColumn)Activator.CreateInstance(dataColumnType.Value);
                        if (dataColumnType.Value == typeof(DataSetReference_DataColumn))
                        {
                            if (currentDataSet.Parent.Sets.Count > 1)
                            {
                                var setPicker = new DataReferenceSetPicker(currentDataSet, adatacolumn as DataSetReference_DataColumn);
                                setPicker.ConfirmedEvent += (f, g) =>
                                {
                                    adatacolumn.Name = p.Result;
                                    currentDataSet.ModifySchema(DataSet.PropertyModificationType.Add, adatacolumn);
                                    DataColumnsChanged?.Invoke(this, null);
                                };
                                setPicker.ShowAsync();
                            }
                            else                              
                                new ConfirmDialog("Add a Data Set", "There must be more than 1 Data Set to add a Data Set Reference.", true).ShowAsync();
                        }
                        else
                        {
                            adatacolumn.Name = p.Result;
                            currentDataSet.ModifySchema(DataSet.PropertyModificationType.Add, adatacolumn);
                            DataColumnsChanged?.Invoke(this, null);
                        }
                    }
                };
            }
            AddButton.Flyout = F;
        }

        private void ClearProperties_Click(object sender, RoutedEventArgs e)
        {
            var c=new ConfirmDialog("Clear Properties", $"Are you sure you want to clear all of \"{currentDataSet.Name}\" properties?");
            c.ConfirmEvent += (a, b) => currentDataSet.ModifySchema(DataSet.PropertyModificationType.RemoveAll);
            c.ShowAsync();
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var mfi = sender as MenuFlyoutItem;
            var dc = mfi.DataContext as ADataColumn;
            switch (mfi.Text)
            {
                case "Rename":
                    var oldname = dc.Name;
                    var p = new PromptDialog("Rename Data Column",dc.Name);
                    p.Validation = new PromptDialog.ValidationDelegate(Validation);
                    p.ConfirmedEvent += (a, b) => {
                            dc.Name = b;
                            currentDataSet.ModifySchema(DataSet.PropertyModificationType.Rename, dc, oldname);
                        DataColumnsChanged?.Invoke(this,null);
                        };
                    p.ShowAsync();
                    
                    break;
                case "Delete":
                    var c = new ConfirmDialog("Delete Property", $"Are you sure you want to delete the\"{dc.Name}\" property?");
                    c.ConfirmEvent += (a, b) =>
                    {
                        currentDataSet.ModifySchema(DataSet.PropertyModificationType.Remove, dc);
                        DataColumnsChanged?.Invoke(null, null);
                    };
                    c.ShowAsync();
                    break;
            }
        }
    }
}
