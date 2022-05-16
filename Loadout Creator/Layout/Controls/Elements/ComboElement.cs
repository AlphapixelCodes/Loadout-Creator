using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Loadout_Creator
{
    public class ComboElement:AElement
    {
        public override string ElementType => "ComboBox";
        private Dictionary<string, AProperty[]> Properties = new Dictionary<string, AProperty[]>()
        {
       /*      {"Design",
                new List<AProperty>(){
                    new TextProperty(){Name="Name"},
                    new DataProperty(){Name="Data Source"},
                    new ColorProperty(){Name="Forground"},
                   new ColorProperty(){Name="Background"},
                   new ColorProperty(){Name="Border"},
                   new ThicknessProperty(){Name ="Border Thickness",Value=new Thickness()}
                }
            },
            {"Layout",
                new List<AProperty>(){
                    new DoubleProperty(){Name="Width",Value=-1},
                    new DoubleProperty(){Name="MaxWidth",Value=-1,TabSize=20},
                    new DoubleProperty(){Name="MinWidth",Value=-1,TabSize=20},
                    new DoubleProperty(){Name="Height",Value=-1},
                    new DoubleProperty(){Name="MaxHeight",Value=-1,TabSize=20},
                    new DoubleProperty(){Name="MinHeight",Value=-1,TabSize=20},
                    new DoubleProperty(){Name="Row"},
                    new DoubleProperty(){Name="Column"},
                    new EnumProperty<HorizontalAlignment>(){Name="Horizontal Alignment",Value=HorizontalAlignment.Stretch},
                    new EnumProperty<VerticalAlignment>(){Name="Vertical Alignment",Value=VerticalAlignment.Stretch},
                    new ThicknessProperty(){Name="Margin",Value=new Thickness()},
                    new ThicknessProperty(){Name="Padding",Value=new Thickness()},
                }
            },*/

        };

        public ComboElement()
        {
            GetProperty("Name").PropertyChanged+=(a,b)=> Name = b as string;
        }

        /*public override UIElement BuildElement()
        {
            throw new NotImplementedException();
        }*/
        public override bool AcceptsAnyChildType => false;
        public override bool isSelector() => true;
        public override Dictionary<string, AProperty[]> GetProperties() => Properties;
    }
}
