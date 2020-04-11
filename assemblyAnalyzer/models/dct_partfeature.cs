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
    public class Dct_partfeature: INotifyPropertyChanged
    {
        [Key]
        public int Dct_partfeature_id { get; set; }
        private string name;

        Dct_partfeature()
        {

        }

        Dct_partfeature(string Name)
        {
            this.name = Name;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
