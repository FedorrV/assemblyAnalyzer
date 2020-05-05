using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace assemblyAnalyzer.models
{
    [Table("Parts")]
    public class Part : INotifyPropertyChanged
    {
        [Key]
        [Column("Part_id")]
        public int PartId { get; set; }

        public string Name { get; set; }

        public byte [] Image { get; set; }

        [Column("Insert_date")]
        public string InsertDate { get; set; }
        
        [Index("IDX_INTERNAL_REVISION", 1, IsUnique = true)]
        [Column("Internal_name")]
        public string InternalName { get; set; }

        [Index("IDX_INTERNAL_REVISION", 2, IsUnique = true)]
        [Column("Revision_id")]
        public string RevisionId { get; set; }

        [Column("Database_revision_id")]
        public string DatabaseRevisionId { get; set; }

        [Column("Model_geometry_version")]
        public string ModelGeometryVersion { get; set; }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public virtual List<Part_PartFeature> Part_PartFeatures { get; set; }

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
            this.Name = Name;
            this.Image = Image;
            this.InsertDate = Insert_date;
            this.InternalName = InternalName;
            this.RevisionId = RevisionId;
            this.DatabaseRevisionId = DatabaseRevisionId;
            this.ModelGeometryVersion = ModelGeometryVersion;
            this.Description = Description;
            Part_PartFeatures = new List<Part_PartFeature>();
        }

        
       
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
