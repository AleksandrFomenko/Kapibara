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
        private FilteredElementCollector collectorLevels;
        private FilteredElementCollector collectorElements;
        private FilteredElementCollector collectorPipeDuct;
        private string parameterNameString;
        private bool setNegativeLevel;
        private string NegativeLevelText;
        private bool setHightLevel;
        private string HighLevelText;
        private bool setNumber;
        private string resultFinal;

        private static List<BuiltInCategory> cats_iso = new List<BuiltInCategory> {
            BuiltInCategory.OST_DuctInsulations,
            BuiltInCategory.OST_DuctLinings,
            BuiltInCategory.OST_PipeInsulations
        };

        private static List<BuiltInCategory> cats_dct_pipe = new List<BuiltInCategory> {
        BuiltInCategory.OST_DuctCurves,
        BuiltInCategory.OST_FlexDuctCurves,
        BuiltInCategory.OST_PipeCurves
        };

        ElementMulticategoryFilter emfDuctPipes = new ElementMulticategoryFilter(cats_dct_pipe);

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
                int summOfElements =  ExecuteTransactionFloor();

                Autodesk.Revit.UI.TaskDialog.Show("Succeeded", string.Format("Обработано {0} элементов",summOfElements));
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
        private void HighLevel_Checked(object sender, RoutedEventArgs e)
        {
            setHightLevel = true;
        }

        private void HighLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            setHightLevel = false;
        }
        private void NegativeLvl_Checked(object sender, RoutedEventArgs e)
        {
            setNegativeLevel = true;
        }
        private void NegativeLvl_Unchecked(object sender, RoutedEventArgs e)
        {
            setNegativeLevel = false;
        }
        private void Onlynumber_Checked(object sender, RoutedEventArgs e)
        {
            setNumber = true;
        }

        private void Onlynumber_Unchecked(object sender, RoutedEventArgs e)
        {
            setNumber = false;
        }
        private void HighText_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender;
            HighLevelText = textBox.Text;

        }

        private void NegativeText_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender;
            NegativeLevelText = textBox.Text;

        }
        //Уровень точки.
        static int GetFloorNumber(double Z, List<Level> levels, List<Level> negativeElevationLevels)
        {
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
            } else
            {
                floorNumber++;
            }

            return floorNumber;


        }
        //Получение двух точек у трубопроводов и воздуховодов
        static (double,double) ProcessTwoPoints (Element elem)
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
            return (firstPointZ, secondPointZ);
        }

        //Обработка строковых значений у FamilyInstance (1 точка).
        private string GetStringFloor(int first, List<Level> HighLevels)
        {
            string result;

            if (setNumber)
            {
                result = string.Format("{0}", first);
            } else
            {
                if (setNegativeLevel && first < 0)
                {
                    result = NegativeLevelText;

                }
                else if (setHightLevel && first == HighLevels.Count)
                {
                    result = HighLevelText;
                }
                else
                {
                    result = string.Format("{0} этаж", first);
                }

            }
            
            return result;   
        }
        //Обработка строковых значений у труб, воздуховодов (2 точки)
        private string GetStringFloor(int first,int second, List<Level> HighLevels)
        {
            string result;
            int maximum = Math.Max(first, second);
            int minimum = Math.Min(first, second);


            if (first == second) {
                result = GetStringFloor(first, HighLevels);
            } else {
                if (setNumber)
                {
                    result = string.Format("{0}—{1}", minimum, maximum);
                }
                else
                {
                    if (setNegativeLevel)
                    {
                        if (minimum < 0)
                        {
                            if (maximum < 0)
                            {
                                result = NegativeLevelText;
                            }
                            else
                            {
                                if (setHightLevel)
                                {
                                    if (maximum == HighLevels.Count)
                                    {
                                        result = string.Format("C {0} до {1}", NegativeLevelText, HighLevelText);
                                    }
                                    else
                                    {
                                        result = string.Format("C {0} до {1} этажа", NegativeLevelText, maximum);
                                    }
                                }
                                else
                                {
                                    result = string.Format("C {0} до {1} этажа", NegativeLevelText, maximum);
                                }
                            }
                        } else
                        {
                            if (setHightLevel)
                            {
                                if (maximum == HighLevels.Count)
                                {
                                    result = string.Format("C {0} этажа до {1}", minimum, HighLevelText);
                                }
                                else
                                {
                                    result = string.Format("C {0} этажа до {1} этажа", minimum, maximum);
                                }
                            }
                            else
                            {
                                if (maximum < 0)
                                {
                                    result = string.Format("C {0} до {1} этажа1111", NegativeLevelText, maximum);
                                }
                                else
                                {
                                    result = string.Format("C {0} этажа до {1} этажа", minimum, maximum);
                                }
                            }

                        }
                    }
                    else if (setHightLevel)
                    {
                        if (maximum == HighLevels.Count)
                        {
                            if (minimum == HighLevels.Count)
                            {
                                result = HighLevelText;
                            }
                            else
                            {
                                if (setNegativeLevel)
                                {
                                    if (minimum < 0)
                                    {
                                        result = string.Format("C {0} до {1}", NegativeLevelText, HighLevelText);
                                    }
                                    else
                                    { 
                                        if(maximum == HighLevels.Count) {
                                            result = string.Format("C {0} этажа до {1}", minimum, HighLevelText);

                                        } else {
                                            result = string.Format("C {0} этажа до {1} этажа", minimum, maximum);
                                        }
                                        
                                    }
                                }
                                else
                                {
                                    result = string.Format("C {0} этажа до {1}", minimum, HighLevelText);
                                }
                            }
                        } else
                        {
                            if (minimum<0)
                            {
                                result = string.Format("C {0}  до {1} этажа", NegativeLevelText, maximum);
                            }
                            else
                            {
                                result = string.Format("C {0} этажа до {1} этажа", minimum, maximum);
                            }
                        }
                    }
                    else
                    {
                        result = string.Format("C {0} этажа до {1} этажа", minimum, maximum);
                    }
                }
            }

            return result;
        }

        private int ExecuteTransactionFloor()
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
                collectorPipeDuct = new FilteredElementCollector(Doc);

            } else
            {
                collectorElements = new FilteredElementCollector(Doc, Doc.ActiveView.Id);
                collectorPipeDuct = new FilteredElementCollector (Doc, Doc.ActiveView.Id);
            }

            List<FamilyInstance> familyInstances = collectorElements
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();
            List <Element> ductPipe = collectorPipeDuct
                .WhereElementIsNotElementType()
                .WherePasses(emfDuctPipes)
                .ToList();


            List<Level> sortedLevels = levels.OrderBy(level => level.Elevation).ToList();

            CollectionMethods cm = new CollectionMethods();

            List<Level> HighLevels = sortedLevels
                .Where(level => Math.Round(level.Elevation) >= 0)
                .ToList();

            List<Level> negativeElevationLevels = levels
                .Where(level => Math.Round(level.Elevation) < 0)
                .ToList();

            foreach (FamilyInstance fi in familyInstances)
            {
                double instanceElevation = (fi.Location as LocationPoint).Point.Z;
                resultFinal = GetStringFloor(GetFloorNumber(instanceElevation, sortedLevels, negativeElevationLevels), HighLevels);
                
                cm.setParameterValueByNameToElement(fi as Element, parameterNameString, resultFinal.ToString());
            }
            foreach (Element elem in ductPipe) {
                (double firstPoint, double secondPoint) = ProcessTwoPoints(elem);
                int firstPointToInt = GetFloorNumber(firstPoint, sortedLevels, negativeElevationLevels);
                int secondPointToInt = GetFloorNumber(secondPoint, sortedLevels, negativeElevationLevels);
                resultFinal = GetStringFloor(firstPointToInt, secondPointToInt, HighLevels);
                cm.setParameterValueByNameToElement(elem as Element, parameterNameString, resultFinal.ToString());
            }

            return ductPipe.Count+familyInstances.Count;
        }
        

    }
}
        
