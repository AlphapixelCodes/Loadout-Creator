using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Loadout_Creator
{
    public static class StaticData
    {
        public static ObservableCollection<DataSet> DataSets = new ObservableCollection<DataSet>();
        public static DataSet AddDataSet(string name)
        {
            var ret = new DataSet(name);
            DataSets.Add(ret);
            return ret;
        }

        internal static XElement GetXElement()
        {
            var ret=new XElement("DataSets");
            foreach (var set in DataSets)
            {
                ret.Add(set.GetXElement());
            }

            return ret;
        }
    }
}
