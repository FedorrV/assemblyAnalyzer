using assemblyAnalyze.Services;
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
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace assemblyAnalyze
{
    public class AnalyzerViewModel : INotifyPropertyChanged
    {
        public AnalyzerViewModel()
        {
            //DGParts = new ObservableCollection<DGPartItem>();
            try
            {
                assemblyAnalyzer = new AssemblyAnalyzer();
                //db = new ApplicationContext();
                //db.Parts.Load();

                //parts = db.Parts.Local.ToBindingList();
            }
            catch (Exception ex)
            {
                DialogService.ShowMessage(ex.Message);//"При попытке подключиться к БД возникла ошибка.");
                Environment.Exit(1);
            }
        }

        private AssemblyAnalyzer assemblyAnalyzer; //анализатор сборок
        private ApprenticeServerDocument activeAssembly;
        private DialogService dsOpenFile = new DialogService();
        private string filePath;
        private AnalyzerContext db;
        IEnumerable<Part> parts;

        public ObservableCollection<DGPartItem> DGParts { get; set;} = new ObservableCollection<DGPartItem>();

        public DGPartItem selectedDGPart;
        public DGPartItem SelectedDGPart
        {
            get { return selectedDGPart; }
            set
            {
                selectedDGPart = value;
                OnPropertyChanged("SelectedDGPart");
                DGProperties = selectedDGPart.Properties;
            }
        }

        public Dictionary<string, string> dGProperties = new Dictionary<string, string>();
        public Dictionary<string, string> DGProperties
        {
            get { return dGProperties; }
            set
            {
                dGProperties = value;
                OnPropertyChanged("DGProperties");
            }
        }

        //public ObservableCollection<DGPartItem> DGParts
        //{
        //    get
        //    {
        //        return DGParts;
        //    }
        //    set
        //    {
        //        DGParts = value
        //    }
        //}


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
                                DGPartItem temp = SelectedDGPart;
                                filePath = dsOpenFile.FilePath;
                                assemblyAnalyzer.OpenAssembly(filePath);
                                DGParts.Clear();
                                foreach(ApprenticeServerDocument part in assemblyAnalyzer.Parts)
                                {
                                    DGParts.Add(new DGPartItem(part, AssemblyAnalyzer.getPartProperties(part), true));
                                }
                                DGPartItem tt = new DGPartItem();
                                tt.Name = "asdfasdsssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssdfsd";
                                tt.IsSaved = true;
                                DGParts.Add(tt);
                                DGProperties = AssemblyAnalyzer.getPartProperties(assemblyAnalyzer.Parts[0]);
                                OnPropertyChanged("DGParts");
                                OnPropertyChanged("DGProperties");
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

        //команда "сохранить деталь"
        private RelayCommand cmdSavePart;
        public RelayCommand CmdSavePart
        {
            get
            {
                return cmdSavePart ??
                    (cmdSavePart = new RelayCommand(obj =>
                    {
                        
                                DGPartItem temp = SelectedDGPart;
                        int a = 0;
                    }, obj => obj != null)
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
