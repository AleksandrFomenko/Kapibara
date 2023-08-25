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
using Autodesk.Revit.DB.Mechanical;

namespace Kapibara
{

    public partial class FloorUI : Window
    {
        public FloorUI(Document doc)
        {
            Doc = doc;
            InitializeComponent();
        }
        static Document Doc;
        private bool activeViewLevel;
        private bool activeViewElem;
        private int value_familyInstancse;
        private FilteredElementCollector collectorLevels;
        private FilteredElementCollector collectorElements;
        private string parameterNameString;
        private string x = "0";
        private bool setNegativeLevel;
        private string NegativeLevelText;
        private bool setHightLevel;
        private string HighLevelText;




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
            string selectedParameterName = parametersComboBox.SelectedItem.ToString();
            parameterNameString = selectedParameterName;

        }
        private void WinLoad_floor(object sender, RoutedEventArgs e)
        {
            BindingMap bindingMap = Doc.ParameterBindings;
            DefinitionBindingMapIterator iterator = bindingMap.ForwardIterator();
            List<String> allParametersStr = new List<String>();

            while (iterator.MoveNext())
            {
                Definition definition = iterator.Key as Definition;
                if (definition != null)
                {
                    ParameterType paramType = definition.ParameterType;

                    if (paramType == ParameterType.Text)
                    {
                        allParametersStr.Add(definition.Name);
                    }
                }
            }
            if (allParametersStr.Count != 0)
            {
                foreach (String par in allParametersStr)
                {
                    if (!parametersComboBox.Items.Contains(par))
                    {
                        parametersComboBox.Items.Add(par);
                    }
                    if (par == "ADSK_Этаж")
                    {
                        parametersComboBox.SelectedItem = "ADSK_Этаж";
                    }
                }
            }
            else
            {
                parametersComboBox.Items.Add("Параметры проекта отсутствуют");
                parametersComboBox.SelectedItem = "Параметры проекта отсутствуют";
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (Transaction t = new Transaction(Doc, "floor"))
            {
                t.Start();
               ExecuteTransactionFloor();

                Autodesk.Revit.UI.TaskDialog.Show("Succeeded", "Успешно");
                t.Commit();
                Close();
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
        //Уровень точки.
        static int GetFloorNumber(double Z, List<Level> levels)
        {
            List<Level> negativeElevationLevels = levels
                .Where(level => Math.Round(level.Elevation)<0)
                .ToList();

            int floorNumber = 0;

            for (int i = 0; i < levels.Count; i++)
            {
                Level level = levels[i];

                if (Z >= level.Elevation)
                {
                    floorNumber = i;
                }
                else
                {
                    break;
                }
            }
            if (negativeElevationLevels.Count > 0)
            {
                floorNumber -= negativeElevationLevels.Count;
                if (Z >= 0)
                {
                    floorNumber++;
                }
            }
            
            return floorNumber;
        }
        //Получение двух точек у трубопроводов и воздуховодов
        static (double,double) ProcessTwoPoints (Element elem, List<Level>levels)
        {

            double firstPointZ = 0;
            double secondPointZ = 0;
            
            if (elem is Pipe pipe || elem is Duct duct)
            {
                LocationCurve lc = (LocationCurve)elem.Location;
                Curve c = lc.Curve;
                IList<XYZ> xyz = c.Tessellate();
                firstPointZ = xyz[0].Z;
                secondPointZ = xyz[1].Z;
            }
            return (firstPointZ, firstPointZ);
        }

        //Обработка строковых значений у FamilyInstance (1 точка).
        private string GetStringFloor(int first, List<Level> Levels)
        {
            string result;

            List<Level> HighLevels = Levels
                .Where(level => Math.Round(level.Elevation) > 0)
                .ToList();

            if (setNegativeLevel && first <0)
            {
                result = NegativeLevelText;

            }else if (setHightLevel && first == HighLevels.Count)
            {
                result = HighLevelText;
            } else
            {
                result = string.Format("{0} этаж", first);
            }
            return result;
            
        }
        //Обработка строковых значений у труб, воздуховодов (2 точки)
        private string GetStringFloor(int first,int second, List<Level> Levels)
        {

            string result = "а";



            return result;

        }
        private void ExecuteTransactionFloor()
        {
            if (!activeViewLevel)
            {
                collectorLevels = new FilteredElementCollector(Doc);
            } else
            {
                collectorLevels = new FilteredElementCollector(Doc,Doc.ActiveView.Id);
            }

            List<Level> levels = collectorLevels
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .ToList();

            if (!activeViewElem)
            {
                collectorElements = new FilteredElementCollector(Doc);

            } else
            {
                collectorElements = new FilteredElementCollector(Doc, Doc.ActiveView.Id);
            }
            List<FamilyInstance> familyInstances = collectorElements
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            List<Level> sortedLevels = levels.OrderBy(level => level.Elevation).ToList();

            CollectionMethods cm = new CollectionMethods();

            foreach (FamilyInstance fi in familyInstances)
            {
                double instanceElevation = (fi.Location as LocationPoint).Point.Z;
                x = GetFloorNumberFromInstance(instanceElevation, sortedLevels).ToString();
                cm.setParameterValueByNameToElement(fi as Element, parameterNameString,x.ToString());
            }
        }
    }
}
