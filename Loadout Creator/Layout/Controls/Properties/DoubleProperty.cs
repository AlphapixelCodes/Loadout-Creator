using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Globalization.NumberFormatting;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Loadout_Creator
{
    public class DoubleProperty : AProperty
    {
        public DoubleProperty()
        {
            DefaultValue = double.NaN;
        }
        internal bool isInt;
        public string PlaceholderText = "Default";

        public override PropertyControl GetControl()
        {
            var b = new NumberBox() {
                Width = 150,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right
            };

            
            if (!Convert.ToDouble(DefaultValue).Equals(double.NaN))
                b.PlaceholderText = PlaceholderText + $" ({DefaultValue})";
            else
                b.PlaceholderText = PlaceholderText;
            if (Value != null)
                b.Value = Convert.ToDouble(Value);
          /*  else if (DefaultValue != null)
                b.Value = Convert.ToDouble(DefaultValue);*/
            
            b.ValueChanged += (a, z) =>
            {
                if (isInt)
                {
                    b.Value = (double)Math.Floor(b.Value);
                }
                Value = b.Value;
            };

            return new PropertyControl(this, b);
        }
        public override bool isDefault()
        {
            if (Value == null || Value.Equals(double.NaN))
                return true;
            if (DefaultValue != null)
            {
                if (isInt)
                    return Convert.ToInt32(Value).Equals(Convert.ToInt32(DefaultValue));
                else
                    return Value.Equals(DefaultValue);
            }
            return false;
        }
        internal override void FromXElement(XElement e, string v)
        {
            if (double.TryParse(v, out double temp))
                Value = temp;
            else
                throw new NotImplementedException(e.ToString());
        }
    }
}
