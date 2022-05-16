using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class DataElementUserControl : UserControl
    {
        public DataElementUserControl()
        {
            this.InitializeComponent();
        }
        private void addColumn(object value,bool isbinding)
        {
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            var tb=new TextBlock();
            if (isbinding)
                tb.SetBinding(TextBlock.TextProperty, value as Binding);
            else
                tb.Text = value as string;
            tb.SetValue(Grid.ColumnProperty, MainGrid.ColumnDefinitions.Count - 1);
            MainGrid.Children.Add(tb);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RedoColumns();
            MainGrid.ColumnSpacing = 5;
        }

        private void RedoColumns()
        {
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.Children.Clear();

            DataElement dc = DataContext as DataElement;
            if (dc == null) { 
                MainGrid.Background = new SolidColorBrush(Colors.Red);
                return;
            }
            addColumn(new Binding() { Path = new PropertyPath("Name") }, true);
            foreach (var col in dc.Properties)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                var tb = new TextBlock();
                Grid.SetColumn(tb, MainGrid.Children.Count);
                var val = col.valueDisplayString();
                if (val != null)
                    tb.Text = val;
                tb.TextTrimming = TextTrimming.CharacterEllipsis;
                //col.DisposedEvent += RedoColumns;
                col.PropertyChanged += (a, b) => tb.Text = col.valueDisplayString();
                MainGrid.Children.Add(tb);
            }
            addColumn(new Binding() { Path=new PropertyPath("GroupsString")}, true);
            dc.Properties.CollectionChanged+=(a,b)=>RedoColumns();
            //dc.Groups.CollectionChanged += (a, b) => RedoColumns();
        }
        public static void UpdateHeaders(Grid headerGrid,DataSet dataSet)
        {
            headerGrid.Children.Clear();
            headerGrid.ColumnDefinitions.Clear();
            headerGrid.Children.Add(new TextBlock() { Text = "Name",TextTrimming=TextTrimming.CharacterEllipsis });
            
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < dataSet.Properties.Count; i++)
            {
                var s = dataSet.Properties[i];
                s.PropertyChanged += (a, b) => {
                    if(b.PropertyName=="Name")
                        UpdateHeaders(headerGrid, dataSet);
                };
                var tb = new TextBlock() { Text=s.Name, TextTrimming = TextTrimming.CharacterEllipsis };
                Grid.SetColumn(tb, i+1);
                
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                headerGrid.Children.Add(tb);
            }
            var tbg = new TextBlock() { Text = "Groups", TextTrimming = TextTrimming.CharacterEllipsis };

            headerGrid.Children.Add(tbg);
            Grid.SetColumn(tbg, headerGrid.ColumnDefinitions.Count);
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition());

            dataSet.Properties.CollectionChanged += (a, b) => UpdateHeaders(headerGrid,dataSet);
        }

       
    }
}
