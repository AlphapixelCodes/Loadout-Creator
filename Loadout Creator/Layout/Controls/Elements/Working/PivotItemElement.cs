using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Loadout_Creator
{
    public class PivotItemElement:AElement
    {
        public override string ElementType => "PivotItem";
        public override void SetProperties(Dictionary<string, AProperty[]> p) => Properties = p;
        private Dictionary<string, AProperty[]> Properties = new Dictionary<string, AProperty[]>()
        {
             {"Designer", DefaultDesigner()},

             {"Text",new AProperty[]{
                    new TextProperty(){Name="make this fucker the god damn thingamajig",Value="",DefaultValue=""},
                    new TextProperty(){Name="HeaderTemp",Value="",DefaultValue=""},
                }
            },
             {"Layout",DefaultLayout()}
        };

        public PivotItemElement()
        {
            OneChildOnly = true;
            InitLabelChangedEvent();
        }
        public override UIElement BuildElement(LoadoutState state)
        {
            var p = new PivotItem();
            ApplyProperties(p);
            if(Children.Count>0)
                p.Content=Children[0].BuildElement(state);
            return p;
        }
        internal override void ApplyUnknownProperty(UIElement e, AProperty prop)
        {
            var p=e as PivotItem;
            switch (prop.Name)
            {
                case "HeaderTemp":
                    p.Header = prop.Value;
                    break;
                default:
                    break;
            }
        }
        public override Dictionary<string, AProperty[]> GetProperties() => Properties;
    }
}
