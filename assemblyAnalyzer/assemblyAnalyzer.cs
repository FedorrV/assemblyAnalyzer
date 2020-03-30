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
        List <string> components = new List<string>();

        public void Initiolize(Inventor.Application app)
        {
            assembly = app.Documents.Open(PathToFile, false) as AssemblyDocument;
            //assembly = app.Documents.Add(DocumentTypeEnum.kAssemblyDocumentObject) as AssemblyDocument;
            foreach (ComponentOccurrence  ss in assembly.ComponentDefinition.Occurrences)
            {
                components.Add(ss.Name);
                //ss.AttributeSets.DataIO.WriteDataToFile("XML", "file.xml");
                int cnt = ss.AttributeSets.Count;
                int sss = 5;
            } 
        }
    }
}
