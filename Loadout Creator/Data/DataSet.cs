using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml.Media.Imaging;

namespace Loadout_Creator
{
    public class DataSet : INotifyPropertyChanged
    {
        private string name;
        public string Name { 
            get => name;
            set { 
                name = value;
                RaisePropertyChanged("Name");
            } 
        }
        public ObservableCollection<String> Groups { get; set; }=new ObservableCollection<String>();
        public ObservableCollection<DataElement> Data = new ObservableCollection<DataElement>();

        internal object GetXElement()
        {
            var ret = new XElement("DataSet");
            ret.SetAttributeValue("Name", Name);
            var g = new XElement("Groups");
            foreach (var item in Groups)
            {
                var gX = new XElement("Group");
                gX.SetAttributeValue("Name", item);
                g.Add(gX);
            }
            ret.Add(g);
            var d = new XElement("Data");
            foreach (var dataElement in Data)
            {
                d.Add(dataElement.GetXElement());
            }
            ret.Add(d);
            return ret;
        }

        public DataSet(string name)
        {
            Name= name;
        }
        public DataSet(XElement element)
        {
            var n = element.Attribute("Name");
            if (n == null)
                new Exception("DataSet XElement Does not have a name");
            Name = n.Value;
            var g = element.Element("Groups");
            if (g != null)
            {
                foreach (var gX in g.Elements("Group"))
                {
                    var gname = gX.Attribute("Name");
                    if (gname != null)
                        Groups.Add(gname.Value);
                }
            }
            var De = element.Element("Data");
            if (De != null)
            {
                foreach (var xDe in De.Elements("DataElement"))
                {
                    Data.Add(new DataElement(xDe));
                }
            }


        }
        public void RemoveGroup(string name)
        {
            Groups.Remove(name);
            foreach (var dataElement in Data)
            {
                dataElement.RemoveGroup(name);
            }
        }
        public void RenameGroup(string oldname,string newname)
        {
            var hasgroup = Data.Where(d => d.Groups.Contains(oldname)).ToList();
            RemoveGroup(oldname);
            Groups.Add(newname);
            foreach (var dataElement in hasgroup)
            {
                dataElement.AddGroup(newname);
            }
        }

        internal Data_2._0.DataSet Clone(string v)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => Name;

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
    public class DataElement : INotifyPropertyChanged
    {
        private string name;
        public string Name {
            get => name;
            set {
                name= value;
                RaisePropertyChanged("Name");
            } 
        }
        public List<string> Groups { get; set; } = new List<string>();
        public void AddGroup(string groupName)
        {
            if (!Groups.Contains(groupName))
            {
                Groups.Add(groupName);
                RaisePropertyChanged("GroupString");
            }
        }
        public void RemoveGroup(string groupName)
        {
            Groups.Remove(groupName);
            RaisePropertyChanged("GroupString");
        }
        public BitmapImage Image { get; set; }
        public string GroupString => String.Join(", ", Groups);
        
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
            var deX = new XElement("DataElement");
            deX.SetAttributeValue("Name", Name);
            foreach (var g in Groups)
            {
                var gX = new XElement("Group");
                gX.SetAttributeValue("Name", g);
                deX.Add(gX);
            }
            return deX;
        }
        public DataElement(XElement xElement)
        {
            var n= xElement.Attribute("Name").Value;
            if (n != null)
                Name = n;
            else
                throw new Exception("XElement Does not contain name attribute");
            if (xElement.HasElements)
            {
                foreach (var g in xElement.Elements("Group"))
                {
                    var gn = g.Attribute("Name").Value;
                    if (gn != null)
                        Groups.Add(gn);
                    else
                        throw new Exception("XElement[Name=\""+Name+"\"] Does not contain group name attribute");
                }
                RaisePropertyChanged("GroupString");
            }

        }
        public DataElement()
        {

        }
    }
}
