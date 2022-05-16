using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

namespace Loadout_Creator
{
    public class DataElement : INotifyPropertyChanged
    {
        public DataSet DataSet;
        public DataElement GetThis => this;
        private string name;
        public string Name { get => name; 
            set{
                name = value;
                RaisePropertyChanged("Name");
            } }

        public DataElement(string name, DataSet set)
        {
            Groups.CollectionChanged += (a, b) => RaisePropertyChanged("GroupsString");
            Name= name;
            DataSet = set;
            if (set.Properties.Count > 0)
            {
                foreach (var acol in set.Properties)
                {
                    if(acol is DataSetReference_DataColumn)
                    {
                        AddDataColumn(acol.Name, acol.GetNonAbstractType(),null,((DataSetReference_DataColumn)acol).ReferencedDataSet);
                    }
                    else
                    {
                        AddDataColumn(acol.Name, acol.GetNonAbstractType());
                    }
                    
                }
            }
        }
        

        public ObservableCollection<ADataColumn> Properties =new ObservableCollection<ADataColumn>();
        public ObservableCollection<string> Groups = new ObservableCollection<string>();
        public String GroupsString => string.Join(", ", Groups);
        public void AddDataColumn(string name, Type type, object value = null,DataSet ReferencedDataSet=null)
        {
            var adc = (ADataColumn)Activator.CreateInstance(type);
            adc.Name = name;
            adc.Value = value;
            if (ReferencedDataSet!=null)
            {
                var reference= (DataSetReference_DataColumn)adc;
                reference.CurrentDataSet = DataSet;
                reference.ReferencedDataSet = ReferencedDataSet;
            }
            Properties.Add(adc);
        }
        public ADataColumn GetDataColumnByName(string name) => Properties.FirstOrDefault(e => e.Name == name);
        

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public XElement GetXElement()
        {
            var ret = new XElement("DataElement");
            ret.SetAttributeValue("Name", Name);
            var groups = new XElement("Groups");
            foreach (var group in Groups)
            {
                var g = new XElement("Group");
                g.SetAttributeValue("Name", group);
                groups.Add(g);
            }
            ret.Add(groups);
            var props = new XElement("Properties");
            props.Add(Properties.Select(e=>e.GetXElement()));
            ret.Add(props);
            return ret;
        }

        internal void AddProperties(XElement xDataElement, DataSets dss)
        {
            foreach (var xDataColumn in xDataElement.Element("Properties").Elements("DataColumn"))
            {
                Properties.Add(ADataColumn.FromXElement(xDataColumn, dss, DataSet));
            }
            
        }
    }
}
