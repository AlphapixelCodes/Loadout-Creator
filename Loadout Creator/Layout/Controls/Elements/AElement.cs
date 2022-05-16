using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public enum ElementTypes
    {
        Container,Display,Selector,Misc
    }
    public class AElement: INotifyPropertyChanged
    {
        public static Dictionary<ElementTypes, Tuple<string, Type>[]> ElementGroups = new Dictionary<ElementTypes, Tuple<string, Type>[]>() {
            {ElementTypes.Container, new Tuple<string, Type>[]{
                new Tuple<string, Type>("Grid",typeof(GridElement)),
                new Tuple<string, Type>("Stack",typeof(StackElement)),
                new Tuple<string, Type>("Pivot",typeof(PivotElement)),
            }},
            {ElementTypes.Misc, new Tuple<string, Type>[]{
                new Tuple<string, Type>("Button",typeof(ButtonElement)),
                new Tuple<string, Type>("Border",typeof(BorderElement)),
            }},
        };
        public static Tuple<string, Type>[] AllElements = new Tuple<string, Type>[]
        {
            new Tuple<string, Type>("Grid",typeof(GridElement)),
            new Tuple<string, Type>("Stack",typeof(StackElement)),
            new Tuple<string, Type>("Pivot",typeof(PivotElement)),
            new Tuple<string, Type>("PivotItem",typeof(PivotItemElement)),
            new Tuple<string, Type>("Button",typeof(ButtonElement)),
            new Tuple<string, Type>("Border",typeof(BorderElement)),
            new Tuple<string, Type>("Popup",typeof(PopupElement)),
            new Tuple<string, Type>("Popups",typeof(PopupContainerElement)),
        };
        public AElement Parent;
        internal string LabelName = "";
        private string _name;
        public bool canDrag = true;
        public bool canDelete = true;
        public bool OneChildOnly;
        public string Name { 
            get =>_name; 
            set { 
                _name = value;
                RaisePropertyChanged("DisplayName");
            } 
        }
        
        public string DisplayName { 
            get {
                var name = ElementType;
               
                if (Name != null && Name.Length > 0)
                    return name + ": " + Name;
                else if (LabelName.Length > 0)
                    return name + ": " + LabelName;
                else
                    return name;
            }
        }
        public virtual bool isSelector() => false;
        public virtual Dictionary<string, AProperty[]> GetProperties() => throw new NotImplementedException();
        public virtual void SetProperties(Dictionary<string, AProperty[]> p) => throw new NotImplementedException();
        public ObservableCollection<AElement> Children = new ObservableCollection<AElement>();
        public virtual string ElementType=>throw new NotImplementedException();
        public virtual bool AcceptsAnyChildType => true;

        public virtual Type GetUniqueChild=> null;

        public static List<AElement> LoadedPropertyGroups=new List<AElement>();
        internal PropertyGroupControl[] propertyGroups;
        public PropertyGroupControl[] PropertyGroups
        {
            get
            {
            
                if(LoadedPropertyGroups.Contains(this))
                    LoadedPropertyGroups.Remove(this);
                LoadedPropertyGroups.Add(this);
                if (LoadedPropertyGroups.Count > 4)
                {
                    LoadedPropertyGroups[0].propertyGroups = null;
                    Debug.WriteLine("property group unloaded","AElement.PropertyGroups");
                    GC.Collect();
                    LoadedPropertyGroups.RemoveAt(0);
                }
                
                if (propertyGroups == null)
                {
                    propertyGroups=GetProperties().Select(kv => new PropertyGroupControl(kv.Key, kv.Value)).ToArray();
                }
                return propertyGroups;
            }
        }
        public void DeleteElement()
        {
            Parent?.Children.Remove(this);
            if (LoadedPropertyGroups.Contains(this))
                LoadedPropertyGroups.Remove(this);
            foreach (var item in Children.ToArray())
            {
                item.DeleteElement();
            }
        }
        public AProperty GetProperty(string name, string group=null)
        {
            if (group != null)
            {
                return GetProperties()[group].FirstOrDefault(z => z.Name == name);
            }

            foreach (var proplist in GetProperties().Values)
            {
                var p=proplist.FirstOrDefault(z => z.Name == name);
                if (p != null)
                    return p;
            }
            return null;
        }
        

      /*  internal  Dictionary<ElementTypes, Tuple<string, Type>[]> CommandBarEvent(string Tag,TreeViewItem target)
        {
            switch (Tag)
            {
                case "Add":
                    if (OneChildOnly && Children.Count!=0)
                    {
                        new ConfirmDialog("Max Children", ElementType + " Can only have one child.", true).ShowAsync();
                        return null;
                    }
                    if (AcceptsAnyChildType)
                    {
                        return ElementGroups;
                    }
                    else if (GetUniqueChild != null)
                    {
                        AddChild((AElement)Activator.CreateInstance(GetUniqueChild));
                        target.IsExpanded = true;
                    }
                    else
                    {
                        new ConfirmDialog("Cannot Have Children", ElementType + " Cannot Have Child Elements", true).ShowAsync();
                    }
                    break;
                case "Delete":
                    if (canDelete)
                    {
                        var name = ElementType;
                        if (Name != null && Name.Length>0)
                            name = name + ": " + Name;
                        else
                            name = "this " + name;
                        var cd=new ConfirmDialog("Confirm Delete", $"Are you sure you want to delete {name}?");
                        cd.ConfirmEvent += (a, b) =>DeleteElement();
                        
                        cd.ShowAsync();
                    }
                    else
                    {
                        new ConfirmDialog("Cannot Delete", "Cannot Delete This Element", true).ShowAsync();
                    }
                    break;
                case "MoveUp":
                    if (canDrag &&Parent != null && Parent.Children.IndexOf(this)>0)
                    {
                        var indx = Parent.Children.IndexOf(this);
                        var temp = Parent.Children[indx - 1];
                        Parent.Children.Remove(temp);
                        Parent.Children.Insert(indx, temp);
                    }
                    break;
                case "MoveDown":
                    if (canDrag && Parent != null && Parent.Children.IndexOf(this) < Parent.Children.Count-1)
                    {
                        var indx = Parent.Children.IndexOf(this);
                        Parent.Children.Remove(this);
                        Parent.Children.Insert(indx + 1, this);
                    }
                    break;
                case "Copy":
                    if (Parent != null)
                    {
                        throw new NotImplementedException();  
                    }else
                        new ConfirmDialog("Cannot Copy", "Cannot Copy Root Element", true).ShowAsync();
                    break;
            }
            return null;
        }*/


        public virtual void AddChild(AElement child) {
            if (child.Parent != null)
            {
                child.Parent.Children.Remove(child);
            }
            Children.Add(child);
            child.Parent = this;
        }
     
        public bool CanHaveAnotherChild()
        {
            if (OneChildOnly && Children.Count != 0)
                return false;
            return AcceptsAnyChildType || GetUniqueChild!=null;//idk maybe remove the unique
            
        }


        internal void ThicknessToCornerRadius(AProperty tp, UIElement element, DependencyProperty property)
        {
            var t = (Thickness)tp.Value;
            element.SetValue(property, new CornerRadius(t.Left, t.Right, t.Bottom, t.Top));
        }

        
        public virtual UIElement BuildElement(LoadoutState state) => throw new NotImplementedException();

        //Properties

        internal void ApplyProperties(UIElement e)
        {
            foreach (var group in GetProperties())
            {
                foreach (var property in group.Value)
                {
                    if (property.Value != null && !property.ApplyDependencyProperty(e) && property.Name != "Label")
                    {
                        if (property.Name == "Corner Radius")
                        {
                         
                        }
                        else
                            ApplyUnknownProperty(e, property);
                    }
                }
            }
        }

        internal virtual AElement Clone()
        {
            AElement ret = (AElement)Activator.CreateInstance(GetType());
            var props = new Dictionary<string, AProperty[]>();
            foreach (var kv in GetProperties())
            {
                
                props.Add(kv.Key, kv.Value.Select(e => e.Clone()).ToArray());
                if (kv.Key == "Designer")
                {
                    var label = kv.Value.FirstOrDefault(e => e.Name == "Label");
                    if (label != null)
                    {
                        ret.InitLabelChangedEvent();
                        ret.LabelName = label.Value as string;
                    }
                }
            }
            ret.SetProperties(props);

            foreach (var child in Children)
            {
                ret.Children.Add(child.Clone());
            }
            return ret;
        }

        internal virtual void ApplyUnknownProperty(UIElement e,AProperty prop) => throw new NotImplementedException();
        internal enum FrequentPropertyGroup {
            Layout,Grid,Design
        }
        internal static AProperty GetFrequentProperty(string name, FrequentPropertyGroup group, DependencyProperty dep = null)
        {
            /*  if(group==FrequentPropertyGroup.Layout)
                  switch (name)
                  {
                  case "Width":
                      return new DoubleProperty() { Name = "Width", Dependency = FrameworkElement.WidthProperty };
                  case "MaxWidth":
                      return new DoubleProperty() { Name = "MaxWidth", Dependency = FrameworkElement.MaxWidthProperty, TabSize = 20 };
                  case "MinWidth":
                      return new DoubleProperty() { Name = "MinWidth", Dependency = FrameworkElement.MinWidthProperty, TabSize = 20 };
                  case "Height":
                      return new DoubleProperty() { Name = "Height", Dependency = FrameworkElement.HeightProperty };
                  case "MaxHeight":
                      return new DoubleProperty() { Name = "MaxHeight", Dependency = FrameworkElement.MaxHeightProperty, TabSize = 20 };
                  case "MinHeight":
                          return new DoubleProperty() { Name = "MinHeight", Dependency = FrameworkElement.MinHeightProperty, TabSize = 20 };
                  case "Horizontal Alignment":
                      return new EnumProperty<HorizontalAlignment>() { Name = "Horizontal Alignment", DefaultValue = HorizontalAlignment.Stretch,Value=HorizontalAlignment.Stretch, Dependency = FrameworkElement.HorizontalAlignmentProperty };
                  case "Vertical Alignment":
                      return new EnumProperty<VerticalAlignment>() { Name = "Vertical Alignment", DefaultValue = VerticalAlignment.Stretch, Value = HorizontalAlignment.Stretch, Dependency = FrameworkElement.VerticalAlignmentProperty };
                  case "Margin":
                      return new ThicknessProperty() { Name = "Margin", Dependency = FrameworkElement.MarginProperty };
                  case "Padding":
                      return new ThicknessProperty() { Name = "Padding", Dependency = Grid.PaddingProperty };

                  }
              else*/
            if (group == FrequentPropertyGroup.Design)
            {
                switch (name)
                {
                    case "Background":
                        return new ColorProperty() { Name = "Background", Dependency = dep };
                    case "Border":
                        return new ColorProperty() { Name = "Border", Dependency = dep };
                    case "Border Thickness":
                        return new ThicknessProperty() { Name = "Border Thickness", Dependency = dep };
                    case "Corner Radius":
                        return new ThicknessProperty() { Name = "Corner Radius", ShowIcons = false };

                }
            }
            if (group == FrequentPropertyGroup.Grid) { 
                switch (name)
                {
                    case "Row":
                        return new DoubleProperty() { Name = "Row", DefaultValue = 0, Dependency = Grid.RowProperty, CastType = typeof(int), isInt = true };
                    case "Column":
                        return new DoubleProperty() { Name = "Column", DefaultValue = 0, Dependency = Grid.ColumnProperty, CastType = typeof(int), isInt = true };
                    case "Row Span":
                        return new DoubleProperty() { Name = "Row Span", DefaultValue = 1, Dependency = Grid.RowSpanProperty, CastType = typeof(int), isInt = true };
                    case "Column Span":
                        return new DoubleProperty() { Name = "Column Span", DefaultValue = 1, Dependency = Grid.ColumnSpanProperty, CastType = typeof(int), isInt = true };
                }
            }
            throw new NotImplementedException($"Could not find {name} in group {group}");
            //return null;
        }
        internal static AProperty[] DefaultLayout(HorizontalAlignment h=HorizontalAlignment.Stretch,VerticalAlignment v=VerticalAlignment.Stretch)
        {
            var pht = "Unset";
            var auto = "Auto";
            return new AProperty[] {
                new DoubleProperty() { Name = "Width", Dependency = FrameworkElement.WidthProperty, PlaceholderText=auto},
                new DoubleProperty() { Name = "MaxWidth", Dependency = FrameworkElement.MaxWidthProperty, TabSize = 20,PlaceholderText=pht },
                new DoubleProperty() { Name = "MinWidth", Dependency = FrameworkElement.MinWidthProperty, TabSize = 20,PlaceholderText=pht ,AddLineAfter=true},
                new DoubleProperty() { Name = "Height", Dependency = FrameworkElement.HeightProperty,PlaceholderText=auto },
                new DoubleProperty() { Name = "MaxHeight", Dependency = FrameworkElement.MaxHeightProperty, TabSize = 20,PlaceholderText=pht },
                new DoubleProperty() { Name = "MinHeight", Dependency = FrameworkElement.MinHeightProperty, TabSize = 20,PlaceholderText=pht, AddLineAfter=true},
                new EnumProperty<HorizontalAlignment>() { Name = "Horizontal Alignment", DefaultValue = h, Value = h, Dependency = FrameworkElement.HorizontalAlignmentProperty },
                new EnumProperty<VerticalAlignment>() { Name = "Vertical Alignment", DefaultValue = v, Value = v, Dependency = FrameworkElement.VerticalAlignmentProperty, AddLineAfter=true },
                new ThicknessProperty() { Name = "Margin", Dependency = FrameworkElement.MarginProperty, AddLineAfter=true},
                new ThicknessProperty() { Name = "Padding", Dependency = Grid.PaddingProperty }
            };
        }
        internal static AProperty[] DefaultGrid()
        {
            return new AProperty[]
            {
                new DoubleProperty(){Name="Row",DefaultValue=0,Dependency=Grid.RowProperty, CastType=typeof(int),isInt=true},
                new DoubleProperty(){Name="Column",DefaultValue=0,Dependency=Grid.ColumnProperty,CastType=typeof(int),isInt=true, AddLineAfter=true},

                new DoubleProperty(){Name="Row Span",DefaultValue=1,Dependency=Grid.RowSpanProperty,CastType=typeof(int),isInt=true},
                new DoubleProperty(){Name="Column Span",DefaultValue=1,Dependency=Grid.ColumnSpanProperty,CastType=typeof(int),isInt=true}
            };
        }
      
        internal static AProperty[] DefaultDesigner() => new AProperty[] { new TextProperty() { Name = "Label", Value = "" } };
        internal void InitLabelChangedEvent()
        {
            GetProperty("Label", "Designer").PropertyChanged += (a, b) =>
            {
                LabelName = b as string;
                RaisePropertyChanged("DisplayName");
            };
        }


        public virtual XElement GetXElement()
        {
            var ret=new XElement(ElementType);
            if(Name!=null)
                ret.SetAttributeValue("Name", Name);
            if (!canDelete)
                ret.SetAttributeValue("canDelete", false);
            if(!canDrag)
                ret.SetAttributeValue("canDrag", false);

            foreach (var group in GetProperties())
            {
                foreach (var prop in group.Value)
                {
                    prop.AddXAttribute(ret);
                }
            }
            foreach (var child in Children)
            {
                ret.Add(child.GetXElement());
            }
            return ret;
        }
        public static AElement FromXElement(XElement e)
        {
            var type = AllElements.FirstOrDefault(z => z.Item1 == e.Name);
            if (type == null)
                return null;
            var ae = (AElement)Activator.CreateInstance(type.Item2);
            var nam = e.Attribute("Name");
            if (nam != null)
                ae.Name = nam.Value;

            var canDelete = e.Attribute("canDelete");
            if (canDelete != null)
                bool.TryParse(canDelete.Value,out ae.canDelete);

            var canDrag = e.Attribute("canDrag");
            if (canDrag != null)
                bool.TryParse(canDrag.Value, out ae.canDrag);

            foreach (var group in ae.GetProperties())
            {
                foreach (var prop in group.Value)
                {
                    
                    prop.LoadFromXElement(e);
                }
            }
            foreach (var child in e.Elements())
            {
                var c = FromXElement(child);
                if(c!=null)
                    ae.AddChild(c);
            }
            return ae;
        }



        //INotifyable Stuff
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}

