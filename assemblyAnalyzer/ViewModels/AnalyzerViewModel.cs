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

        private ObservableCollection<PartViewModel> assemblyParts { get; set; } = new ObservableCollection<PartViewModel>();

        private ObservableCollection<PartViewModel> filteredAssemblyParts = new ObservableCollection<PartViewModel>();
        public ObservableCollection<PartViewModel> FilteredAssemblyParts
        {
            get
            {
                return filteredAssemblyParts;
            }
            set
            {
                filteredAssemblyParts = value;
                OnPropertyChanged("FilteredAssemblyParts");
            }
        }

        private PartViewModel selectedAssemblyPart;
        public PartViewModel SelectedAssemblyPart
        {
            get { return selectedAssemblyPart; }
            set
            {
                selectedAssemblyPart = value;
                OnPropertyChanged("SelectedAssemblyPart");
                if (selectedAssemblyPart == null)
                {
                    AssemblyPartProps = null;
                    CurAssemblyPartImage = null;
                }
                else
                {
                    updatePartPhoto(selectedAssemblyPart.PartDocument.Thumbnail);
                    if (selectedAssemblyPart.Properties == null)
                        selectedAssemblyPart.Properties = AssemblyAnalyzer.getPartProperties(selectedAssemblyPart.PartDocument);
                    AssemblyPartProps = selectedAssemblyPart.Properties;
                }
            }
        }

        private Dictionary<string, string> assemblyPartProps = new Dictionary<string, string>();
        public Dictionary<string, string> AssemblyPartProps
        {
            get { return assemblyPartProps; }
            set
            {
                assemblyPartProps = value;
                OnPropertyChanged("AssemblyPartProps");
            }
        }

        private string assemblyPartSearchText;
        public string AssemblyPartSearchText
        {
            get
            {
                return assemblyPartSearchText;
            }
            set
            {
                assemblyPartSearchText = value;
                OnPropertyChanged("AssemblyPartSearchText");
                if (assemblyPartSearchText == "")
                    FilteredAssemblyParts = assemblyParts;
                else
                    FilteredAssemblyParts = new ObservableCollection<PartViewModel>(assemblyParts.Where(x => x.Name.ToLower().Contains(assemblyPartSearchText.ToLower())));
                //if (FilteredDGParts.Count != DGParts.Count)
                    //DGProperties = null;
            }
        }

        private BitmapSource curAssemblyPartImage;
        public BitmapSource CurAssemblyPartImage 
        {
            get
            {
                return curAssemblyPartImage;
            }
            set
            {
                curAssemblyPartImage = value;
                OnPropertyChanged("CurAssemblyPartImage");
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
                                PartViewModel temp = SelectedAssemblyPart;
                                filePath = dsOpenFile.FilePath;
                                assemblyAnalyzer.OpenAssembly(filePath);
                                assemblyParts.Clear();
                                foreach(ApprenticeServerDocument part in assemblyAnalyzer.Parts)
                                {
                                    assemblyParts.Add(new PartViewModel(part,  false));
                                }
                                
                                FilteredAssemblyParts = assemblyParts;
                                if(assemblyPartSearchText!="" && assemblyPartSearchText!=null)
                                    FilteredAssemblyParts = new ObservableCollection<PartViewModel>(assemblyParts.Where(x => x.Name.ToLower().Contains(assemblyPartSearchText.ToLower())));
                                AssemblyPartProps = null;
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
                                AssemblyPartSearchText = savePartVM.TextValue;
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
   
        private async void updatePartPhoto(stdole.IPictureDisp pictureDisp)
        {
            BitmapSource bitmap = null;
            await Task.Run(()=>
                {
                Metafile metaFile = null;
                if (pictureDisp.Type == 2)
                {
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
                        bitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        bitmap.Freeze();
                        CurAssemblyPartImage = bitmap;
                    }
                }
            });
        }
    }
}
