using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Loadout_Creator
{
    public class LabelElement:AElement
    {
        public override string ElementType => "Label";
        private Dictionary<string, AProperty[]> Properties = new Dictionary<string, AProperty[]>()
        {
            {"Designer",new AProperty[]{
                    new TextProperty(){Name="Label",Value=""}
                }
            },
             {"Design",
                new AProperty[]{
                    new TextProperty(){Name="Text",Value=string.Empty},
                    new ColorProperty(){Name="Foreground"},
                   new ColorProperty(){Name="Background"},
                   new ColorProperty(){Name="Border"},
                   new ThicknessProperty(){Name ="Border Thickness",Value=new Thickness()},
                   new EnumProperty<HorizontalAlignment>(){Name="Text Horizontal Alignment",Value=HorizontalAlignment.Stretch},
                    new EnumProperty<VerticalAlignment>(){Name="Text Vertical Alignment",Value=VerticalAlignment.Stretch},
                }
            },
            {"Layout",
                new AProperty[]{
                    new DoubleProperty(){Name="Width" },
                    new DoubleProperty(){Name="MaxWidth",TabSize=20},
                    new DoubleProperty(){Name="MinWidth",TabSize=20},
                    new DoubleProperty(){Name="Height"},
                    new DoubleProperty(){Name="MaxHeight",TabSize=20},
                    new DoubleProperty(){Name="MinHeight",TabSize=20},
                    new DoubleProperty(){Name="Row"},
                    new DoubleProperty(){Name="Column"},
                    new EnumProperty<HorizontalAlignment>(){Name="Horizontal Alignment",Value=HorizontalAlignment.Stretch},
                    new EnumProperty<VerticalAlignment>(){Name="Vertical Alignment",Value=VerticalAlignment.Stretch},
                    new ThicknessProperty(){Name="Margin",Value=new Thickness()},
                }
            },
           
        };

        public LabelElement()
        {
            GetProperty("Label").PropertyChanged += (a, b) => {
                LabelName = b as string;
                RaisePropertyChanged("DisplayName");
            };
        }
        public override bool AcceptsAnyChildType => false;
        public override Dictionary<string, AProperty[]> GetProperties() => Properties;

     
    }
}
