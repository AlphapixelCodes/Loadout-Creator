using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Loadout_Creator
{
    public class PopupContainerElement:AElement
    {
        public override string ElementType => "Popups";
        public override Type GetUniqueChild => typeof(PopupElement);

        public override bool AcceptsAnyChildType => false;
        public override UIElement BuildElement(LoadoutState state)
        {
            foreach (var item in Children)
            {
                item.BuildElement(state);
            }
            return null;
        }
        public override Dictionary<string, AProperty[]> GetProperties() => new Dictionary<string, AProperty[]>();
        public PopupContainerElement()
        {
            canDelete = false;
            canDrag = false;
        }
    }
}
