using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using System.Xml.Linq;

namespace Loadout_Creator
{
 
    public class DataSet : INotifyPropertyChanged
    {
        private string name;
        public string Name { get => name; set {
                name = value;
                RaisePropertyChanged("Name");
            } }
        public int ElementCount => DataElements.Count;
        public int DataColumnCount => Properties.Count;
        public DataSets Parent { get; }

        public ObservableCollection<ADataColumn> Properties = new ObservableCollection<ADataColumn>();
        public ObservableCollection<DataElement> DataElements=new ObservableCollection<DataElement>();


        public ObservableCollection<String> Groups=new ObservableCollection<string>();
        public DataSet(DataSets parent,string name)
        {
            Parent = parent;
            Name = name;
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

        internal static DataSet FromXElement(XElement set, DataSets ds)
        {
            
            var ret = new DataSet(ds, set.Attribute("Name").Value);
            var elms = set.Element("DataElements");
            foreach (var XDataElement in elms.Elements("DataElement"))
            {
                var de=ret.AddDataElement(XDataElement.Attribute("Name").Value);
                foreach (var g in XDataElement.Element("Groups").Elements("Group"))
                {
                    de.Groups.Add(g.Attribute("Name").Value);
                }
            }

            foreach (var g in set.Element("Groups").Elements("Group"))
            {
                ret.Groups.Add(g.Attribute("Name").Value);
            }
            return ret;
        }
        //adds properties because every element needed to be loaded before it was set
        internal void LoadProperties(XElement DataSetXElement, DataSets dss)
        {
            foreach (var XProp in DataSetXElement.Element("Properties").Elements("DataColumn"))
            {
                Properties.Add(ADataColumn.FromXElement(XProp,dss, this));
            }
            var XDataElements = DataSetXElement.Element("DataElements").Elements("DataElement");
            foreach (var dataElement in DataElements)
            {
                var deName = dataElement.Name;
                var x = XDataElements.First(e => e.Attribute("Name").Value == deName);

                dataElement.AddProperties(x, dss);
                 
            }
        }

        internal DataSet Clone(string v)
        {
            throw new NotImplementedException();
        }

        internal DataElement AddDataElement(string datElementName)
        {
            var de = new DataElement(datElementName, this);
            DataElements.Add(de);
            RaisePropertyChanged("ElementCount");
            return de;
        }

        public enum PropertyModificationType
        {
            Add, Rename, Remove, RemoveAll
        }
        public void ModifySchema(PropertyModificationType type,ADataColumn column=null,string oldName=null)
        {
            switch (type)
            {
                case PropertyModificationType.Add:
                    Properties.Add(column);
                    var t = column.GetNonAbstractType();
                    if (column is DataSetReference_DataColumn) {
                        var reference = (column as DataSetReference_DataColumn);
                        reference.CurrentDataSet = this;
                        foreach (var de in DataElements)
                            de.AddDataColumn(column.Name, t, null,reference.ReferencedDataSet);
                    }
                    else
                    {
                        foreach (var de in DataElements)
                            de.AddDataColumn(column.Name, t);
                    }
                    
                    
                    
                    break;
                case PropertyModificationType.Remove:
                    Properties.Remove(column);
                    foreach (var de in DataElements)
                        de.Properties.Remove(de.GetDataColumnByName(column.Name));
                    break;
                case PropertyModificationType.RemoveAll:
                    Properties.Clear();
                    foreach (var de in DataElements)
                        de.Properties.Clear();
                    break;
                case PropertyModificationType.Rename:
                    foreach (var de in DataElements)
                        de.GetDataColumnByName(oldName).Name = column.Name;
                    break;
            }
        }
        public XElement GetXElement()
        {
            var ret = new XElement("DataSet");
            ret.SetAttributeValue("Name", Name);

            var props = new XElement("Properties");
            props.Add(Properties.Select(e => e.GetXElement()));
            ret.Add(props);

            var groups = new XElement("Groups");
            foreach (var item in Groups)
            {
                var x = new XElement("Group");
                x.SetAttributeValue("Name", item);
                groups.Add(x);

            }
            ret.Add(groups);

            var elements = new XElement("DataElements");
            elements.Add(DataElements.Select(e => e.GetXElement()));
            ret.Add(elements);
            return ret;
        }
        

    }
}
