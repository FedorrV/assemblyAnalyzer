using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace assemblyAnalyzer.models
{
    [Table("Dct_partfeatures")]
    public class PartFeature
    {
        public PartFeature() { }

        public PartFeature(string Name)
        {
            this.Name = Name;
        }

        [Key]
        [Column("Dct_partfeature_id")]
        public int PartFeatureId { get; set; }

        [Column("Name")]
        public string Name { get; set; }
    }
}
