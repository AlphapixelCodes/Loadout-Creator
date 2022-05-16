using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;

namespace Loadout_Creator
{
    public abstract class AProperty
    {
        public event EventHandler<object> PropertyChanged;

        public bool AddLineAfter = false;
        public string Name { get; set; }
        public DependencyProperty Dependency;
        public Type CastType;
        //private string xmlName;
        public string XMLName => new Regex("[^a-zA-Z0-9]").Replace(Name,"");

        public object DefaultValue;

        //public object AttachedValue;//currently being used for corner radius
        private object _value;
        public object Value { 
            get=>_value; 
            set {
                _value = value;
                PropertyChanged?.Invoke(this, value);
            }
        }


        public int TabSize=0;

        /// <summary>
        /// attempts to apply the dependency property to the uielement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool ApplyDependencyProperty(UIElement element)
        {
            //Debug.WriteLine($"{Name} : {Value}");
            if (Dependency == null)
                return false;
            if (isDefault())
            {
                Debug.WriteLine($"{Name} : {Value}","\tis Default");
                return true;
            }
            if (Value != null)
            {
                if (this is DoubleProperty && double.IsNaN((double)Convert.ChangeType(Value,typeof(double))))
                    return true;
                try
                {
                    Debug.WriteLine($"Setting Property: {Name} to {Value}", "AProperty");
                    if(CastType!=null)
                        element.SetValue(Dependency, Convert.ChangeType(Value,CastType));
                    else
                        element.SetValue(Dependency, Value);
                }catch
                {
                    Debug.WriteLine($"Error Setting Property {Name} to {Value}","AProperty");
            //       throw new NotImplementedException("Property failure: "+Name +", "+ex.Message);
                }
            }
            return true;

        }
        public virtual bool isDefault()
        {
            if (Value == null)
                return true;
            if(DefaultValue != null)
            {
                return Value.Equals(DefaultValue);
            }
            return false;
        }
        public virtual void AddXAttribute(XElement x)
        {
            if(!isDefault())
                x.SetAttributeValue(XMLName, Value);
        }

        internal abstract void FromXElement(XElement e,string value);
        public void LoadFromXElement(XElement e)
        {
            var a = e.Attribute(XMLName);
            if (a!=null)
                FromXElement(e,a.Value);
        }
        public abstract PropertyControl GetControl();
        internal virtual AProperty Clone()
        {
            var clone = (AProperty)Activator.CreateInstance(GetType());
            clone.Name = Name;
            clone.Value = Value;
            clone.DefaultValue = DefaultValue;
            clone.Dependency = Dependency;
            clone.AddLineAfter= AddLineAfter;
            clone.TabSize = TabSize;
            return clone;
        }
    }
}
