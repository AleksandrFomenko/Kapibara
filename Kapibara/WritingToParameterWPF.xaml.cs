using Autodesk.Revit.DB;
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

namespace Kapibara
{
    
    public partial class WritingToParameterWPF : Window
    {
        public WritingToParameterWPF(Document doc)
        {
            InitializeComponent();
            Doc = doc;
        }
        Document Doc;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string enteredText = textBox.Text;

        }
        private void Parameters_Selection(object sender, SelectionChangedEventArgs e)
        {

        }
        public void ExecuteTransactionSystemName()
        {




        }
    }
}
