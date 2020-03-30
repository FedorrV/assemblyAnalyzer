using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Inventor;

namespace assemblyAnalyze
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Inventor.Application invApp;
        private static Inventor.Document curDocument;
        private static Inventor.ContentCenter contentCenter;
        public MainWindow()
        {
            InitializeComponent();
            ApplicationViewModel applicationViewModel = new ApplicationViewModel();
            this.DataContext = applicationViewModel;
        }

    }
}
