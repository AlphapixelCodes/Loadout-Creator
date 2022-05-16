using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Loadout_Creator
{
    public class PopupElement:AElement
    {
        public override string ElementType => "Popup";

        public Dictionary<string, AProperty[]> Properties = new Dictionary<string, AProperty[]>()
        {
            { "Popup",new AProperty[]{
                new TextProperty(){Name="Name",Value=""},
                new TextProperty(){Name="Title"}
                }
            }
        };
        public override void SetProperties(Dictionary<string, AProperty[]> p) => Properties = p;
        public PopupElement()
        {
            OneChildOnly = true;
            canDrag = false;
            GetProperty("Name").PropertyChanged += (a, b) => Name = b as string;
            
        }
        public override UIElement BuildElement(LoadoutState state)
        {
            state.AddPopup(this);
            return null;
        }

        //public override bool AcceptsAnyChildType => null;

        public override Dictionary<string, AProperty[]> GetProperties() => Properties;

    }
}
