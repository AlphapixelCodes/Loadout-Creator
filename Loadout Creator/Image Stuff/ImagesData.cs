using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Linq;
using System.Xml.Linq;
using Windows.Graphics.Imaging;

namespace Loadout_Creator
{
    public class ImagesData
    {
        public ObservableCollection<ImageData> Images=new ObservableCollection<ImageData>();
        public ObservableCollection<string> Groups=new ObservableCollection<string>();
        private SchemaData schemaData;

        public ImagesData(SchemaData schemaData)
        {
            this.schemaData = schemaData;
        }

        public ImageData AddImage(StorageFile file,bool autoID=true)
        {

            /*try
            {*/
            uint id;
            
            if (autoID || !uint.TryParse(file.DisplayName, out id))
            {
                id = NextAvaliableID();
            }
            var ret = new ImageData(id);
            ret.SetImage(file);
            Images.Add(ret);
            return ret;
            /*  }
              catch
              {
                  return null;
              }*/
            
        }
        internal async Task<ImageData> AddImage(XElement xImg, StorageFile imageFile)
        {
            ImageData ret=ImageData.FromXElement(xImg);
            await ret.SetImage(imageFile);
            if (ret.Group != null && !Groups.Contains(ret.Group))
                Groups.Add(ret.Group);
            if(Images.Any(e=>e.ID==ret.ID))
                ret.ID = NextAvaliableID();
            Images.Add(ret);
            return ret;
        }
        public Tuple<List<ImageData>,Dictionary<string,List<ImageData>>> GetGroups()
        {
            var Dict =new Dictionary<string,List<ImageData>>();
            var ungrouped=new List<ImageData>();    
            foreach (var image in Images)
            {
                if (image.Group != null)
                {
                    if (Dict.ContainsKey(image.Group))
                    {
                        Dict[image.Group].Add(image);
                    }
                    else
                    {
                        Dict.Add(image.Group, new List<ImageData>() { image });
                    }
                }
                else
                {
                    ungrouped.Add(image);
                }
            }
            return new Tuple<List<ImageData>, Dictionary<string, List<ImageData>>>(ungrouped,Dict);
        }
        private uint NextAvaliableID()
        {
            uint i = 0;
            while (Images.Any(image => image.ID == i))
            {
                i++;
            }
            return i;
        }
        public XElement GetXElement()
        {
            var ret = new XElement("ImagesData");
            var groups = new XElement("Groups");
            groups.Add(Groups.Select(e=>new XElement("Group",e)).ToArray());
            ret.Add(groups);
            var data = new XElement("Images");
            data.Add(Images.Select(e => e.GetXElement()).ToArray());
            ret.Add(data);
            return ret;
        }

      
    }
    
}