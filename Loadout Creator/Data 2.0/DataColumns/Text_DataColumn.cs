using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Loadout_Creator
{
    public class Text_DataColumn : ADataColumn
    {
        public override void EditDataColumn(SchemaData currentSchema = null)
        {
            PromptDialog p;
            if(Value!=null)
                p=new PromptDialog("Enter New Text", Value.ToString()) { canBeEmpty=true};
            else
                p = new PromptDialog("Enter Text") { canBeEmpty=true};
            p.ConfirmedEvent += (a, b) => Value = b;
            p.ShowAsync();
        }

        public override string getTypeName() => "Text";
        internal override Type GetNonAbstractType() => typeof(Text_DataColumn);
        public override string valueDisplayString() => Value == null ? "" : Value as string;

        public override void ApplyXElementValue(XElement x)
        {
            var val = x.Attribute("Value");
            if (val != null && val.Value != "Null")
                Value = val.Value;
        }
    }
}
