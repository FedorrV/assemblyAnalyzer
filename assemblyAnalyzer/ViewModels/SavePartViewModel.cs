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
        public bool IsSaved = false;
        private string textValue;
        public string TextValue
        {
            get { return textValue; }
            set
            {
                textValue = value;
                if (textValue.Length == 150)
                    AlertMessage = "Не более 150 символов";
                else
                    AlertMessage = "";
                OnPropertyChanged("TextValue");
            }
        }

        private string alertMessage;
        public string AlertMessage
        {
            get { return alertMessage; }
            set
            {
                alertMessage = value;
                OnPropertyChanged("AlertMessage");
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
                    }, (obj)=> {return obj != null && obj.ToString().Length != 0; }));
            }
        }

        
    }
}
