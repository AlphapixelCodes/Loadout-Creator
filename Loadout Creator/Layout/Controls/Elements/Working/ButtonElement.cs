using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Loadout_Creator
{
    public class ButtonElement :AElement
    {
        public override string ElementType => "Button";
        //public override string[] AcceptsAnyChildType => null;
        public override Dictionary<string, AProperty[]> GetProperties() => Properties;
        public override void SetProperties(Dictionary<string, AProperty[]> p) => Properties = p;
      
        private Dictionary<string, AProperty[]> Properties = new Dictionary<string, AProperty[]>()
        {
            { "Designer",DefaultDesigner()},
            {"Popup",new AProperty[]{
                    
                }
            },
            {"Layout",DefaultLayout(HorizontalAlignment.Left,VerticalAlignment.Center) },
            { "Grid",DefaultGrid()},
             {"Design",
                new AProperty[]{
                   GetFrequentProperty("Background",FrequentPropertyGroup.Design,Control.BackgroundProperty),
                    GetFrequentProperty("Border",FrequentPropertyGroup.Design,Control.BorderBrushProperty),
                    GetFrequentProperty("Border Thickness",FrequentPropertyGroup.Design,Control.BorderThicknessProperty),
                    GetFrequentProperty("Corner Radius",FrequentPropertyGroup.Design),
                }
            },
        };

        public ButtonElement()
        {
            InitLabelChangedEvent();
            OneChildOnly = true;
        }
        public override UIElement BuildElement(LoadoutState state)
        {
            var b=new Button();
            b.Click += (a, z) => state.ShowPopup("name idk");
            ApplyProperties(b);
            //b.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
            if(Children.Count>0)
                b.Content=Children[0].BuildElement(state);
            return b;
        }
        internal override void ApplyUnknownProperty(UIElement e, AProperty prop)
        {
            Button b=e as Button;
            switch (prop.Name)
            {
                case "Corner Radius":
                    ThicknessToCornerRadius(prop, b, Control.CornerRadiusProperty);
                    break;
                default:
                    throw new NotImplementedException(prop.Name);
            }
        }

    }
}
