using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using InventorApprentice;

namespace assemblyAnalyzer
{
    
    public class DGPartItem
    {
        public DGPartItem()
        {}
        public DGPartItem(ApprenticeServerDocument partDoc, Dictionary<string, string> properties, bool isSaved= false )
        {
            this.PartDoc = partDoc;
            this.IsSaved = isSaved;
            this.Properties = properties;
            this.Name = partDoc.DisplayName;
        }

        public ApprenticeServerDocument PartDoc;

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private bool isSaved;
        private Dictionary<string, string> properties;
        public bool IsSaved
        {
            get
            {
                return isSaved;
            }
            set
            {
                isSaved = value;
                OnPropertyChanged("IsSaved");
            }
        }

        public Dictionary<string, string> Properties
        {
            get
            {
                return properties;
            }
            set
            {
                properties = value;
                OnPropertyChanged("Properties");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
