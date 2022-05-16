using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadout_Creator
{
    public class Query : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        private DataSet dataSet;
        public DataSet DataSet
        {
            get => dataSet;
            set
            {
                dataSet = value;
                RaisePropertyChanged("DataSetDisplayName");
            }
        }
        public string DataSetDisplayName => (dataSet != null) ? dataSet.Name : "";

        private string[] groups;
        public string[] Groups
        {
            get => groups;
            set
            {
                groups = value;
                RaisePropertyChanged("GroupsString");
            }
        }
        public string GroupsString => string.Join(", ", groups);

        public Query(string Name)
        {
            this.Name = Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
