using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Windows;

namespace Kapibara
{
    public partial class UI : Window
    {
      
        List<BuiltInCategory> cats = new List<BuiltInCategory>();

        List<BuiltInCategory> catsiso = new List<BuiltInCategory>();
      
        private double lengthFromRevit;
        private double lengthCorrect;
        private bool Area = false;
        Guid adskValue = new Guid("8d057bb3-6ccd-4655-9165-55526691fe3a");
        Document Doc;
        private bool ActiveView;

        public UI(Document doc)
        {
            InitializeComponent();
            Doc = doc;
        }

        //Click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteTransaction();
            Autodesk.Revit.UI.TaskDialog.Show("Succeeded", "Успешно");
            Close();
        }

        //Checed
        private void FlexPipe_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (checkBoxName == "FlexPipe" && checkBox.IsChecked == true && !cats.Contains(BuiltInCategory.OST_FlexPipeCurves))
            {
                cats.Add(BuiltInCategory.OST_FlexPipeCurves);
            }
        }

        private void Duct1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (checkBoxName == "Duct1" && checkBox.IsChecked == true && !cats.Contains(BuiltInCategory.OST_DuctCurves))
            {
                cats.Add(BuiltInCategory.OST_DuctCurves);
            }
        }

        private void Lotki_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (checkBoxName == "Lotki" && checkBox.IsChecked == true && !cats.Contains(BuiltInCategory.OST_CableTray))
            {
                cats.Add(BuiltInCategory.OST_CableTray);
            }
        }

        private void FlexDuct_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (checkBoxName == "FlexDuct" && checkBox.IsChecked == true && !cats.Contains(BuiltInCategory.OST_FlexDuctCurves))
            {
                cats.Add(BuiltInCategory.OST_FlexDuctCurves);
            }
        }

        private void Pipe_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (checkBoxName == "Pipe" && checkBox.IsChecked == true && !cats.Contains(BuiltInCategory.OST_PipeCurves))
            {
                cats.Add(BuiltInCategory.OST_PipeCurves);
            }
        }

       
        private void Isolation_duct_in_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (checkBox.IsChecked == true && !cats.Contains(BuiltInCategory.OST_DuctLinings))
            {
                catsiso.Add(BuiltInCategory.OST_DuctLinings);
            }
        }

        private void Isolation_duct_out_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (checkBox.IsChecked == true && !catsiso.Contains(BuiltInCategory.OST_DuctInsulations))
            {
                catsiso.Add(BuiltInCategory.OST_DuctInsulations);
            }
        }

        private void Isolation_pipe_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if  (checkBox.IsChecked == true && !catsiso.Contains(BuiltInCategory.OST_PipeInsulations))
            {
                catsiso.Add(BuiltInCategory.OST_PipeInsulations);
            }
        }
        private void Isolation_area_Checked(object sender, RoutedEventArgs e)
        {
            Area = true;

        }

        private void View_Checked(object sender, RoutedEventArgs e)
        {
            ActiveView = true;
        }

        //UnChecked
        private void Duct1_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (cats.Contains(BuiltInCategory.OST_DuctCurves))
            {
                cats.Remove(BuiltInCategory.OST_DuctCurves);
            }
        }

        private void Lotki_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (cats.Contains(BuiltInCategory.OST_CableTray))
            {
                cats.Remove(BuiltInCategory.OST_CableTray);
            }
        }

        private void FlexDuct_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (cats.Contains(BuiltInCategory.OST_FlexDuctCurves))
            {
                cats.Remove(BuiltInCategory.OST_FlexDuctCurves);
            }
        }

        private void Pipe_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (cats.Contains(BuiltInCategory.OST_PipeCurves))
            {
                cats.Remove(BuiltInCategory.OST_PipeCurves);
            }
        }
        //
        private void Isolation_duct_in_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (catsiso.Contains(BuiltInCategory.OST_DuctLinings))
            {
                catsiso.Remove(BuiltInCategory.OST_DuctLinings);
            }
        }

        private void Isolation_duct_out_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (catsiso.Contains(BuiltInCategory.OST_DuctInsulations))
            {   
                catsiso.Remove(BuiltInCategory.OST_DuctInsulations);
            }
        }

        private void Isolation_pipe_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (catsiso.Contains(BuiltInCategory.OST_PipeInsulations))
            {
                catsiso.Remove(BuiltInCategory.OST_PipeInsulations);
            }
        }
        private void FlexPipe_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string checkBoxName = checkBox.Name;

            if (cats.Contains(BuiltInCategory.OST_FlexPipeCurves))
            {
                cats.Remove(BuiltInCategory.OST_FlexPipeCurves);
            }
        }

        private void View_Unchecked(object sender, RoutedEventArgs e)
        {
            ActiveView = false;
        }
        private void Isolation_area_Unchecked(object sender, RoutedEventArgs e)
        {
            Area = false;

        }
        public void ExecuteTransaction()
        {
            var collectorAllElements = new FilteredElementCollector(Doc);
            var collectorAllElementsIso= new FilteredElementCollector(Doc);

            var collectorView = new FilteredElementCollector(Doc, Doc.ActiveView.Id);
            var collectorViewIso = new FilteredElementCollector(Doc, Doc.ActiveView.Id);

            var collector = collectorAllElements;
            var collectorIso = collectorAllElementsIso;

            //Трубы, воздуховоды и тд.
            var catIds = new List<ElementId>(cats.Select(c => new ElementId((int)c)));
            var catFilt = new ElementMulticategoryFilter(catIds);

            // Изоляция
            var catIdsiso = new List<ElementId>(catsiso.Select(c => new ElementId((int)c)));
            var catFiltIso = new ElementMulticategoryFilter(catIdsiso);
           
           

            if (ActiveView == true)
            {
               collector = collectorView;
               collectorIso = collectorViewIso;

            }
            

            var all_items = collector
                .WherePasses(catFilt)
                .WhereElementIsNotElementType()
                .ToElements();
            
            var isolation = collectorIso
                .WherePasses(catFiltIso)
                .WhereElementIsNotElementType()
                .ToElements();


            using (Transaction t = new Transaction(Doc, "Start"))
            {
                t.Start();

                foreach (Element elem in all_items)
                {
                    if (elem.get_Parameter(adskValue) != null && !elem.get_Parameter(adskValue).IsReadOnly)
                    {
                        lengthFromRevit = elem.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                        lengthCorrect = UnitUtils.ConvertFromInternalUnits(lengthFromRevit, DisplayUnitType.DUT_MILLIMETERS) / 1000;

                        elem.get_Parameter(adskValue).Set(Math.Round(lengthCorrect, 3));

                    }
                }

                foreach (Element iso in isolation)
                {

                    if (iso.get_Parameter(adskValue) != null && !iso.get_Parameter(adskValue).IsReadOnly)
                    {
                        if (Area == false)
                        {
                            lengthFromRevit = iso.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                            lengthCorrect = UnitUtils.ConvertFromInternalUnits(lengthFromRevit, DisplayUnitType.DUT_MILLIMETERS)/1000;

                        } else
                        {
                            lengthFromRevit = iso.get_Parameter(BuiltInParameter.RBS_CURVE_SURFACE_AREA).AsDouble();
                            lengthCorrect = UnitUtils.ConvertFromInternalUnits(lengthFromRevit, DisplayUnitType.DUT_SQUARE_METERS);
                        }
                        

                        iso.get_Parameter(adskValue).Set(Math.Round(lengthCorrect,3));

                    }
                }

                t.Commit();
            }

        }

       
    }
}