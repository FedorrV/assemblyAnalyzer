using InventorApprentice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace assemblyAnalyze
{
    class AssemblyAnalyzer
    {
        public AssemblyAnalyzer()
        {
            startupApprenticeServer();
        }

        private ApprenticeServerComponent aprServer; //экзепляр Apprentice Server
        private ApprenticeServerDocument assembly; //сборка, с которой работает  AS
        public ApprenticeServerDocument ActiveAssembly
        {
            get
            {
                return assembly;
            }
        }

        //private List<string> partsName = new List<string>();
        //public List<string> PartsName
        //{
        //    get
        //    {
        //        return partsName;
        //    }
        //}

        private List<ApprenticeServerDocument> parts = new List<ApprenticeServerDocument>();
        public IReadOnlyList<ApprenticeServerDocument>  Parts
        {
            get
            {
                return parts.AsReadOnly();
            }
        }

        //private List<Dictionary<string, string>> partProperties = new List<Dictionary<string, string>>();
        //public Dictionary<string, string> PartProperties(ApprenticeServerDocument part)
        //{
        //    int index = parts.FindIndex(x => x == part);
        //    return partProperties[index];
        //}

        private void startupApprenticeServer()
        {
            aprServer = new ApprenticeServerComponent();
            if (aprServer == null)
            {
                new Exception("Ошибка при подключении к Inventor ApprenticeServer");
            }
        }

        public void OpenAssembly(string pathToFile)
        {
            parts.Clear();
            try
            {
                assembly = aprServer.Open(pathToFile);
                if(assembly.DocumentType != DocumentTypeEnum.kAssemblyDocumentObject)
                    throw new Exception("Данный файл не является файлом сборки Inventor.");
            }
            catch(Exception ex)
            {
                throw new Exception("Ошибка при попытке открытия файла, возможно файл имеет правильную структуру или создан в более поздней версии.");
            }
            getParts();
        }


        private void getParts()
        {
            getAllDefinitionParts(assembly);
        }

        private void getAllDefinitionParts(ApprenticeServerDocument assemblyDoc)
        {
            foreach (ApprenticeServerDocument curDoc in assemblyDoc.AllReferencedDocuments)
            {
                string val = curDoc.DisplayName;
                if (curDoc.DocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
                {
                    getAllDefinitionParts(curDoc);
                }
                else if (curDoc.DocumentType == DocumentTypeEnum.kPartDocumentObject
                         && curDoc.Type != ObjectTypeEnum.kVirtualComponentDefinitionObject)
                {
                    /*нет ли в сборке документа с такими же ссылками и геометрией(копии этой детали)*/
                    ApprenticeServerDocument copyDoc = parts.Find(x => x.InternalName == curDoc.InternalName 
                                                                && x.DatabaseRevisionId == curDoc.DatabaseRevisionId 
                                                                && x.ComponentDefinition.ModelGeometryVersion == curDoc.ComponentDefinition.ModelGeometryVersion
                    );
                    
                    if (copyDoc == null)// если нет такого документа, тогда добавляем
                    {
                        parts.Add(curDoc);
                        //partsName.Add(curDoc.DisplayName);
                    }
                }
            }
        }

        private void deleteProxyParts(List<ApprenticeServerDocument> parts)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                foreach (DocumentDescriptor dd in parts[i].ReferencedDocumentDescriptors)
                {
                    ApprenticeServerDocument tempDoc = dd.ReferencedDocument as ApprenticeServerDocument;
                    tempDoc = parts.Find(x => x.InternalName == tempDoc.InternalName);
                    if (tempDoc != null)
                    {
                        parts.RemoveAt(i);
                        //partsName.RemoveAt(i);
                    }
                }
            }
        }

        public static Dictionary<string,string> getPartProperties(ApprenticeServerDocument part)
        {
            IPictureDisp pict = part.Thumbnail as IPictureDisp;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            int[] designPropsIds = getDesignTrackingProperties();
            Property tempProp;
            foreach (int propId in Enum.GetValues(typeof(PropertiesForDocSummaryInformationEnum)))
            {
                try
                {
                    tempProp = part.PropertySets["Inventor Document Summary Information"].ItemByPropId[propId];
                    if (tempProp.Value != null && tempProp.Value.ToString() != "")
                        dict.Add(tempProp.DisplayName, tempProp.Value.ToString());
                }
                catch { }
            }
            for (int propId = 0; propId < designPropsIds.Length; propId++)
            {
                try
                {
                    tempProp = part.PropertySets["Design Tracking Properties"].ItemByPropId[designPropsIds[propId]];
                    if (tempProp.Value != null && tempProp.Value.ToString() != "")
                        dict.Add(tempProp.DisplayName, tempProp.Value.ToString());
                }
                catch { }
            }
            return dict;
        }

        private static int [] getDesignTrackingProperties()
        {
            int[] propIds = {
                4,5,20,29,32,37,36,48,58,59,60,61,67,72 // основные нужные свойства
            };
            return propIds;
        }
    }
}


