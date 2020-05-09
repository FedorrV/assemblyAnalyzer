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
using Application = System.Windows.Application;

namespace assemblyAnalyze
{
    public class AnalyzerViewModel : BasicViewModel,INotifyPropertyChanged 
    {
        public AnalyzerViewModel()
        {
            //при старте открытая вкладка это "База деталей"
            openedTabItemAssembly = false;
            openedTabItemDB = true;
            
            assemblyParts = new ObservableCollection<PartViewModel>();

            try
            {
                assemblyAnalyzer = new AssemblyAnalyzer();
                db = new DataContext();
                loadDBParts();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}\nПрограмма аварийно закрывается.");
                Environment.Exit(1);
            }
        }

        private AssemblyAnalyzer assemblyAnalyzer; //анализатор сборок
        ///private ApprenticeServerDocument activeAssembly;
        private FileDialogService dsOpenFile = new FileDialogService();
        private DataContext db;

        private bool openedTabItemAssembly;
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
        public bool OpenedTabItemDB
        {
            get { return openedTabItemDB; }
            set
            {
                openedTabItemDB = value;
                OnPropertyChanged("OpenedTabItemDB");
            }
        }

        /*---------------*/
        /*ПЕРЕМЕННЫЕ ДЛЯ TabItem "БАЗА ДЕТАЛЕЙ"*/
        public ICollection<Part> DBParts;

        private ObservableCollection<Part> filteredDBParts;
        public ObservableCollection<Part> FilteredDBParts
        {
            get
            {
                return filteredDBParts;
            }
            set
            {
                filteredDBParts = value;
                OnPropertyChanged("FilteredDBParts");
            }
        }

        private Part selectedDBPart;
        public Part SelectedDBPart
        {
            get { return selectedDBPart; }
            set
            {
                selectedDBPart = value;
                OnPropertyChanged("SelectedDBPart");
                if (selectedDBPart == null)
                {
                    DBPartProperties = null;
                    DBPartDescription = "";
                    DBPartImage = null;
                }
                else
                {
                    DBPartDescription = selectedDBPart.Description;
                    refreshDBPartInfo(selectedDBPart);
                }
            }
        }

        private Dictionary<string, string> dBPartProperties = new Dictionary<string, string>();
        public Dictionary<string, string> DBPartProperties
        {
            get { return dBPartProperties; }
            set
            {
                dBPartProperties = value;
                OnPropertyChanged("DBPartProperties");
            }
        }

        private string dBPartDescription;
        public string DBPartDescription
        {
            get
            {
                return dBPartDescription;
            }
            set
            {
                dBPartDescription = value;
                OnPropertyChanged("DBPartDescription");
            }
        }

        private BitmapImage dBPartImage;
        public BitmapImage DBPartImage
        {
            get { return dBPartImage; }
            set
            {
                dBPartImage = value;
                OnPropertyChanged("DBPartImage");
            }
        }

        private string dBPartSearchText;
        public string DBPartSearchText
        {
            get
            {
                return dBPartSearchText;
            }
            set
            {
                dBPartSearchText = value;
                OnPropertyChanged("DBPartSearchText");
                if (dBPartSearchText == "")
                    FilteredDBParts = new ObservableCollection<Part>(DBParts);
                else
                    FilteredDBParts = new ObservableCollection<Part>(DBParts.Where(x => x.Name.Contains(dBPartSearchText)));
            }
        }

        //команда "обновить список деталей"
        private SimpleCommand cmdRefreshDBParts;
        public SimpleCommand СmdRefreshDBParts
        {
            get
            {
                return cmdRefreshDBParts ??
                    (cmdRefreshDBParts = new SimpleCommand(obj =>refreshDBParts()));
            }
        }

        //команда "удалить деталь из БД"
        private OpenDialogWindowCommand<ConfirmActionViewModel> сmdDeleteDBPart;
        public OpenDialogWindowCommand<ConfirmActionViewModel> CmdDeleteDBPart
        {
            get
            {
                return сmdDeleteDBPart ??
                    (сmdDeleteDBPart = new OpenDialogWindowCommand<ConfirmActionViewModel>(
                    (obj) =>
                    {
                        ConfirmActionViewModel savePartVM = obj as ConfirmActionViewModel;
                        if (savePartVM != null)
                        {
                            if (savePartVM.IsConfirmed)
                            {
                                removeDBPart(selectedDBPart);
                            }
                        }
                        else throw new Exception("Внутренняя ошибка при передаче данных между окнами.");
                    }, 
                    (obj) => obj != null,
                    "Вы действительно хотите удалить данную деталь?"));
            }
        }

        private async void removeDBPart(Part selectedPart)
        {
            try
            {
                db.Parts.Remove(selectedPart);
                DBParts.Remove(selectedPart);
                FilteredDBParts.Remove(selectedPart);
                await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show("При попытке удалить деталь возникла ошибка. Деталь не была удалена.");
            }
        }

        private async void loadDBParts()
        {
            //await Task.Run(() =>
            //{
            try
            {
                await db.Parts.LoadAsync();
                //await db.PartFeatures.LoadAsync();
                //await db.Part_partfeatures.LoadAsync();
                DBParts = db.Parts.Local.ToBindingList();
                if (string.IsNullOrEmpty(dBPartSearchText))
                    FilteredDBParts = new ObservableCollection<Part>(DBParts);
                else
                    FilteredDBParts = new ObservableCollection<Part>(DBParts.Where(x => x.Name.ToLower().Contains(dBPartSearchText.ToLower())));
                DBPartProperties = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\nОшибка при подключении к БД. Работа с базой деталей будет не доступна.");//"При попытке подключиться к БД возникла ошибка.");
            }
            //});
        }

        private async void refreshDBParts()
        {
            await db.Parts.LoadAsync();
            DBParts = db.Parts.Local.ToBindingList();
            if (string.IsNullOrEmpty(dBPartSearchText))
                FilteredDBParts = new ObservableCollection<Part>(DBParts);
            else
                FilteredDBParts = new ObservableCollection<Part>(DBParts.Where(x => x.Name.ToLower().Contains(dBPartSearchText.ToLower())));
            DBPartProperties = null;
            DBPartDescription = "";
            DBPartImage = null;
        }

        private async void refreshDBPartInfo(Part currentPart)
        {
            if (selectedDBPart.Part_PartFeatures == null)
                return;
            Dictionary<string, string> properties = new Dictionary<string, string>();
            DBPartImage = await ByteArray_2_BitmapImage(currentPart.Image);
            await Task.Run(() => {
                foreach (Part_PartFeature ppf in currentPart.Part_PartFeatures)
                {
                    try
                    {
                        properties.Add(ppf?.PartFeature?.Name, ppf?.FeatureValue);
                    }
                    catch { }
                }
            });
            DBPartProperties = properties;
        }


        /*==========================================*/
        /*==========================================*/
        /*ПЕРЕМЕННЫЕ ДЛЯ TabItem "АНАЛИЗ СБОРКИ"*/
        private string filePath;

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
                    updateAssemblyPartPhoto(selectedAssemblyPart.Image);
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
                            if (!OpenedTabItemAssembly)
                                OpenedTabItemAssembly = true; //открываем TabItem для анализа сборки
                            assemblyAnalyzer = new AssemblyAnalyzer();
                            filePath = dsOpenFile.FilePath;
                            assemblyAnalyzer.OpenAssembly(filePath);
                            assemblyParts.Clear();
                            foreach(ApprenticeServerDocument part in assemblyAnalyzer.Parts)
                            {
                                assemblyParts.Add(new PartViewModel(part.DisplayName,
                                                                    part.InternalName,
                                                                    part.RevisionId,
                                                                    part.DatabaseRevisionId,
                                                                    part.ComponentDefinition.ModelGeometryVersion,
                                                                    part.Thumbnail,
                                                                    AssemblyAnalyzer.getPartProperties(part)));
                            }
                            FilteredAssemblyParts = assemblyParts;
                            if(!string.IsNullOrEmpty(assemblyPartSearchText))
                                FilteredAssemblyParts = new ObservableCollection<PartViewModel>(assemblyParts.Where(x => x.Name.ToLower().Contains(assemblyPartSearchText.ToLower())));
                            AssemblyPartProps = null;
                            assemblyAnalyzer = null;
                        }
                    })
                );
            }
        }

        //команда "сохранить деталь"
        private OpenDialogWindowCommand<SavePartViewModel> cmdSaveAssemblyPart;
        public OpenDialogWindowCommand<SavePartViewModel> CmdSaveAssemblyPart
        {
            get
            {
                return cmdSaveAssemblyPart ??
                    (cmdSaveAssemblyPart = new OpenDialogWindowCommand<SavePartViewModel>(
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
                        savePartVM = null;
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

        private async Task <BitmapImage> ByteArray_2_BitmapImage(byte[] imageData)
        {
            return await Task.Run( () =>
            {
                if (imageData == null || imageData.Length == 0) return null;
                var image = new BitmapImage();
                using (var mem = new MemoryStream(imageData))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            });
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
            stdole.IPictureDisp disp = part.Image;
            byte[] ar = null;
            Part newPart = null;
            List<Part_PartFeature> part_PartFeatures = new List<Part_PartFeature>();
            ConfirmActionViewModel ViewModel = null;
            List<Part> findedPart = null; 

            try
            {
                findedPart = new List<Part>(db.Parts.Where(p => p.InternalName == part.InternalName)); 
                if (findedPart.Count() == 1)
                {
                    ViewModel = new ConfirmActionViewModel($"В базе деталей уже содержится данная деталь под именем {findedPart[0].Name}. Перезаписать данную деталь?");
                    var displayRootRegistry = (Application.Current as App).displayRootRegistry;
                    await displayRootRegistry.ShowModalPresentation(ViewModel);
                }

                part.IsSaved = true;

                if (ViewModel != null && ViewModel.IsConfirmed)
                {
                    removeDBPart(findedPart[0]);
                }

                await Task.Run(() =>
                {
                    ar = BitmapSource_2_ByteArray(IPictureDisp_2_BitmapSource(disp));
                    newPart = new Part(part.Name,
                                            DateTime.Now.ToString(),
                                            part.InternalName,
                                            part.RevisionId,
                                            part.DatabaseRevisionId,
                                            part.ModelGeometryVersion,
                                            ar,
                                            description);
               
                    for (int i = 0; i < part.Properties.Count; i++)
                    {
                        string key = part.Properties.ElementAt(i).Key;
                        assemblyAnalyzer.models.PartFeature partFeature =   db.PartFeatures.FirstOrDefault(x => x.Name == key);
                        if (partFeature == null)
                        {
                            partFeature = db.PartFeatures.Add(new assemblyAnalyzer.models.PartFeature(part.Properties.ElementAt(i).Key));
                        }
                        Part_PartFeature part_PartFeature = new Part_PartFeature();
                        part_PartFeature.PartFeature = partFeature;
                        part_PartFeature.Part = newPart;
                        part_PartFeature.FeatureValue = part.Properties.ElementAt(i).Value;
                        part_PartFeatures.Add(part_PartFeature);
                    }
                });
                
                db.Parts.Add(newPart);
                db.Part_partfeatures.AddRange(part_PartFeatures);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"{ex.InnerException.InnerException.Message}\nОшибка при сохранении детали. Деталь не сохранена.");
                part.IsSaved = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка при сохранении в детали.");
                part.IsSaved = false;
                //throw new Exception($"{ex.Message+"\n"}Ошибка при сохранении в БД.");
            }

        }
    }
}
