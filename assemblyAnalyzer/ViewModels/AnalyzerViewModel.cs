using assemblyAnalyze.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using InventorApprentice;
using assemblyAnalyzer.models;
using System.Data.Entity;
using assemblyAnalyzer;
using System;
using System.Collections.ObjectModel;

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

        private ObservableCollection<DGPartItem> DGParts { get; set; } = new ObservableCollection<DGPartItem>();

        private ObservableCollection<DGPartItem> filteredDGParts = new ObservableCollection<DGPartItem>();
        public ObservableCollection<DGPartItem> FilteredDGParts
        {
            get
            {
                return filteredDGParts;
            }
            set
            {
                filteredDGParts = value;
                OnPropertyChanged("FilteredDGParts");
            }
        }

        private DGPartItem selectedDGPart;
        public DGPartItem SelectedDGPart
        {
            get { return selectedDGPart; }
            set
            {
                selectedDGPart = value;
                OnPropertyChanged("SelectedDGPart");
                if (selectedDGPart == null)
                    DGProperties = null;
                else
                    DGProperties = selectedDGPart.Properties;
            }
        }

        private Dictionary<string, string> dGProperties = new Dictionary<string, string>();
        public Dictionary<string, string> DGProperties
        {
            get { return dGProperties; }
            set
            {
                dGProperties = value;
                OnPropertyChanged("DGProperties");
            }
        }

        private string aA_SearchText;
        public string AA_SearchText
        {
            get
            {
                return aA_SearchText;
            }
            set
            {
                aA_SearchText = value;
                OnPropertyChanged("AA_SearchText");
                if (aA_SearchText == "")
                    FilteredDGParts = DGParts;
                else
                    FilteredDGParts = new ObservableCollection<DGPartItem>(DGParts.Where(x => x.Name.ToLower().Contains(aA_SearchText.ToLower())));
                //if (FilteredDGParts.Count != DGParts.Count)
                    //DGProperties = null;
            }
        }
        
        private stdole.IPictureDisp curPartImage;
        public stdole.IPictureDisp CurPartImage 
        {
            get
            {
                return curPartImage;
            }
            set
            {
                curPartImage = value;
                OnPropertyChanged("CurPartImage");
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
                                DGPartItem temp = SelectedDGPart;
                                filePath = dsOpenFile.FilePath;
                                assemblyAnalyzer.OpenAssembly(filePath);
                                DGParts.Clear();
                                foreach(ApprenticeServerDocument part in assemblyAnalyzer.Parts)
                                {
                                    DGParts.Add(new DGPartItem(part, AssemblyAnalyzer.getPartProperties(part), false));
                                }
                                DGPartItem tt = new DGPartItem() ;
                                CurPartImage = DGParts[0].PartDoc.Thumbnail;
                                tt.Name = "dddddddddddddddddddddddddddddddddddddddd";
                                tt.IsSaved = false;
                                DGParts.Add(tt);
                                FilteredDGParts = DGParts;

                                if(aA_SearchText!="" && aA_SearchText!=null)
                                    FilteredDGParts = new ObservableCollection<DGPartItem>(DGParts.Where(x => x.Name.ToLower().Contains(aA_SearchText.ToLower())));
                                DGProperties = null;
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
                    }, (obj) =>
                    {
                        DGPartItem temp = obj as DGPartItem;
                        return temp != null && temp.IsSaved == false;
                    }
                    ));
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
