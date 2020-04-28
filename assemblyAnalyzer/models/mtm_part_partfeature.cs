using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace assemblyAnalyzer.models
{
    public class Mtm_part_partfeature : INotifyPropertyChanged
    {
        public Mtm_part_partfeature()
        {
        }

        public Mtm_part_partfeature(int Part_id,
                             int Partfeature_id,
                             string Feature_value) {
            partfeature_id = Partfeature_id;
            part_id = Part_id;
            feature_value = Feature_value;
        }

       // private int mtm_part_feature_id;
        private int partfeature_id;
        private int part_id;
        private string feature_value;
        
        [Key]
        public int Mtm_part_feature_id { get; set; }
        //{
        //    get { return mtm_part_feature_id; }
        //    set
        //    {
        //        mtm_part_feature_id = value;
        //        OnPropertyChanged("Mtm_part_feature_id");
        //    }
        //}

        public int Partfeature_id
        {
            get { return partfeature_id; }
            set
            {
                partfeature_id = value;
                OnPropertyChanged("Partfeature_id");
            }
        }

        public int Part_id
        {
            get { return part_id; }
            set
            {
                part_id = value;
                OnPropertyChanged("Part_id");
            }
        }

        public string Feature_value
        {
            get { return feature_value; }
            set
            {
                feature_value = value;
                OnPropertyChanged("Feature_value");
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
