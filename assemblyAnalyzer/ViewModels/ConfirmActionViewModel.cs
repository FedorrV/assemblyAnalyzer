using assemblyAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assemblyAnalyze.ViewModels
{
    public class ConfirmActionViewModel:BasicViewModel
    {

        public ConfirmActionViewModel(object questionText)
        {
            this.QuestionText = questionText as string;
        }

        public ConfirmActionViewModel()
        {
            IsConfirmed = false;
            ExistsResult = false;
        }

        public bool IsConfirmed;
        public bool ExistsResult;

        private string questionText;
        public string QuestionText
        {
            get { return questionText; }
            set
            {
                questionText = value;
                OnPropertyChanged("QuestionText");
            }
        }


        private SimpleCommand cmdConfirmAction;
        public SimpleCommand CmdConfirmAction
        {
            get
            {
                return cmdConfirmAction ??
                    (cmdConfirmAction = new SimpleCommand(obj =>
                    {
                        IsConfirmed = true;
                        ExistsResult = true;
                    }));
            }
        }

        private SimpleCommand cmdCancelAction;
        public SimpleCommand CmdCancelAction
        {
            get
            {
                return cmdCancelAction ??
                    (cmdCancelAction = new SimpleCommand(obj =>
                    {
                        ExistsResult = true;
                    }));
            }
        }
    }
}
