using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Loadout_Creator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageGroupViewPage : Page
    {
        private ImageManagementPage imageManagementPage;
        internal IEnumerable<ImageGroup> ImageGroups=>MainStacks.Children.OfType<ImageGroup>();

        public ImageGroupViewPage(ImageManagementPage imageManagementPage)
        {
            this.imageManagementPage = imageManagementPage;
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Clear()=>MainStacks.Children.Clear();

        public void UpdateGrouping()
        {
            MainStacks.Children.Clear();
            var groups=imageManagementPage.SchemaData.ImagesData.GetGroups();
            if (groups.Item1.Count > 0)
            {
                MainStacks.Children.Add(new ImageGroup("Ungrouped", groups.Item1, imageManagementPage));
            }
            foreach (var item in groups.Item2.OrderBy(e=>e.Key))
            {
                MainStacks.Children.Add(new ImageGroup(item.Key, item.Value, imageManagementPage));
            }
        }
     
    }
}
