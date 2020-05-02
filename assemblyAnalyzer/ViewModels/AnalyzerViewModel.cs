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
using assemblyAnalyze.ViewModels;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Threading.Tasks;

namespace assemblyAnalyze
{
    public class AnalyzerViewModel : BasicViewModel,INotifyPropertyChanged 
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
                FileDialogService.ShowMessage(ex.Message);//"При попытке подключиться к БД возникла ошибка.");
                Environment.Exit(1);
            }
        }

        private AssemblyAnalyzer assemblyAnalyzer; //анализатор сборок
        private ApprenticeServerDocument activeAssembly;
        private FileDialogService dsOpenFile = new FileDialogService();
        private string filePath;
        private DataContext db;
        IEnumerable<Part> parts;

        private ObservableCollection<PartViewModel> DGParts { get; set; } = new ObservableCollection<PartViewModel>();

        private ObservableCollection<PartViewModel> filteredDGParts = new ObservableCollection<PartViewModel>();
        public ObservableCollection<PartViewModel> FilteredDGParts
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

        private PartViewModel selectedDGPart;
        public PartViewModel SelectedDGPart
        {
            get { return selectedDGPart; }
            set
            {
                selectedDGPart = value;
                OnPropertyChanged("SelectedDGPart");
                ToBitmapSource(selectedDGPart.PartDoc.Thumbnail);
                //CurPartImage = d.Result;
                //CurPartImage = await Emfutilities.ToBitmapSource(selectedDGPart.PartDoc.Thumbnail);
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
                    FilteredDGParts = new ObservableCollection<PartViewModel>(DGParts.Where(x => x.Name.ToLower().Contains(aA_SearchText.ToLower())));
                //if (FilteredDGParts.Count != DGParts.Count)
                    //DGProperties = null;
            }
        }

        private BitmapSource curPartImage =null;
        public BitmapSource CurPartImage 
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
        private SimpleCommand cmdOpenAssembly;
        public SimpleCommand CmdOpenAssembly
        {
            get
            {
                return cmdOpenAssembly ??
                    (cmdOpenAssembly = new SimpleCommand(obj =>
                    {
                        if (dsOpenFile.OpenFileDialog("Assembly files|*.iam"))
                        {
                            try
                            {
                                PartViewModel temp = SelectedDGPart;
                                filePath = dsOpenFile.FilePath;
                                assemblyAnalyzer.OpenAssembly(filePath);
                                DGParts.Clear();
                                foreach(ApprenticeServerDocument part in assemblyAnalyzer.Parts)
                                {
                                    DGParts.Add(new PartViewModel(part, AssemblyAnalyzer.getPartProperties(part), false));
                                }
                                PartViewModel tt = new PartViewModel() ;
                                
                                tt.Name = "dddddddddddddddddddddddddddddddddddddddd";
                                tt.IsSaved = false;
                                DGParts.Add(tt);
                                FilteredDGParts = DGParts;

                                if(aA_SearchText!="" && aA_SearchText!=null)
                                    FilteredDGParts = new ObservableCollection<PartViewModel>(DGParts.Where(x => x.Name.ToLower().Contains(aA_SearchText.ToLower())));
                                DGProperties = null;
                            }
                            catch (Exception ex)
                            {
                                FileDialogService.ShowMessage(ex.Message);
                            }
                        }
                    })
                    );
            }
        }

        //команда "сохранить деталь"
        private OpenDialogWindowCommand cmdSavePart;
        public OpenDialogWindowCommand CmdSavePart
        {
            get
            {
                return cmdSavePart ??
                    (cmdSavePart = new OpenDialogWindowCommand(this,
                    (obj) =>
                    {
                        try
                        {
                            SavePartViewModel savePartVM = obj as SavePartViewModel;
                            if (savePartVM != null)
                            {
                                AA_SearchText = savePartVM.TextValue;
                            }
                            else throw new Exception("SavePartViewModel is null");
                        }
                        catch (Exception ex)
                        {
                            FileDialogService.ShowMessage(ex.Message);
                        }
                    },
                    (obj) => {
                        PartViewModel temp = obj as PartViewModel;
                        return temp != null && temp.IsSaved == false;
                    }
                    ));
            }
        }
   
        private   void ToBitmapSource(stdole.IPictureDisp pictureDisp)
        {
                Metafile metaFile = null;
                if (pictureDisp.Type == 2)
                {
                    //Конвертация метафайла
                    IntPtr metafileHandle = new IntPtr(pictureDisp.Handle);
                    metaFile = new Metafile(metafileHandle, new WmfPlaceableFileHeader());
                }
                using (System.Drawing.Imaging.Metafile emf = metaFile)
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(emf.Width, emf.Height))
                {
                    bmp.SetResolution(emf.HorizontalResolution, emf.VerticalResolution);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        g.DrawImage(emf, 0, 0);
                        CurPartImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                }
        }
    }
}
