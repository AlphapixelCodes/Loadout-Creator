using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
namespace Loadout_Creator
{
    public class ImageData: INotifyPropertyChanged
    {
        public uint ID { get; set; }
        private string group;
        public string Group { get=>group; set { 
            group = value;
            RaisePropertyChanged("Group");
            } }
        private SoftwareBitmapSource image;
        public SoftwareBitmapSource Image { get=>image; 
            set { 
            image = value;
            RaisePropertyChanged("Image");
            } }

        public SoftwareBitmap SoftwareBitmap { get; internal set; }

        public ImageData(uint id, string group=null)
        {
            Image= image;
            ID= id;
            Group= group;
        }

        public XElement GetXElement()
        {
            var ret= new XElement("ImageData");
            ret.SetAttributeValue("id", ID);
            if(group!=null)
                ret.SetAttributeValue("group", group);
            return ret;
        }
        public static ImageData FromXElement(XElement xElement)
        {
            
            if (xElement.Attribute("id") != null && uint.TryParse(xElement.Attribute("id").Value,out uint id))
            {
                if (xElement.Attribute("group") != null)
                    return new ImageData(id, xElement.Attribute("group").Value);
                return new ImageData(id, null);
            }
            return null;
        }
        public async Task SetImage(StorageFile file)
        {
            var decoder = await BitmapDecoder.CreateAsync(await file.OpenReadAsync());
            var softBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            var source = new SoftwareBitmapSource();
            await source.SetBitmapAsync(softBitmap);
            Image= source;
            SoftwareBitmap= softBitmap;
        }
        public void Dispose()
        {
            Image.Dispose();
            SoftwareBitmap.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
