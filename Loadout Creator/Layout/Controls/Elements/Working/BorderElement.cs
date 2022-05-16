using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Loadout_Creator
{
    public class BorderElement : AElement
    {
        public override string ElementType => "Border";
        public override Dictionary<string, AProperty[]> GetProperties() => Properties;
        public override void SetProperties(Dictionary<string, AProperty[]> p) => Properties = p;
        private Dictionary<string, AProperty[]> Properties = new Dictionary<string, AProperty[]>()
        {
            { "Designer",DefaultDesigner()},
            {"Layout",DefaultLayout() },
             {"Design",
                new AProperty[]{
                   GetFrequentProperty("Background",FrequentPropertyGroup.Design,Border.BackgroundProperty),
                    GetFrequentProperty("Border",FrequentPropertyGroup.Design,Border.BorderBrushProperty),
                    GetFrequentProperty("Border Thickness",FrequentPropertyGroup.Design,Border.BorderThicknessProperty),
                    GetFrequentProperty("Corner Radius",FrequentPropertyGroup.Design),
                }
            },
        };
        public override UIElement BuildElement(LoadoutState state)
        {
            var b = new Border();
            ApplyProperties(b);
            if (Children.Count != 0)
                b.Child = Children[0].BuildElement(state);

            return b;
        }
        internal override void ApplyUnknownProperty(UIElement e, AProperty property)
        {
            var b = e as Border;
            switch (property.Name)
            {
                case "Corner Radius":
                    ThicknessToCornerRadius(property, b, Border.CornerRadiusProperty);
                    break;
                default:
                    throw new NotImplementedException(property.Name);
            }
        }
        public BorderElement()
        {
            OneChildOnly = true;
            InitLabelChangedEvent();
        }
    }
}
