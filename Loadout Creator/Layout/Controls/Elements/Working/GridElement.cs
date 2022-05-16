using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Loadout_Creator
{
    
    public class GridElement : AElement
    {
        public override string ElementType => "Grid";
        private Dictionary<string, AProperty[]> Properties = new Dictionary<string, AProperty[]>()
        {
            {"Designer", DefaultDesigner()},
            {"Layout",DefaultLayout()},
            { "Grid",new AProperty[]
                {
                    GetFrequentProperty("Row",FrequentPropertyGroup.Grid),
                    GetFrequentProperty("Column",FrequentPropertyGroup.Grid),
                    GetFrequentProperty("Row Span",FrequentPropertyGroup.Grid),
                    GetFrequentProperty("Column Span",FrequentPropertyGroup.Grid),


                    new DoubleProperty(){Name="Row Spacing",Dependency=Grid.RowSpacingProperty},
                    new DoubleProperty(){Name="Column Spacing",Dependency=Grid.ColumnSpacingProperty},

                    new RowColumnProperty(){Name="Rows",isRow=true,Value=new List<RowColData>()},
                    new RowColumnProperty(){Name="Columns",isRow=false,Value=new List<RowColData>()},
                }
            },
            {"Design",
               new AProperty[]{
                  GetFrequentProperty("Background",FrequentPropertyGroup.Design,Grid.BackgroundProperty),
                    GetFrequentProperty("Border",FrequentPropertyGroup.Design,Grid.BorderBrushProperty),
                    GetFrequentProperty("Border Thickness",FrequentPropertyGroup.Design,Grid.BorderThicknessProperty),
                    GetFrequentProperty("Corner Radius",FrequentPropertyGroup.Design),
                }
            },
          
        };
        public override void SetProperties(Dictionary<string, AProperty[]> p) => Properties = p;
        public GridElement()
        {
            InitLabelChangedEvent();
        }


        public override UIElement BuildElement(LoadoutState state)
        {
            var g = new Grid();
            ApplyProperties(g);
            foreach (var child in Children)
            {
                g.Children.Add(child.BuildElement(state));
            }
            return g;
        }
        internal override void ApplyUnknownProperty(UIElement e,AProperty property)
        {
            var g=e as Grid;
            switch (property.Name)
            {
                case "Rows":
                    foreach (var r in property.Value as List<RowColData>)
                    {

                        var rd = new RowDefinition();
                        rd.SetValue(RowDefinition.HeightProperty, r.Value);
                        if (r.Min > 0)
                            rd.SetValue(RowDefinition.MinHeightProperty, r.Min);


                        if (r.Max > 0)
                            rd.SetValue(RowDefinition.MaxHeightProperty, r.Max);

                        g.RowDefinitions.Add(rd);
                    }
                    break;
                case "Columns":

                    foreach (var c in property.Value as List<RowColData>)
                    {
                        var cd = new ColumnDefinition();
                        cd.SetValue(ColumnDefinition.WidthProperty, c.Value);
                        if (c.Min > 0)
                            cd.SetValue(RowDefinition.MinHeightProperty, c.Min);
                        if (c.Max > 0)
                            cd.SetValue(RowDefinition.MaxHeightProperty, c.Max);
                        g.ColumnDefinitions.Add(cd);
                    }
                    break;
                case "Corner Radius":
                    ThicknessToCornerRadius(property, g, Grid.CornerRadiusProperty);
                    break;
                default:
                    throw new NotImplementedException("whoopsie: " + property.Name);
            }
        } 
        public override Dictionary<string, AProperty[]> GetProperties() {
            if (Name == "Root")
            {
                Properties.Remove("Designer");
            }
            return Properties;
        }
    }      
}
