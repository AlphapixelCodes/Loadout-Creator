using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml.Media.Imaging;

namespace Loadout_Creator
{
    internal class Image_DataColumn : ADataColumn
    {
        public override void EditDataColumn(SchemaData currentSchema=null)
        {
            if (currentSchema.ImagesData.Images.Count == 0)
            {
                new ConfirmDialog("No Images", "There are no images in this project. You can add images in the \"Images\" tab.", true).ShowAsync();
            }
            else
            {
                var isd = new ImageSelectionDialog(currentSchema);
                isd.ConfirmedEvent += (a, b) =>
                {
                    Value = b.ID;
                };
                isd.ShowAsync();
            }
        }

        public override string getTypeName() => "Image";
        internal override Type GetNonAbstractType() => typeof(Image_DataColumn);
        public override string valueDisplayString()
        {
            if (Value == null)
                return "Unset";
            return Value.ToString();
            //  return (Value as BitmapImage).
        }

        public override void ApplyXElementValue(XElement x)
        {
            var val = x.Attribute("Value");
            if (val != null && val.Value != "Null")
                Value=uint.Parse(val.Value);
        }
    }
}
