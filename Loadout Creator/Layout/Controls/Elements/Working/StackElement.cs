using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Loadout_Creator
{
    public class StackElement: AElement
    {
        public override string ElementType => "Stack";
        public override void SetProperties(Dictionary<string, AProperty[]> p) => Properties = p;
        private Dictionary<string, AProperty[]> Properties = new Dictionary<string, AProperty[]>()
        {
            {"Designer",DefaultDesigner()},
            {"Layout",DefaultLayout()},

            { "Stack",new AProperty[]
                {
                    new DoubleProperty(){Name="Spacing", DefaultValue=Orientation.Vertical,Dependency=StackPanel.SpacingProperty,isInt=true,CastType=typeof(int)},
                    new EnumProperty<Orientation>(){Name="Orientation",Value=Orientation.Vertical,Dependency=StackPanel.OrientationProperty},
                } 
            },
             { "Grid",DefaultGrid()},
            {"Design",
                new AProperty[]{
                   GetFrequentProperty("Background",FrequentPropertyGroup.Design,Panel.BackgroundProperty),
                    GetFrequentProperty("Border",FrequentPropertyGroup.Design,StackPanel.BorderBrushProperty),
                    GetFrequentProperty("Border Thickness",FrequentPropertyGroup.Design,StackPanel.BorderThicknessProperty),
                    GetFrequentProperty("Corner Radius",FrequentPropertyGroup.Design),
                }
            },
        };

        public override UIElement BuildElement(LoadoutState state)
        {
            var s = new StackPanel();
            ApplyProperties(s);
            foreach (var child in Children)
            {
                s.Children.Add(child.BuildElement(state));
            }
            return s;
        }
        internal override void ApplyUnknownProperty(UIElement e, AProperty prop)
        {
            var s=e as StackPanel;
            switch (prop.Name)
            {
                case "Corner Radius":
                    ThicknessToCornerRadius(prop, s, StackPanel.CornerRadiusProperty);
                    //    var t = (Thickness)property.Value;
                    //  s.SetValue(Grid.CornerRadiusProperty, new CornerRadius(t.Left, t.Right, t.Bottom, t.Top));
                    break;
                default:
                    throw new NotImplementedException(prop.Name);
            }

        }
        public StackElement()
        {
            InitLabelChangedEvent();
        }
        //public override string[] AcceptsAnyChildType => null;

        public override Dictionary<string, AProperty[]> GetProperties() => Properties;

    }
}
