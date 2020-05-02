using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace assemblyAnalyze.Services
{
    class FileDialogService
    {
        private string filePath;
        public string FilePath { get { return filePath; }}
        public bool OpenFileDialog(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }
        public static void ShowMessage(string message)
        {
            MessageBox.Show(message+"\nПрограмма аварийно завершается.");
            Environment.Exit(1);
        }
    }
}
