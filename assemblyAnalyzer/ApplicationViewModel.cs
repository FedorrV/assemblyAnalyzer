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

namespace assemblyAnalyze
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel()
        {
            //реализация команды "открыть сборку"
            //cbOpenAssembly.Command = ApplicationCommands.Open;
            //cbOpenAssembly.Executed += openAssembly;
            app = getInventorApp();
            if (app == null)
            {
                dsOpenFile.ShowMessage("Ошибка при подключении к Inventor API. Попробуйте перезапустить программу");
                Environment.Exit(1);
            }
        }

        ~ApplicationViewModel()
        {
            shutDownInventorApp();
        }

        private Inventor.Application app;
        private assemblyAnalyzer assemblyAn;
        private DialogService dsOpenFile = new DialogService();

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
                        String pathFile;
                        if (dsOpenFile.OpenFileDialog("Assembly files|*.iam"))
                        {
                            try
                            {
                                pathFile = dsOpenFile.FilePath;
                                assemblyAn = new assemblyAnalyzer(pathFile);
                                assemblyAn.Initiolize(app);
                                assemblyAn.getAllParts();
                                assemblyAn.getAllProperties();
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

        private Inventor.Application getInventorApp()
        {
            Inventor.Application invApp;
            try
            {
                invApp = System.Runtime.InteropServices.Marshal.
                    GetActiveObject("Inventor.Application") as Inventor.Application;
            }
            catch
            {
                try
                {
                    invApp = Activator.CreateInstance(
                        Type.GetTypeFromProgID("Inventor.Application")) as Inventor.Application;
                }
                catch
                {
                    invApp = null;
                }
                //invApp.Visible = true;
            }
            return invApp;
        }
        public void shutDownInventorApp()
        {
            if (app != null)
                app.Quit();
        }
    }
}
