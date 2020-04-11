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
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private ApprenticeServerComponent aprServer; //экзепляр Apprentice Server
        private AssemblyAnalyzer assemblyAnalyzer; //анализатор сборок
        private DialogService dsOpenFile = new DialogService();
        private string pathFile;
        private ApplicationContext db;
        IEnumerable<Part> parts;

        public ApplicationViewModel()
        {
            aprServer = new ApprenticeServerComponent();
            if (aprServer == null)
            {
                DialogService.ShowMessage("Ошибка при подключении к Inventor ApprenticeServer");
                Environment.Exit(1);
            }

            try
            {
                db = new ApplicationContext();
                db.Parts.Load();
                
                //parts = db.Parts.Local.ToBindingList();
            }
            catch(Exception ex)
            {
                DialogService.ShowMessage(ex.Message);//"При попытке подключиться к БД возникла ошибка.");
                Environment.Exit(1);
            }
        }

        ~ApplicationViewModel()
        {
        }

        //команда "открыть сборку"
        //public CommandBinding cbOpenAssembly = new CommandBinding();

        private RelayCommand rcOpenAssembly;
        public RelayCommand RcOpenAssembly
        {
            get
            {
                return rcOpenAssembly ??
                    (rcOpenAssembly = new RelayCommand(obj =>
                    {
                        if (dsOpenFile.OpenFileDialog("Assembly files|*.iam"))
                        {
                            try
                            {
                                pathFile = dsOpenFile.FilePath;
                                assemblyAnalyzer = new AssemblyAnalyzer(pathFile, aprServer);
                                assemblyAnalyzer.getAllParts();
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
