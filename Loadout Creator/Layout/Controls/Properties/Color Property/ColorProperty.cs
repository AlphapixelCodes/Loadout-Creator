using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Loadout_Creator
{
    public class ColorProperty : AProperty
    {
        public override bool isDefault()
        {
            if(DefaultValue!=null)
                return base.isDefault();
            return Value == null;
        }
        public override PropertyControl GetControl()
        {
            return new PropertyControl(this,new ColorControl(this) { HorizontalAlignment=Windows.UI.Xaml.HorizontalAlignment.Right});
        }
        public override void AddXAttribute(XElement x)
        {
            if (!isDefault())
            {
                var c=(Value as SolidColorBrush).Color;
                x.SetAttributeValue(XMLName, $"{c.A},{c.R},{c.G},{c.B}");
            }
        }

        internal override void FromXElement(XElement e, string v)
        {
            var s=v.Split(",");
            Value = new SolidColorBrush(Color.FromArgb(byte.Parse(s[0]), byte.Parse(s[1]), byte.Parse(s[2]), byte.Parse(s[3])));
        }
    }
}
