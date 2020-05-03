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
using System.IO;
using System.Data.Entity.Infrastructure;

namespace assemblyAnalyze
{
    public class AnalyzerViewModel : BasicViewModel,INotifyPropertyChanged 
    {
        public AnalyzerViewModel()
        {
            //при старте открытая вкладка это "База деталей"
            openedTabItemAssembly = false;
            openedTabItemDB = true;
            
            assemblyAnalyzer = new AssemblyAnalyzer();
            assemblyParts = new ObservableCollection<PartViewModel>();
            try
            {
                db = new DataContext();
                db.Parts.Load();
                db.PartFeatures.Load();
                db.Part_partfeatures.Load();
                //parts = db.Parts.Local.ToBindingList();
            }
            catch (Exception ex)
            {
                FileDialogService.ShowMessage(ex.Message+ ex.InnerException.Message+ex.InnerException.InnerException.Message);//"При попытке подключиться к БД возникла ошибка.");
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

        private bool openedTabItemAssembly ;
        public bool OpenedTabItemAssembly
        {
            get { return openedTabItemAssembly; }
            set
            {
                openedTabItemAssembly = value;
                OnPropertyChanged("OpenedTabItemAssembly");
            }
        }

        private bool openedTabItemDB = true;
        public bool  OpenedTabItemDB
        {
            get { return openedTabItemDB; }
            set
            {
                openedTabItemDB = value;
                OnPropertyChanged("OpenedTabItemDB");
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
                    updateAssemblyPartPhoto(selectedAssemblyPart.PartDocument.Thumbnail);
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
                        if(!OpenedTabItemAssembly)
                            OpenedTabItemAssembly = true; //открываем TabItem для анализа сборки
                        if (dsOpenFile.OpenFileDialog("Assembly files|*.iam"))
                        {
                            PartViewModel temp = SelectedAssemblyPart;
                            filePath = dsOpenFile.FilePath;
                            assemblyAnalyzer.OpenAssembly(filePath);
                            assemblyParts.Clear();
                            foreach(ApprenticeServerDocument part in assemblyAnalyzer.Parts)
                            {
                                assemblyParts.Add(new PartViewModel(part));
                            }
                            FilteredAssemblyParts = assemblyParts;
                            if(assemblyPartSearchText!="" && assemblyPartSearchText!=null)
                                FilteredAssemblyParts = new ObservableCollection<PartViewModel>(assemblyParts.Where(x => x.Name.ToLower().Contains(assemblyPartSearchText.ToLower())));
                            AssemblyPartProps = null;
                        }
                    })
                    );
            }
        }

        //команда "сохранить деталь"
        private OpenDialogWindowCommand cmdSaveAssemblyPart;
        public OpenDialogWindowCommand CmdSaveAssemblyPart
        {
            get
            {
                return cmdSaveAssemblyPart ??
                    (cmdSaveAssemblyPart = new OpenDialogWindowCommand(this,
                    (obj) =>
                    {
                        SavePartViewModel savePartVM = obj as SavePartViewModel;
                        if (savePartVM != null)
                        {
                            if (savePartVM.IsSaved)
                            {
                                String partDesctiprion = savePartVM.PartDescription;
                                saveAssemblyPartInDB(selectedAssemblyPart, partDesctiprion);
                            }
                        }
                        else throw new Exception("Внутренняя ошибка при передаче данных между окнами.");
                    },
                    (obj) => {
                        PartViewModel temp = obj as PartViewModel;
                        return temp?.IsSaved == false;
                    }
                    ));
            }
        }
        
        private byte[] BitmapSource_2_ByteArray(BitmapSource bitmap)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.QualityLevel = 100;
            byte[] bit;
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                bit = stream.ToArray();
                stream.Close();
            }
            return bit;
        }

        private BitmapSource IPictureDisp_2_BitmapSource(stdole.IPictureDisp pictureDisp)
        {
            BitmapSource bitmap = null;
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
                    return bitmap;
                }
            }
        }

        private async void updateAssemblyPartPhoto(stdole.IPictureDisp pictureDisp)
        {
            await Task.Run(()=>
            {
                BitmapSource bitmap = IPictureDisp_2_BitmapSource(pictureDisp);
                bitmap.Freeze();
                CurAssemblyPartImage = bitmap;
            });
        }

        private async void saveAssemblyPartInDB(PartViewModel part, string description)
        {
            stdole.IPictureDisp disp = part.PartDocument.Thumbnail;
            part.IsSaved = true;
            await Task.Run(()=>
            {
                try
                {
                    byte[] ar = BitmapSource_2_ByteArray(IPictureDisp_2_BitmapSource(disp));
                    Part newPart = new Part(part.PartDocument.DisplayName,
                                            DateTime.Now.ToString(),
                                            part.PartDocument.InternalName,
                                            part.PartDocument.RevisionId,
                                            part.PartDocument.DatabaseRevisionId,
                                            part.PartDocument.ComponentDefinition.ModelGeometryVersion,
                                            ar,
                                            description);

                    List<Part_PartFeature> part_PartFeatures = new List<Part_PartFeature>();
                    for(int i =0; i < part.Properties.Count; i++)
                    {
                        string key = part.Properties.ElementAt(i).Key;
                        assemblyAnalyzer.models.PartFeature partFeature = db.PartFeatures.FirstOrDefault(x => x.Name == key);
                        if (partFeature == null)
                        {
                            partFeature = db.PartFeatures.Add(new assemblyAnalyzer.models.PartFeature(part.Properties.ElementAt(i).Key));
                        }
                        Part_PartFeature part_PartFeature = new Part_PartFeature();
                        part_PartFeature.PartFeature = partFeature;
                        part_PartFeature.Part = newPart;
                        part_PartFeature.FeatureValue = part.Properties.ElementAt(i).Value;
                        part_PartFeatures.Add( part_PartFeature);
                    }
                    db.Parts.Add(newPart);
                    db.Part_partfeatures.AddRange(part_PartFeatures);
                    db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Part findedPart = db.Parts.FirstOrDefault(p => p.InternalName == part.PartDocument.InternalName && p.RevisionId == part.PartDocument.RevisionId);
                    if (findedPart != null)
                        FileDialogService.ShowMessage($"Данная деталь уже сохранена под именем {findedPart.Name}, с описанием\n{findedPart.Description}.");
                    else
                        FileDialogService.ShowMessage($"Ошибка при сохранении детали. Деталь не сохранена.");
                    part.IsSaved = false;
                }
                catch (Exception ex)
                {
                    FileDialogService.ShowMessage($"Неизвестная ошибка при сохранении в детали. Деталь не сохранена.");
                    part.IsSaved = false;
                    //throw new Exception($"{ex.Message+"\n"}Ошибка при сохранении в БД.");
                }
            });
        }
    }
}
