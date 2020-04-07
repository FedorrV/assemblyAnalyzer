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

namespace assemblyAnalyze
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel()
        {
            //реализация команды "открыть сборку"
            //cbOpenAssembly.Command = ApplicationCommands.Open;
            //cbOpenAssembly.Executed += openAssembly;
            aprServer = new ApprenticeServerComponent();
            if (aprServer == null)
            {
                dsOpenFile.ShowMessage("Ошибка при подключении к Inventor ApprenticeServer");
                Environment.Exit(1);
            }
        }

        ~ApplicationViewModel()
        {
        }

        ApprenticeServerComponent aprServer;
        private AssemblyAnalyzer assemblyAnalyzer;
        private DialogService dsOpenFile = new DialogService();

        private string pathFile;

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
                                //assemblyAnalyzer.Initiolize(aprServer);
                                assemblyAnalyzer.getAllParts();
                                assemblyAnalyzer.getAllProperties();
                                //...
                                dsOpenFile.ShowMessage("Открытие файла");
                            }
                            catch(Exception ex)
                            {
                                dsOpenFile.ShowMessage(ex.Message);
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

        public void newFunc()
        {
            //ApprenticeServerComponent ap = new ApprenticeServerComponent();
            //ApprenticeServerDocument doc = ap.Open(pathFile);
            //doc.Type;
        }
    }
}
