
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadout_Creator
{
    /// <summary>
    /// Holds all the data for the schema, will be passed into loadout creator and DataPage2 and images
    /// </summary>
    public class SchemaData
    {
        public string Name { get; set; }
        public ElementTree ElementTree { get; set; }
        public DataSets Data { get; set; }
        public ImagesData ImagesData { get; private set; }
        public Queries Queries { get; private set; }
        public SchemaData()
        {
            Data = new DataSets(this);
            ImagesData = new ImagesData(this);
            ElementTree= new ElementTree(this);
            Queries= new Queries(this);
        }
    }
}
