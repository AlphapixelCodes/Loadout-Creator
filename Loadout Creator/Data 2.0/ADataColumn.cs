
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Loadout_Creator
{
    public abstract class ADataColumn : INotifyPropertyChanged
    {
        public static Dictionary<string, Type> ColumnTypes = new Dictionary<string, Type>()
        {
            {"Text",typeof(Text_DataColumn) },
            {"Image",typeof(Image_DataColumn) },
            {"Data Set Reference",typeof(DataSetReference_DataColumn) },
        };


        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }


        private object value;
        public object Value
        {
            get => value;
            set
            {
                this.value = VerifyValue(value);
                RaisePropertyChanged("ValueDisplayString");
            }
        }
        public virtual object VerifyValue(object value) => value;
     
        internal abstract Type GetNonAbstractType();
        public abstract string getTypeName();
        public string GetTypeName => getTypeName();

        

        public abstract void EditDataColumn(SchemaData currentSchema = null);
        public abstract string valueDisplayString();
        public string ValueDisplayString => valueDisplayString();

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<ADataColumn> DisposedEvent;
        public void Dispose() => DisposedEvent?.Invoke(this,null);

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual XElement GetXElement()
        {
            var ret = new XElement("DataColumn");
            ret.SetAttributeValue("Name", name);
            ret.SetAttributeValue("Type", GetTypeName);
            if(value != null)
                ret.SetAttributeValue("Value", value);
            return ret;
        }
        public abstract void ApplyXElementValue(XElement x);
        public static ADataColumn FromXElement(XElement XDataColumn,DataSets dss,DataSet ds)
        {
            ADataColumn ret;
            switch (XDataColumn.Attribute("Type").Value)
            {
                case "Data Set Reference":
                    ret = new DataSetReference_DataColumn
                    {
                        CurrentDataSet = ds,
                        ReferencedDataSet = dss.GetDataSet(XDataColumn.Attribute("RefrerencedSet").Value)
                    };
                    break;
                case "Image":
                    ret = new Image_DataColumn();
                    break;
                case "Text":
                    ret=new Text_DataColumn();
                    break;
                default:
                    throw new NotImplementedException();
            }
           
            ret.ApplyXElementValue(XDataColumn);
            ret.Name = XDataColumn.Attribute("Name").Value;
            return ret;
        }
    }
}
