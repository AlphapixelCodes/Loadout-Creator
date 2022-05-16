using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Loadout_Creator
{
    public class DataSetReference_DataColumn : ADataColumn
    {
        public DataSet ReferencedDataSet;
        public DataSet CurrentDataSet;
        public DataSetReference_DataColumn()
        {
            Value = new List<DataElement>();
        }

        public override object VerifyValue(object value)
        {
            if (value == null)
            {
                if (this.Value == null)
                    value = new List<DataElement>();
                else
                {
                    (Value as List<DataElement>).Clear();
                    value = Value;
                }
            }
            return value;
        }
        
        public override void EditDataColumn(SchemaData currentSchema = null)
        {
            if (currentSchema.Data.Sets.Any(e => e != CurrentDataSet))
            {
                var drd = new DataReferenceDialog(currentSchema, this);
                drd.ShowAsync();
            }
            else
            {
                new ConfirmDialog("No Data set to reference.","There are no other data sets to reference.",true).ShowAsync();
            }
        }

        public override string getTypeName() => "Data Set Reference";
        internal override Type GetNonAbstractType() => typeof(DataSetReference_DataColumn);

        public override string valueDisplayString()
        {
            if (Value == null)
            {
                return "Unset";
            }
            return ReferencedDataSet.Name +": " +(Value as List<DataElement>).Count();
        }
        public override XElement GetXElement()
        {
            var e=base.GetXElement();
            e.SetAttributeValue("RefrerencedSet", ReferencedDataSet.Name);
            if (Value != null)
            {
                foreach (var item in Value as List<DataElement>)
                {
                    var x = new XElement("Reference");
                    x.SetAttributeValue("ElementName",item.Name);
                    e.Add(x);
                }
            }
            return e;
        }

        public override void ApplyXElementValue(XElement XProperty)
        {
            var names= XProperty.Elements("Reference").Select(z => z.Attribute("ElementName").Value);
            if(names.Count()>0)
                Value = ReferencedDataSet.DataElements.Where(z => names.Contains(z.Name)).ToList();
        }
    }
}
