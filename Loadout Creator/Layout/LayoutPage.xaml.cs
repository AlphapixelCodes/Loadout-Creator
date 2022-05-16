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
using System.Collections.Generic;
using ColorCode;
using System.Diagnostics;
using Windows.UI.Input;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;
using System.Runtime.InteropServices;
using System.Xml.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Loadout_Creator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LayoutPage : Page
    {
        private AppWindow openPropertyWindow;
        private ElementTree elementTree;
        private SchemaData SchemaData;
        public LayoutPage(SchemaData schemaData)
        {
            SchemaData = schemaData;
            this.InitializeComponent();
           
        }
        private void LoadElementTree(ElementTree tree)
        {
            if (elementTree != null)
            {
                elementTree.Dispose();
            }
            elementTree = tree;
            TreeController.SetSource(new List<AElement>() { elementTree.PopupContainer, elementTree.RootGridElement });
            PropertiesController.LoadGroups(elementTree.RootGridElement);
        }

       

        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadElementTree(SchemaData.ElementTree);   
            TreeController.ItemInvoked += (A, item) => PropertiesController.LoadGroups(item);   
        }
   

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            nedthemolester.Content = null;
            var state = new LoadoutState(true);
            elementTree.PopupContainer.BuildElement(state);
            nedthemolester.Content=(elementTree.RootGridElement.BuildElement(state));
            ErrorListView.ItemsSource = state.ErrorMessages;

        }

        private async void DetatchProperties_Click(object sender, RoutedEventArgs e)
        {
            if (openPropertyWindow == null)
            {

                ResizeRowGrid.Children.Remove(TreeGrid);
                FullColumnGrid.Children.Add(TreeGrid);
                ResizeRowGrid.Visibility = Visibility.Collapsed;
                ColumnSplitter.SetValue(Grid.ColumnProperty, 1);
                PropTreeColumnDefinition.SetValue(ColumnDefinition.WidthProperty, new GridLength(11, GridUnitType.Pixel));
                FullColumnGridDefinition.SetValue(ColumnDefinition.WidthProperty, new GridLength(400, GridUnitType.Pixel));
                
                var parent = (Grid)PropertiesController.Parent;
                parent.Children.Remove(PropertiesController);
                openPropertyWindow = await AppWindow.TryCreateAsync();
                Grid g = new Grid();
                g.Children.Add(PropertiesController);
                ElementCompositionPreview.SetAppWindowContent(openPropertyWindow, g);
                openPropertyWindow.Closed += (z, q) =>
                {
                    PropTreeColumnDefinition.SetValue(ColumnDefinition.WidthProperty, new GridLength(400, GridUnitType.Auto));
                    FullColumnGridDefinition.SetValue(ColumnDefinition.WidthProperty, new GridLength(0, GridUnitType.Pixel));
                    g.Children.Remove(PropertiesController);
                    ColumnSplitter.SetValue(Grid.ColumnProperty, 2);
                    PropertiesGrid.Children.Add(PropertiesController);
                    FullColumnGrid.Children.Remove(TreeGrid);
                    ResizeRowGrid.Children.Add(TreeGrid);
                    openPropertyWindow = null;

                    ResizeRowGrid.Visibility = Visibility.Visible;

                };
                await openPropertyWindow.TryShowAsync();
            }
        }

        private void Import_Export_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem m=sender as MenuFlyoutItem;
            switch (m.Text)
            {
                case "Import Data":
                    var pdImport = new PromptDialog("Paste Data") { MultiLine = true };
                    pdImport.ConfirmedEvent += (a, text) =>
                    {
                        try
                        {
                            pdImport.Hide();
                            var et=ElementTree.LoadXElement(XElement.Parse(text),SchemaData);
                            LoadElementTree(et);
                            SchemaData.ElementTree = et;
                        }
                        catch
                        {
                            pdImport.Closed += (z, b) => new ConfirmDialog("Layout Import Failed", "XML Error", true).ShowAsync();

                            return;
                        }
                        pdImport.Closed += (z, b) => new ConfirmDialog("Layout Import Successfull", "", true).ShowAsync();
                    };
                    pdImport.ShowAsync();
                break;
                case "Export Data":
                   new PromptDialog("Export (Copy)", 
                        string.Join("\n", "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + elementTree.GetXElement())) 
                    { MultiLine = true, PrimaryButtonText = "", SecondaryButtonText = "Close" }.ShowAsync();
                    
                break;
            }
        }
    }
}
