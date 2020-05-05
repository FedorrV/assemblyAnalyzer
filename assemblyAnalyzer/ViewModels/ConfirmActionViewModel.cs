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
        public bool IsConfirmed;

        public ConfirmActionViewModel()
        {
            IsConfirmed = false;
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
                    }));
            }
        }
    }
}
