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
using Binding = Autodesk.Revit.DB.Binding;

namespace Kapibara
{
   
    public partial class NumerarionWPF : Window
    {
        public NumerarionWPF(Document doc)
        {
            InitializeComponent();
            Doc = doc;
        }
        Document Doc;
    }
}
