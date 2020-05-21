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
    [Table("mtm_part_partfeatures")]
    public class Part_PartFeature
    {
        [ Key ]
        [Column("Mtm_part_partfeature_id")]
        public int Part_PartFeatureId { get; set; }

        [Column("Partfeature_id")]
        public int PartFeatureId { get; set; }
        [ForeignKey("PartFeatureId")]
        public PartFeature PartFeature { get; set; }

        [Column("Part_id")]
        public int PartId { get; set; }
        [ForeignKey("PartId")]
        public Part Part { get; set; }

        [Column("Feature_value")]
        public string FeatureValue { get; set; }

        public Part_PartFeature()
        {
        }

        public Part_PartFeature(int Part_id,
                             int Partfeature_id,
                             string Feature_value)
        {
            this.PartFeatureId = Partfeature_id;
            this.PartId = Part_id;
            this.FeatureValue = Feature_value;
        }
    }  
}
