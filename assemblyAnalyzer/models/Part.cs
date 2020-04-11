using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace assemblyAnalyzer.models
{
    public class Part : INotifyPropertyChanged
    {
        [Key]
        public int Part_id { get; set; }
        private string name;
        private byte [] image;
        private string insert_date;
        private string internal_name;
        private string revision_id;
        private string database_revision_id;
        private string model_geometry_version;
        private string description;

        public Part()
        {
        }

        public Part(string Name,
                     string Insert_date,
                     string InternalName,
                     string RevisionId,
                     string DatabaseRevisionId,
                     string ModelGeometryVersion,
                     byte[] Image= null,
                     string Description= null)
        {
            this.name = Name;
            this.image = Image;
            this.insert_date = Insert_date;
            this.internal_name = InternalName;
            this.revision_id = RevisionId;
            this.database_revision_id = DatabaseRevisionId;
            this.model_geometry_version = ModelGeometryVersion;
            this.description = Description;
        }
        

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Insert_date
        {
            get { return insert_date; }
            set
            {
                insert_date = value;
                OnPropertyChanged("Insert_date");
            }
        }

        public byte [] Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged("Image");
            }
        }

        public string Internal_name
        {
            get { return internal_name; }
            set
            {
                internal_name = value;
                OnPropertyChanged("Internal_name");
            }
        }

        public string Revision_id
        {
            get { return revision_id; }
            set
            {
                revision_id = value;
                OnPropertyChanged("Revision_id");
            }
        }

        public string Database_revision_id
        {
            get { return database_revision_id; }
            set
            {
                database_revision_id = value;
                OnPropertyChanged("Database_revision_id");
            }
        }

        public string Model_geometry_version
        {
            get { return model_geometry_version; }
            set
            {
                model_geometry_version = value;
                OnPropertyChanged("Model_geometry_version");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
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
