using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Loadout_Creator
{
    public class ElementTree
    {
        public SchemaData SchemaData { get; set; }
        public  GridElement RootGridElement = new GridElement() { Name = "Main Grid", canDrag = false, canDelete = false };
        public  PopupContainerElement PopupContainer = new PopupContainerElement();

        public ElementTree(SchemaData schema)
        {
            SchemaData = schema;
        }

        public List<String> GetAllBindableElementNames()
        {
            List<string> ret=new List<string>();
            foreach (var item in PopupContainer.Children)
            {
                ret.AddRange(Recurse(item));
            }
            ret.AddRange(Recurse(RootGridElement));
            return ret;
        }
        private List<String> Recurse(AElement element)
        {
            List<string> ret = new List<string>();
            if (element.isSelector() && element.Name != null && element.Name.Length > 0)
                ret.Add(element.Name);
            foreach (var item in element.Children)
            {
                ret.AddRange(Recurse(item));
            }
            return ret;
        }

        public XElement GetXElement()
        {
            var ret = new XElement("ElementTree");
            ret.Add(new XElement("RootGrid",RootGridElement.GetXElement()));
            ret.Add(new XElement("RootPopup", PopupContainer.GetXElement()));
            return ret;
        }

        internal void Dispose()
        {
            RootGridElement.DeleteElement();
            PopupContainer.DeleteElement();
            GC.Collect();
        }

        public static ElementTree LoadXElement(XElement xElement,SchemaData schema)
        {
            var ret=new ElementTree(schema);
            var r = xElement.Element("RootGrid");
            if (r!=null && r.Element("Grid")!=null)
               ret.RootGridElement= (GridElement)AElement.FromXElement(r.Element("Grid"));

            var p = xElement.Element("RootPopup");
            if (p != null && p.Element("Popups") != null)
                ret.PopupContainer = (PopupContainerElement)AElement.FromXElement(p.Element("Popups"));

            return ret;
        }
    }
}
