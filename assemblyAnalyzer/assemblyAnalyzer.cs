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
        Dictionary<string, string> partProperties = new Dictionary<string, string>();


        public void Initiolize(Inventor.Application app)
        {
            assembly = (AssemblyDocument)app.Documents.Open(PathToFile, false) ;
            //assembly = app.Documents.Add(DocumentTypeEnum.kAssemblyDocumentObject) as AssemblyDocument;
            
            //foreach (AssemblyComponentDefinition sss in assembly.ComponentDefinitions)
            //{
            //    //components.Add(sss.AttributeSets);
            //    int cntAttr = sss.AttributeSets.Count;
            //    //sss.BOM.BOMViews.
            //    string revision = sss.BOM.RevisionId;
            //    string [] strs = { "asdf", "aasdf" };
            //    bool flag = false;
            //    sss.BOM.GetPartNumberMergeSettings(out flag, out strs);
            //    BOMStructureEnum b =sss.BOMStructure;
            //}
        }

        public void getAllParts()
        {
            getAllDefinition(assembly.ComponentDefinition.Occurrences);
        }

        private void getAllDefinition(ComponentOccurrences occurrences)
        {
            int cntComponents = assembly.ComponentDefinitions.Count;
            int cntRef1 = assembly.ReferencedDocumentDescriptors.Count;
            int cntRef = assembly.AllReferencedDocuments.Count;
            foreach (Document temp in assembly.AllReferencedDocuments)
            {
                PartDocument partDoc = temp as PartDocument;
                string asd = partDoc.FullDocumentName;
                componentsName.Add( temp.DisplayName);
                //string asd = temp.PropertySets["Design Tracking Properties"]["Authority"].Value;
            }

            foreach (ComponentOccurrence co in occurrences)
            {
                //co.Definition;
            }
                //AssemblyComponentDefinition compDef=  co.Parent;
                ////compDef.BOMStructure
                    
                //int cnnt = compDef.BOM.BOMViews.Count;
                //foreach (BOMView bv in compDef.BOM.BOMViews)
                //{
                //    int cntRows = bv.BOMRows.Count; 

                //    foreach(BOMRow br in bv.BOMRows)
                //    {
                //        int cntComp = br.ComponentDefinitions.Count;
                //        foreach (ComponentDefinition cd in br.ComponentDefinitions)
                //        {
                //            var ss = cd.Document.PropertySets.Count;
                //            foreach (PropertySet ps in cd.Document.PropertySets)
                //            {
                //                int cntProp = ps.Count;

                //                //var objj = ps.ItemByPropId[4];
                //                    //var obj = ps.Item("Description").Value;
                //                    //var obj = ps.InternalName;
                //                    //var objs = ps.Name;
                //                    //Property t = ps.ItemByPropId[i];
                                
                //            }
                           
                //        }
                //    }
                //}
                ////AssemblyComponentDefinition acd = co.Definition as AssemblyComponentDefinition;
                //if (co.DefinitionDocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
                //{
                //    getAllDefinition((ComponentOccurrences)co.SubOccurrences );
                //}
                //else if (co.DefinitionDocumentType == DocumentTypeEnum.kPartDocumentObject)
                //{
                //    components.Add(co.Name);
                //}
            //}
        }
    }
}


//foreach (Document temp in assembly.AllReferencedDocuments)
//{
//    int sdf = temp.PropertySets.Count;
//    List<string> DocSummaryValues = new List<string>();
//    List<string> DesignTrackValues = new List<string>();
//    foreach (PropertySet item in temp.PropertySets)
//    {
//        string propName = item.Name;
//        if (propName == "Inventor Document Summary Information")
//        {
//            var obj = item.ItemByPropId[2].Value;
//            DocSummaryValues.Add(item.ItemByPropId[2].Value);
//            DocSummaryValues.Add(item.ItemByPropId[15].Value);
//            DocSummaryValues.Add(item.ItemByPropId[14].Value);
//        }
//        else if (propName == "Design Tracking Properties")
//        {
//            var obj = item.ItemByPropId[72].Value;
//            DesignTrackValues.Add(item.ItemByPropId[72].Value);
//            DesignTrackValues.Add(item.ItemByPropId[43].Value);
//            DesignTrackValues.Add(item.ItemByPropId[23].Value);
//            DesignTrackValues.Add(item.ItemByPropId[56].Value);
//            DesignTrackValues.Add(item.ItemByPropId[10].Value);
//        }
//        else if (propName == "Inventor Summary Information")
//        {
//            string a = item.ItemByPropId[4].Value;
//            string b = item.ItemByPropId[6].Value;
//            string c = item.ItemByPropId[2].Value;
//        }
//    }
//    //ObjectTypeEnum ad = temp.PropertySets[i].Type;
//}

