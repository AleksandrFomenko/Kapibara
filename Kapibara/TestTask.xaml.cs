using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Kapibara
{
    public partial class TestTask : Window
    {
        private Document doc;
        private Element element1;
        private Element element2;
        private string parNameFirst;
        private string parNameSecond;
        private string result = "Успешно";

        public TestTask(Document doc,Element elemFirst,Element elemSecond)
        {
            InitializeComponent();
            this.doc = doc;
            this.element1 = elemFirst;
            this.element2 = elemSecond;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> commonParameterNames = new List<string>();
            foreach (Parameter param1 in element1.Parameters)
            {
                if (param1.Definition.ParameterType == ParameterType.Text)
                {
                    foreach (Parameter param2 in element2.Parameters)
                    {
                        if (param2.Definition.ParameterType == ParameterType.Text && param1.Definition.Name == param2.Definition.Name)
                        {
                            commonParameterNames.Add(param1.Definition.Name);
                        }
                    }
                }
            }

                foreach (string paramName in commonParameterNames)
                {
                    if (!get_param.Items.Contains(paramName))
                    {
                        get_param.Items.Add(paramName);
                        set_param.Items.Add(paramName);
                    }
               }
        }
        

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedParameterName = get_param.SelectedItem.ToString();
            parNameFirst = selectedParameterName;
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            string selectedParameterName = set_param.SelectedItem.ToString();
            parNameSecond = selectedParameterName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (element1 != null && element2 != null)
            {
                using (Transaction t = new Transaction(doc, "Start"))
                {
                    t.Start();
                    ExecuteTransactionTestTask();
                    t.Commit();
                }

                Autodesk.Revit.UI.TaskDialog.Show("result", result);
                Close();
            }
           
        }

      

        private void ExecuteTransactionTestTask()
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> elements = collector.WhereElementIsNotElementType().ToElements();
            foreach (Element elem in elements)
            {
                if (elem != null)
                {
                    Parameter parOne = elem.LookupParameter(parNameFirst);
                    Parameter parTwo = elem.LookupParameter(parNameSecond);
                    if (parOne != null && parTwo != null)
                    {
                        if (!parTwo.IsReadOnly && parOne.AsString() != null)
                        {
                            parTwo.Set(parOne.AsString());
                        }
                        
                    }
                }
            }
        }
    }
}
