using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assemblyAnalyze
{
    class assemblyAnalyzer
    {
        public assemblyAnalyzer(string pathToFile)
        {
            this.pathToFile = pathToFile;
        }

        private AssemblyDocument assembly;
        private string pathToFile;
        public string PathToFile
        {
            get
            {
                return pathToFile;
            }
        }
        List <string> componentsName = new List<string>();
        List<PartDocument> partsDocs = new List<PartDocument>();
        Dictionary<string, string> partProperties = new Dictionary<string, string>();
        Dictionary<string, string> partProperties1 = new Dictionary<string, string>();
        Dictionary<string, string> partProperties2 = new Dictionary<string, string>();
        Dictionary<string, string> partProperties3 = new Dictionary<string, string>();

        public void Initiolize(Inventor.Application app)
        {
            try
            {
                assembly = (AssemblyDocument)app.Documents.Open(PathToFile, false);
            }
            catch(Exception ex)
            {
                throw new Exception("Ошибка при попытке открытия файла, возможно файл повреждён или имеет не правильную структуру.");
            }
        }

        public void getAllParts()
        {
            getAllDefinitionParts(assembly);
            int ass = assembly.ComponentDefinitions.Count;
        }

        private void getAllDefinitionParts(AssemblyDocument assemblyDoc)
        {
            int cntRef = assemblyDoc.AllReferencedDocuments.Count;
            foreach (Document curDoc in assemblyDoc.AllReferencedDocuments)
            {
                string val = curDoc.DisplayName;
                if (curDoc.DocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
                {
                    getAllDefinitionParts((AssemblyDocument)curDoc);
                }
                else if (curDoc.DocumentType == DocumentTypeEnum.kPartDocumentObject)
                {
                    PartDocument temp = partsDocs.Find(x => x.InternalName == curDoc.InternalName && x.RevisionId == curDoc.RevisionId);
                    //PartDocument temp = partsDocs.Find(x => x == curDoc);
                    if (temp == null)
                    {
                        partsDocs.Add((PartDocument)curDoc);
                        componentsName.Add(curDoc.DisplayName);
                    }
                }
            }
        }

        public void getAllProperties()
        {
            //ReferenceParameters modelParameters = partsDocs[2].ComponentDefinition.Parameters.ReferenceParameters;
            //Dictionary<string, string> dict= new Dictionary<string, string>();
            //int cnt = modelParameters.Count;
            //foreach(ReferenceParameter mp in modelParameters)
            //{
            //    var obj1 = mp.Name;
            //    var obj2 = mp.ModelValue;
            //    var obj3 = mp.Value;
            //    var obj4 = mp._Value;
            //}
            int []propIds = getDesignTrackingProperties();
           
            foreach (int i in Enum.GetValues(typeof(PropertiesForDesignTrackingPropertiesEnum)))
            {
                Property tempProp = partsDocs[0].PropertySets["Design Tracking Properties"].ItemByPropId[i];
                if (tempProp.Value != null && tempProp.Value.ToString() != "")
                    partProperties.Add(tempProp.DisplayName + i.ToString(), tempProp.Value.ToString());
            }
            Asset tempAsset = partsDocs[1].ActiveMaterial;
            string val = tempAsset.Name;
            val = tempAsset.Name;
            val = tempAsset.CategoryName; 
            foreach(AssetValue av in tempAsset)
            {
                val = av.Name;
                val = av.DisplayName;
            }
            //BOM BOMInfo = assembly.ComponentDefinition.BOM;

            //int cnnt = BOMInfo.BOMViews.Count;
            //foreach (BOMView bv in BOMInfo.BOMViews)
            //{
            //    int cntRows = bv.BOMRows.Count;

            //    foreach (BOMRow br in bv.BOMRows)
            //    {

            //        int cntComp = br.ComponentDefinitions.Count;
            //        foreach (ComponentDefinition cd in br.ComponentDefinitions)
            //        {
            //            var ss = cd.Document.PropertySets.Count;
            //            partProperties.Clear();
            //            partProperties1.Clear();
            //            partProperties2.Clear();
            //            partProperties3.Clear();
            //            foreach (PropertySet ps in cd.Document.PropertySets)
            //            {
            //                if(ps.DisplayName == "Inventor Document Summary Information")
            //                {
            //                    foreach(int i in Enum.GetValues(typeof(PropertiesForDocSummaryInformationEnum))){
            //                        Property tempProp = ps.ItemByPropId[i];
            //                        if (tempProp.Value != null && tempProp.Value.ToString() != "")
            //                            partProperties.Add(tempProp.DisplayName, tempProp.Value.ToString());
            //                    }
            //                }
            //                else if(ps.Name == "Design Tracking Properties")
            //                {
            //                    foreach (int i in Enum.GetValues(typeof(PropertiesForDesignTrackingPropertiesEnum))){
            //                        Property tempProp = ps.ItemByPropId[i];
            //                        if (tempProp.Value != null && tempProp.Value.ToString() != "")
            //                            partProperties.Add(tempProp.DisplayName+i.ToString(), tempProp.Value.ToString());
            //                    }
            //                }
            //                else if (ps.Name == "Inventor Summary Information")
            //                {
            //                    foreach (int i in Enum.GetValues(typeof(PropertiesForSummaryInformationEnum))){
            //                        if (i == 12 || i==17)
            //                            continue;
            //                        Property tempProp = ps.ItemByPropId[i];
            //                        var value = tempProp.Value;
            //                        if (tempProp.Value != null && tempProp.Value.ToString() != "")
            //                            partProperties.Add(tempProp.DisplayName, tempProp.Value.ToString());
            //                    }
            //                }
            //                else if (ps.Name == "Inventor User Defined Properties" && ps.Count != 0)
            //                {
            //                    foreach (int i in Enum.GetValues(typeof(PropertiesForUserDefinedPropertiesEnum))){
            //                        Property tempProp = ps.ItemByPropId[i];
            //                        if (tempProp.Value != null && tempProp.Value.ToString() != "")
            //                            partProperties.Add(tempProp.DisplayName, tempProp.Value.ToString());
            //                    }
            //                }




            //            }

            //        }
            //        int adas = 0;
            //    }
            //}
        }

            private  int [] getDesignTrackingProperties()
        {
            int[] propIds = {
                4,
                5,
                7,
                9,
                10,
                11,
                12,
                13,
                17,
                20,
                21,
                23,
                28,
                29,
                30,
                31,
                32,
                33,
                34,
                35,
                36,
                37,
                40,
                41,
                42,
                43,
                44,
                45,
                46,
                47,
                48,
                49,
                50,
                51,
                55,
                56,
                57,
                58,
                59,
                60,
                61,
                62,
                63,
                64,
                65,
                66,
                67,
                71,
                72,
                73
            };
            return propIds;
        }
    }
}


