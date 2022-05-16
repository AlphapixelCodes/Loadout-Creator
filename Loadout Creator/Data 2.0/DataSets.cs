using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Loadout_Creator
{
    public class DataSets
    {
        public ObservableCollection<DataSet> Sets = new ObservableCollection<DataSet>();
        public DataSets(SchemaData schemaData)
        {
            SchemaData = schemaData;
        }
        public DataSet AddDataSet(string name)
        {
            var ds = new DataSet(this, name);
            Sets.Add(ds);
            return ds;
        }

        public DataSet GetDataSet(string name) => Sets.FirstOrDefault(x => x.Name.Equals(name));
        public SchemaData SchemaData { get; }

        public XElement GetXElement()
        {
            var Ret = new XElement("DataSets");
            Ret.Add(Sets.Select(e => e.GetXElement()));
            return Ret;
        }
        public static DataSets FromXElement(XElement dataSets, SchemaData schema)
        {
            var dss = new DataSets(schema);

            Dictionary<DataSet, XElement> DatasetX = new Dictionary<DataSet, XElement>();
            
            foreach (var set in dataSets.Elements("DataSet"))
            {
                var ds = DataSet.FromXElement(set, dss);
                dss.Sets.Add(ds);
                DatasetX.Add(ds, set);
            }
            foreach (KeyValuePair<DataSet, XElement> set in DatasetX)
            {
                set.Key.LoadProperties(set.Value,dss);
            //    Debug.WriteLine("Properties Being Set for " + set.Key.Name, "DataSets");
            }

            return dss;

        }
    }
}
