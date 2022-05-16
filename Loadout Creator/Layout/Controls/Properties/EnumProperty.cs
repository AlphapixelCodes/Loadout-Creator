using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Loadout_Creator
{
    public class EnumProperty<T> : AProperty
    {

        public override PropertyControl GetControl()
        {
            var combo = new ComboBox();
            combo.HorizontalAlignment = HorizontalAlignment.Right;

            combo.ItemsSource = Enum.GetNames(typeof(T));

            if (Value != null)
                combo.SelectedItem = Enum.GetName(typeof(T), Value);
            else
                throw new NotImplementedException("Value is not set");
            combo.SelectionChanged += (a, b) => Value = Enum.Parse(typeof(T), combo.SelectedItem as string);

            return new PropertyControl(this, combo);
        }

        internal override void FromXElement(XElement e,string v)
        {
            if (Enum.TryParse(typeof(T), v, out object z))
                Value = z;
            else
                throw new NotImplementedException(e.ToString());
        }
    }
}
