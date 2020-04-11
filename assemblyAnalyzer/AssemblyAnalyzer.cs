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
        public AssemblyAnalyzer(string pathToFile, ApprenticeServerComponent aprServer)
        {
            this.pathToFile = pathToFile;
            OpenAssembly(aprServer);
        }

        private ApprenticeServerDocument assembly;
        private string pathToFile;
        public string PathToFile
        {
            get
            {
                return pathToFile;
            }
        }
       
        List <string> componentsName = new List<string>();
        public List <ApprenticeServerDocument> parts = new List<ApprenticeServerDocument>();
        public List<Dictionary<string, string>> partProperties = new List<Dictionary<string, string>>();
        
        private void OpenAssembly(ApprenticeServerComponent app)
        {
            try
            {
                assembly = app.Open(PathToFile);
                if(assembly.DocumentType != DocumentTypeEnum.kAssemblyDocumentObject)
                    throw new Exception("Данный файл не является файлом сборки Inventor.");
            }
            catch(Exception ex)
            {
                throw new Exception("Ошибка при попытке открытия файла, возможно файл имеет правильную структуру или создан в более поздней версии.");
            }
        }

        public void getAllParts()
        {
            getAllDefinitionParts(assembly);
            getAllPartProperties();
            int a = 0;
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
                        componentsName.Add(curDoc.DisplayName);
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
                        componentsName.RemoveAt(i);
                    }
                }
            }
        }

        public void getAllPartProperties()
        {
            IPictureDisp pict = parts[0].Thumbnail as IPictureDisp;
            int[] designPropsIds = getDesignTrackingProperties();
            Property tempProp;
            for (int i =0; i < parts.Count; i++)
            {
                partProperties.Add(new Dictionary<string, string>());
                foreach (int propId in Enum.GetValues(typeof(PropertiesForDocSummaryInformationEnum)))
                {
                    try
                    {
                        tempProp = parts[i].PropertySets["Inventor Document Summary Information"].ItemByPropId[propId];
                        if (tempProp.Value != null && tempProp.Value.ToString() != "")
                            partProperties[i].Add(tempProp.DisplayName, tempProp.Value.ToString());
                    }
                    catch {}
                }
                for (int propId =0; propId < designPropsIds.Length; propId++)
                {
                    try
                    {
                        tempProp = parts[i].PropertySets["Design Tracking Properties"].ItemByPropId[designPropsIds[propId]];
                        if (tempProp.Value != null && tempProp.Value.ToString() != "")
                            partProperties[i].Add(tempProp.DisplayName, tempProp.Value.ToString());
                    }
                    catch {}
                }
            }
        }

        private  int [] getDesignTrackingProperties()
        {
            int[] propIds = {
                4,5,20,29,32,37,36,48,58,59,60,61,67,72 // основные нужные свойства
            };
            return propIds;
        }
    }
}


