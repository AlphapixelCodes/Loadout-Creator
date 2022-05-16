using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Loadout_Creator
{
    public class LoadoutState
    {
        public Dictionary<string, UIElement> Popups,SelectionElements;
        public ObservableCollection<ErrorMessageClass> ErrorMessages;
        private bool isEditor;
        public LoadoutState(bool editor)
        {
            isEditor = editor;
            if(isEditor)
                ErrorMessages = new ObservableCollection<ErrorMessageClass>();
            Popups=new Dictionary<string, UIElement>();
            SelectionElements = new Dictionary<string, UIElement>();
        }
        public void AddPopup(PopupElement element)
        {
            if (element.Name==null || element.Name.Length == 0)
                LogError("Unnamed Popup","There is an Unnamed Popup (Popup will be unreachable by Buttons)");
            else if (Popups.ContainsKey(element.Name))
            {
                LogError("Duplicate Name","Duplicate Popup Names: \""+element.Name+"\" (Only first popup with name is kept)");
            }else
            {
                if(element.Children.Count==1)
                    Popups.Add(element.Name,element.Children[0].BuildElement(this));
                else
                    LogError("Empty Popup", $"Popup \"{element.Name}\" does not have any content");
            }
        }
        public void AddSelectionElement(AElement element)
        {
            throw new NotImplementedException();
          /*  if (element.Name.Length == 0)
                LogError("There is an Unnamed Popup (Popup will be unreachable by Buttons)");
            else if (Popups.ContainsKey(element.Name))
            {
                LogError("Duplicate Popup Names: \"" + element.Name + "\" (Only first popup with name is kept)");
            }
            else
            {
                Popups.Add(element.Name, element.Children[0].BuildElement(this));
            }*/
        }

        internal void ShowPopup(string v)
        {
            if (Popups.ContainsKey(v))
            {

            }
            else
            {
                LogError("Button Couldn't Find Popup", $"No Popup exists by the name \"{v}\"");
            }
        }

        public void LogError(string title,string message)
        {
            if(isEditor)
                ErrorMessages.Insert(0,new ErrorMessageClass(title,message));
        }
        
    }
    public class ErrorMessageClass
    {
        public string Title { get; }
        public string Message { get; }
        public ErrorMessageClass(string title,string message)
        {
            Title = title;
            Message = message;
        }
    }
}
