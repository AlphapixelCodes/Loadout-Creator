using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;

namespace Loadout_Creator
{
    public class ThicknessProperty : AProperty
    {
        internal bool ShowIcons=true;
        public bool AllowNegatives = false;
        public override PropertyControl GetControl()
        {
            return new PropertyControl(this, new ThicknessControl(this));
        }
        public ThicknessProperty()
        {
            DefaultValue = new Thickness();
            //Value=new Thickness();
        }
        public override bool isDefault()
        {
            if (Value == null)
                return true;
            if (DefaultValue != null)
            {
                var def = (Thickness)DefaultValue;
                var val = (Thickness)Value;
                return def == val;
            }
            return false;
        }

        internal override void FromXElement(XElement e, string v)
        {
            var s = v.Split(",");
            Value = new Thickness(double.Parse(s[0]), double.Parse(s[1]), double.Parse(s[2]), double.Parse(s[3]));
        }

        internal override AProperty Clone()
        {
            var c=base.Clone() as ThicknessProperty;
            c.ShowIcons=ShowIcons;
            c.AllowNegatives=AllowNegatives;
            return c;
        }
    }
}
