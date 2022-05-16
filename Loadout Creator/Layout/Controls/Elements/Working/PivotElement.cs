using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Loadout_Creator
{
    public class PivotElement:AElement
    {
        public override string ElementType => "Pivot";
        public override void SetProperties(Dictionary<string, AProperty[]> p) => Properties = p;
        private Dictionary<string, AProperty[]> Properties = new Dictionary<string, AProperty[]>()
        {
            {"Designer",DefaultDesigner()},
             {"Layout",DefaultLayout() },            
              { "Grid",DefaultGrid()},
               {"Design", new AProperty[]{
                    GetFrequentProperty("Background",FrequentPropertyGroup.Design,Control.BackgroundProperty)
                }
            },
        };
        public override UIElement BuildElement(LoadoutState state)
        {
            var p = new Pivot();
            foreach (var group in Properties)
            {
                foreach (var property in group.Value)
                {
                    if (property.Value != null
                        && !property.ApplyDependencyProperty(p)
                        && property.Name != "Label"
                        )
                    {
                        switch (property.Name)
                        {
                            case "Corner Radius":
                                ThicknessToCornerRadius(property, p, Pivot.CornerRadiusProperty);
                                
                                break;
                            default:
                                throw new NotImplementedException(property.Name);
                        }
                    }
                }
            }
            foreach (var child in Children)
            {
                p.Items.Add(child.BuildElement(state));
            }
            
            return p;
        }
        public PivotElement()
        {
            InitLabelChangedEvent();
        }
        public override bool AcceptsAnyChildType => false;
        public override Type GetUniqueChild => typeof(PivotItemElement);

        public override Dictionary<string, AProperty[]> GetProperties() => Properties;

    }
}
