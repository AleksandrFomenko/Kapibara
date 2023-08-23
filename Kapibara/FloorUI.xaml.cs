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
using System.Linq;




namespace Kapibara
{

    public partial class FloorUI : Window
    {
        public FloorUI(Document doc)
        {
            Doc = doc;
            InitializeComponent();
        }
        Document Doc;
        private bool activeViewLevel;
        private bool activeViewElem;
        private int value_familyInstancse;
        private FilteredElementCollector collectorView;
        private FilteredElementCollector collectorElements;
        
        

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
            using (Transaction t = new Transaction(Doc, "floor"))
            {
                t.Start();
             //  ExecuteTransactionFloor();
                List<Element> elem_a = new FilteredElementCollector(Doc)
              .OfClass(typeof(FamilyInstance))
              .WhereElementIsNotElementType()
              .ToList();
               
                Autodesk.Revit.UI.TaskDialog.Show("Succeeded", string.Format("Количество: {0}", elem_a.Count));
                t.Commit();
            }
            
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
        static int GetFloorNumberFromInstance(Document doc, FamilyInstance familyInstance, List<Level> levels)
        {
            double instanceElevation = (familyInstance.Location as LocationPoint).Point.Z;
            

            int left = 0;
            int right = levels.Count - 1;
            int floorNumber = 0;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                Level midLevel = levels[mid];

                if (midLevel.Elevation > instanceElevation)
                {
                    floorNumber = midLevel.Elevation < 0 ? (int)midLevel.Elevation : (int)midLevel.Elevation + 1;
                    right = mid - 1;
                }
                else
                {
                    floorNumber = midLevel.Elevation < 0 ? (int)Math.Floor(midLevel.Elevation) : (int)Math.Ceiling(midLevel.Elevation);
                    left = mid + 1;
                }
            }

            return floorNumber;
        }
        private void ExecuteTransactionFloor()
        {
            List<Element> elem_a = new FilteredElementCollector(Doc)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .ToList();
            if (activeViewLevel)
            {
                collectorView = new FilteredElementCollector(Doc);
            } else
            {
                collectorView= new FilteredElementCollector(Doc,Doc.ActiveView.Id);
            }

            List<Element> levels = collectorView
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .ToList();
            
            List<Element> sortedLevels = levels.OrderBy(level => ((Level)level).Elevation).ToList();
           // GetFloorNumberFromInstance(Doc, FamilyInstance familyInstance, levels);

        }
        
    }

}
