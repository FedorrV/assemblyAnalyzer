using assemblyAnalyzer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assemblyAnalyze.ViewModels
{
    public class SavePartViewModel: BasicViewModel, INotifyPropertyChanged
    {
        public SavePartViewModel() { }

        public SavePartViewModel(object partName)
        {
            this.PartName = partName as string;
        }

        public bool IsSaved = false;

        private string partDescription;
        public string PartDescription
        {
            get { return partDescription; }
            set
            {
                partDescription = value;
                if (partDescription.Length == 150)
                    AlertMessageDescription = "Не более 150 символов";
                else
                    AlertMessageDescription = "";
                OnPropertyChanged("PartDescription");
            }
        }

        private string partName;
        public string PartName
        {
            get { return partName; }
            set
            {
                partName = value;
                if (partName.Length == 50)
                    AlertMessageName = "Не более 50 символов";
                else
                    AlertMessageName = "";
                OnPropertyChanged("PartName");
            }
        }
        
        private string alertMessageDescription;
        public string AlertMessageDescription
        {
            get { return alertMessageDescription; }
            set
            {
                alertMessageDescription = value;
                OnPropertyChanged("AlertMessageDescription");
            }
        }

        private string alertMessageName;
        public string AlertMessageName
        {
            get { return alertMessageName; }
            set
            {
                alertMessageName = value;
                OnPropertyChanged("AlertMessageName");
            }
        }

        private SimpleCommand cmdSavePart;
        public SimpleCommand CmdSavePart
        {
            get
            {
                return cmdSavePart ??
                    (cmdSavePart = new SimpleCommand(obj =>
                    {
                        IsSaved = true;
                    }, (obj)=> {return !string.IsNullOrEmpty(obj?.ToString()) && !string.IsNullOrEmpty(PartName); }));
            }
        }
    }
}
