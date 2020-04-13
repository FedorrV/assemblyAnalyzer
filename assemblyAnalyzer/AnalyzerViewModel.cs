using assemblyAnalyze.Services;
using assemblyAnalyze;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using InventorApprentice;
using assemblyAnalyzer.models;
using System.Data.Entity;
using assemblyAnalyzer;

namespace assemblyAnalyze
{
    public class AnalyzerViewModel : INotifyPropertyChanged
    {
        private AssemblyAnalyzer assemblyAnalyzer; //анализатор сборок
        private ApprenticeServerDocument activeAssembly;
        private DialogService dsOpenFile = new DialogService();
        private string filePath;
        private AnalyzerContext db;
        IEnumerable<Part> parts;

        public AnalyzerViewModel()
        {
            
            try
            {
                assemblyAnalyzer = new AssemblyAnalyzer();
                //db = new ApplicationContext();
                //db.Parts.Load();
                
                //parts = db.Parts.Local.ToBindingList();
            }
            catch(Exception ex)
            {
                DialogService.ShowMessage(ex.Message);//"При попытке подключиться к БД возникла ошибка.");
                Environment.Exit(1);
            }
        }

        ~AnalyzerViewModel()
        {
        }

        //команда "открыть сборку"
        private RelayCommand cmdOpenAssembly;
        public RelayCommand CmdOpenAssembly
        {
            get
            {
                return cmdOpenAssembly ??
                    (cmdOpenAssembly = new RelayCommand(obj =>
                    {
                        if (dsOpenFile.OpenFileDialog("Assembly files|*.iam"))
                        {
                            try
                            {
                                filePath = dsOpenFile.FilePath;
                                assemblyAnalyzer.OpenAssembly(filePath);
                                foreach(ApprenticeServerDocument part in assemblyAnalyzer.Parts)
                                {

                                }
                            }
                            catch(Exception ex)
                            {
                                DialogService.ShowMessage(ex.Message);
                            }
                            
                        }
                    })
                    );
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
