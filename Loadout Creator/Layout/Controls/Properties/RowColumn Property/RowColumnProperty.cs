using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;

namespace Loadout_Creator
{
    public class RowColumnProperty : AProperty
    {
        public bool isRow;
        public override PropertyControl GetControl()
        {
            return new PropertyControl(this,new RowOrColumnPropertyControl(this) { HorizontalAlignment=HorizontalAlignment.Right});
        }
        public override bool isDefault()
        {
            if (Value == null)
                return true;
            return ((List<RowColData>)Value).Count == 0;
        }
        public override void AddXAttribute(XElement x)
        {
            if (!isDefault())
            {
                var rcdef = new XElement("Grid."+(isRow ? "Row" : "Column") + "Definitions");
                x.Add(rcdef);
                
                foreach (var rcd in (List<RowColData>)Value)
                {
                    rcdef.Add(rcd.GetXElement());
                }
            }
        }

        internal override void FromXElement(XElement e, string v)
        {
            var defs=e.Element("Grid." + (isRow ? "Row" : "Column") + "Definitions").Elements("Definition");
            Value = defs.Select(d => new RowColData(d));
        }

        internal override AProperty Clone()
        {
            var c= base.Clone() as RowColumnProperty;
            c.Value= ((List<RowColData>)Value).Select(d => d.Clone()).ToList();
            c.isRow= isRow;
            return c;
        }
    }
    
    public class RowColData
    {
        public int Max = -1, Min = -1;
        public GridLength Value=new GridLength(1, GridUnitType.Star);

        public RowColData(XElement d)
        {
            var val = d.Attribute("Value").Value;
            if (val.Equals("Auto"))
            {
                Value=new GridLength(0, GridUnitType.Auto);
            }else if (val.Contains("(Star)"))
            {
                Value = new GridLength(double.Parse(val.Replace("(Star)","")), GridUnitType.Star);
            }
            else
            {
                Value = new GridLength(double.Parse(val), GridUnitType.Pixel);
            }
            
                
            var min = d.Attribute("Min");
            if (min != null)
                int.Parse(min.Value);
            var max = d.Attribute("Max");
            if (max != null)
                int.Parse(max.Value);
        }
        public RowColData() { }
        internal XElement GetXElement()
        {
            var ret = new XElement("Definition");
            if (Min >= 0)
                ret.SetAttributeValue("Min", Min);
            if (Max >= 0)
                ret.SetAttributeValue("Max", Max);
            ret.SetAttributeValue("Value", getString(Value));
            return ret;
        }
        public override string ToString()
        {
            return $"Value: {getString(Value)}, Min: {getString(Min)}, Max: {getString(Max)}";
        }
        private string getString(int x) => x >= 0 ? x.ToString() : "Unset";
        private string getString(GridLength g)
        {
            switch (g.GridUnitType)
            {
                case GridUnitType.Star:
                    return g.Value + "(Star)";
                case GridUnitType.Auto:
                    return "Auto";
                case GridUnitType.Pixel:
                    return g.Value.ToString();
            }
            throw new Exception("Shouldnt be able to reach here");
        }
        public RowColData Clone()
        {
            return  new RowColData() {Max=Max,Min=Min,Value=new GridLength(Value.Value,Value.GridUnitType) };
        }
    }
}
