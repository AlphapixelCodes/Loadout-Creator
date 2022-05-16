using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Loadout_Creator
{
    public sealed partial class TreeControl : UserControl
    {
        public event EventHandler<AElement> ItemInvoked;
        private AElement _ElementBeingHovered;
        private MenuBarItemFlyout _flyout;
        public TreeControl()
        {
            this.InitializeComponent();
        }
        public void SetSource(object source) => tree.ItemsSource = source;
        /*private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var butt = (Button)sender;
            var target = _flyout.Target as TreeViewItem;
            var aElement = (AElement)(butt).DataContext;
            var resp= aElement.CommandBarEvent(butt.Tag as string,target);
            _flyout?.Hide();
            
            if (resp != null)
            {
            var f = new MenuFlyout();
                
                foreach (var item in resp)
                {
                    var sub = new MenuFlyoutSubItem();
                    sub.Text=item.Key.ToString();
                    f.Items.Add(sub);
                    foreach (var subitem in item.Value)
                    {
                        var m = new MenuFlyoutItem();
                        m.Text = subitem.Item1;
                        m.Click += (a, b) =>
                        {
                            aElement.AddChild((AElement)Activator.CreateInstance(subitem.Item2));
                            target.IsExpanded = true;
                        };
                        sub.Items.Add(m);
                    }
                }                
                f.ShowAt(target,new FlyoutShowOptions() { Position = new Point(0, 0) });
            }
        }
        private void CommandBarFlyout_Opening(object sender, object e)
        {
            return;
            var cbf = (Flyout)sender;
            throw new NotImplementedException("dont use this shit custom flyout just use a menu flyout");
            _flyout = cbf;
            var stack = cbf.Content as StackPanel;
            var element = (stack.Children[0] as Button).DataContext as AElement;
            var buttonstohide = new List<string>();
            if (!element.canDelete)
            {
                buttonstohide.Add("Delete");
                buttonstohide.Add("Copy");
            }
            if (!element.canDrag)
            {
                buttonstohide.Add("MoveUp");
                buttonstohide.Add("MoveDown");
            }
            else if (element.Parent != null)
            {
                var index = element.Parent.Children.IndexOf(element);
                if (index == 0)
                    buttonstohide.Add("MoveUp");
                if (index == element.Parent.Children.Count - 1)
                    buttonstohide.Add("MoveDown");
            }

            foreach (Button item in stack.Children)
            {
                item.Visibility = buttonstohide.Contains(item.Tag) ? Visibility.Collapsed : Visibility.Visible;
            }

        }*/
        private static Type isDragDataOfType<Type>(DragEventArgs e)
        {
            object obj;
            if (e.DataView.Properties.TryGetValue("ItemViewModel", out obj))
            {
                if (obj is Type)
                {
                    return (Type)obj;
                }
            }
            return default(Type);
        }
        private void TreeViewItem_DragEnter(object sender, DragEventArgs e)
        {
            var ea = isDragDataOfType<AElement>(e);
            AElement eaRecip;
            if (sender is TreeViewItem)
                eaRecip = (AElement)((TreeViewItem)sender).DataContext;
            else
                eaRecip = (AElement)((TextBlock)sender).DataContext;
            //Debug.WriteLine("ea: " + ea.DisplayName+ "\tearecip: " + eaRecip.DisplayName);

            e.DragUIOverride.IsCaptionVisible = true;

            e.DragUIOverride.IsGlyphVisible = false;
            if (ea != null && ea != eaRecip && eaRecip.CanHaveAnotherChild() && ea.canDrag)
            {
                e.DragUIOverride.Caption = "Can Drop";
                Debug.WriteLine("Can drop");
                _ElementBeingHovered = eaRecip;
            }
            else
            {
                _ElementBeingHovered = null;
                e.DragUIOverride.Caption = "Cannot Drop";
                Debug.WriteLine("Cannot drop");
            }
        }

        private void Tree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args) {
            ItemInvoked?.Invoke(this, args.InvokedItem as AElement);
        }

        private void Tree_DragItemsStarting(TreeView sender, TreeViewDragItemsStartingEventArgs args)
        {
            if (args.Items.Cast<AElement>().Any(e => !e.canDrag))
            {
                args.Cancel = true;
            }
            else
            {
                args.Data.RequestedOperation = DataPackageOperation.Move;
                args.Data.Properties.Add("ItemViewModel", args.Items[0] as AElement);

                Debug.WriteLine("Drag AElement Starting");
            }
           
        }
        private void TextBlock_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.Data.RequestedOperation = DataPackageOperation.Move;
            args.Data.Properties.Add("ItemViewModel", ((TextBlock)sender).DataContext as AElement);

            Debug.WriteLine("Drag AElement Starting");
        }


        private void TextBlock_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            if (_ElementBeingHovered != null)
            {
                _ElementBeingHovered.AddChild(((FrameworkElement)sender).DataContext as AElement);   
            }
        }

        private void MenuBarItemFlyout_Opening(object sender, object e)
        {
            _flyout = (MenuBarItemFlyout)sender;
          
            AElement de= _flyout.Target.DataContext as AElement;
            
            MenuFlyoutItemBase NoneButton = null;
            foreach (MenuFlyoutItemBase item in _flyout.Items.ToArray())
            {
                switch (item.Tag)
                {
                    case "AddItem":
                        if (de.CanHaveAnotherChild() && de.GetUniqueChild != null) {
                            item.Visibility = Visibility.Visible;
                            (item as MenuFlyoutItem).Text = "Add " + de.GetUniqueChild.Name.Replace("Element", "");
                        }
                        else
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                        break;
                    case "AddSub":
                        if (de.CanHaveAnotherChild() && de.AcceptsAnyChildType) {
                            item.Visibility =Visibility.Visible;
                            var add = item as MenuFlyoutSubItem;
                            add.Items.Clear();
                            foreach (var groups in AElement.ElementGroups)
                            {
                                var mfsi = new MenuFlyoutSubItem() { Text = groups.Key.ToString() };

                                foreach (var nameType in groups.Value)
                                {
                                    var mfi = new MenuFlyoutItem() { Text = nameType.Item1 };
                                    mfi.Click += (z, b) =>
                                    {
                                        de.AddChild((AElement)Activator.CreateInstance(nameType.Item2));
                                        (_flyout.Target as TreeViewItem).IsExpanded = true;
                                    };
                                    mfsi.Items.Add(mfi);
                                }
                                add.Items.Add(mfsi);
                            }
                        }
                        else
                            item.Visibility =Visibility.Collapsed;
                        break;
                    case "Delete":
                        item.Visibility=de.canDelete? Visibility.Visible: Visibility.Collapsed;
                        break;
                    case "Copy":
                        if (de.Parent != null && de.Parent.CanHaveAnotherChild())
                            item.Visibility = Visibility.Visible;
                        else
                            item.Visibility = Visibility.Collapsed;
                        break;
                    case "Move":
                        item.Visibility = de.canDrag&&de.Parent!=null ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "None":
                        NoneButton = item;
                        item.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            if (_flyout.Items.All(z =>z.Visibility==Visibility.Collapsed))
            {
               NoneButton.Visibility=Visibility.Visible;
            }          

        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var mfi = (MenuFlyoutItem)sender;
            var ae = mfi.DataContext as AElement;
           // var ae = ((MenuBarItemFlyout)((mfi.Parent is MenuFlyoutSubItem) ? ((MenuFlyoutSubItem)mfi.Parent).Parent : mfi.Parent)).Target.DataContext as AElement;
            switch (mfi.Tag)
            {
                case "Delete":
                    ae.DeleteElement();
                    break;
                case "Copy":
                    ae.Parent.AddChild(ae.Clone());
                    break;
                case "AddItem":
                    ae.AddChild((AElement)Activator.CreateInstance(ae.GetUniqueChild));
                    (_flyout.Target as TreeViewItem).IsExpanded = true;
                    break;
                case "Up":
                    var index1 = ae.Parent.Children.IndexOf(ae);
                    if (index1 > 0)
                    {
                        ae.Parent.Children.Remove(ae);
                        ae.Parent.Children.Insert(index1-1,ae);
                    }
                    break;
                case "Down":
                    var index2 = ae.Parent.Children.IndexOf(ae);
                    if (index2 <ae.Parent.Children.Count-1)
                    {
                        ae.Parent.Children.Remove(ae);
                        ae.Parent.Children.Insert(index2 + 1, ae);
                    }
                    break;
            }
        }
    }
}
