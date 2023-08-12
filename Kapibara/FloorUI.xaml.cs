using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;



namespace Kapibara
{

    public partial class FloorUI : Window
    {
        public FloorUI(Document Doc)
        {
            Doc = doc;
            InitializeComponent();
        }
        Document doc;
        private bool activeViewLevel;
        public bool activeViewElem;

        private string FloorParam = "ADSK_Этаж";

        private static List<BuiltInCategory> cats_iso = new List<BuiltInCategory> {
            BuiltInCategory.OST_DuctInsulations,
            BuiltInCategory.OST_DuctLinings,
            BuiltInCategory.OST_PipeInsulations
        };

        List<BuiltInCategory> cats_dct_pipe = new List<BuiltInCategory> {
        BuiltInCategory.OST_DuctCurves,
        BuiltInCategory.OST_FlexDuctCurves,
        BuiltInCategory.OST_PipeCurves
        };
        ElementMulticategoryFilter emf = new ElementMulticategoryFilter(cats_iso);


        

        private void parametersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (Transaction t = new Transaction(doc, "floor"))
            {
                t.Start();
                ExecuteTransactionFloor();
                t.Commit();
            }
            Autodesk.Revit.UI.TaskDialog.Show("Succeeded", "Успешно");
        }

        private void CheckBox_ActiveView_levels(object sender, RoutedEventArgs e)
        {
            activeViewLevel = true;

        }
        private void CheckBox_ActiveView_Levels_Unchecked(object sender, RoutedEventArgs e)
        {
            activeViewLevel = false;

        }
        private void CheckBox_ActiveView_elems(object sender, RoutedEventArgs e)
        {
            activeViewElem = true;

        }
        private void CheckBox_ActiveView_elems_Unchecked(object sender, RoutedEventArgs e)
        {
            activeViewElem = false;
        }
        private void ExecuteTransactionFloor()
        {


        }
    }

}
