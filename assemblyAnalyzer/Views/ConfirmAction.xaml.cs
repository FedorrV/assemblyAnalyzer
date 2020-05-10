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
using System.Windows.Shapes;

namespace assemblyAnalyze
{
    /// <summary>
    /// Логика взаимодействия для Confirm.xaml
    /// </summary>
    public partial class ConfirmAction : Window
    {
        public ConfirmAction()
        {
            InitializeComponent();
            ConfirmBTN.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
