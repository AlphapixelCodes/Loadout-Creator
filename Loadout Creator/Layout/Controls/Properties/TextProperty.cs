using System.Xml.Linq;
using Windows.UI.Xaml.Controls;

namespace Loadout_Creator
{
    internal class TextProperty : AProperty
    {
        public TextProperty()
        {
            DefaultValue = "";
        }
        public override PropertyControl GetControl()
        {
            var tb=new TextBox();
            tb.Text=(Value==null)? "":Value.ToString();
            tb.TextChanged+=(a,b)=>Value=tb.Text;
            tb.MaxWidth =250;
            tb.Width = 200;
            tb.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
            return new PropertyControl(this,tb);
        }

        internal override void FromXElement(XElement e, string v) => Value = v;
    }
}