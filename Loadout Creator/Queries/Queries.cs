using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadout_Creator
{
    public class Queries
    {
        public SchemaData Schema { get; set; }
        public ObservableCollection<Query> QueryData=new ObservableCollection<Query>();
        public Queries(SchemaData schema)
        {
            Schema = schema;
        }
    }
}
