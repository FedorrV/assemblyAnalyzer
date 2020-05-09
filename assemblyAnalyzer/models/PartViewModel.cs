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

    public class PartViewModel : INotifyPropertyChanged
    {
        public PartViewModel() { }
        public PartViewModel(string Name, string InternalName, string RevisionId, string DatabaseRevisionId, string ModelGeometryVersion, stdole.IPictureDisp image, Dictionary<string, string> properties)
        {
            this.IsSaved = false;
            //this.Properties = properties;
            this.name = Name;
            this.InternalName = InternalName;
            this.RevisionId = RevisionId;
            this.DatabaseRevisionId = DatabaseRevisionId;
            this.ModelGeometryVersion = ModelGeometryVersion;
            this.Image = image;
            this.Properties = properties;
        }

        //public ApprenticeServerDocument PartDocument;
        public string InternalName;
        public string RevisionId;
        public string DatabaseRevisionId;
        public string ModelGeometryVersion;

        public stdole.IPictureDisp Image;

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

        private Dictionary<string, string> properties;
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
