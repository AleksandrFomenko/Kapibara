﻿using Autodesk.Revit.DB;
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
using Autodesk.Revit.DB;
using Binding = Autodesk.Revit.DB.Binding;

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
        private string parName;
        private string value;
        private List<Parameter> allParameters = new List<Parameter>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            BindingMap bindingMap = Doc.ParameterBindings;
            DefinitionBindingMapIterator iterator = bindingMap.ForwardIterator();
            List<String> allParametersStr = new List<String>();

            while (iterator.MoveNext())
            {
                Definition definition = iterator.Key as Definition;
                if (definition != null && definition.ParameterType == ParameterType.Text)
                {
                    allParametersStr.Add(definition.Name);
                }
            }



            foreach (String par in allParametersStr)
            {
                if (!Parameters.Items.Contains(par))
                {
                    Parameters.Items.Add(par);
                }
                if (par == "ААА_Раздел")
                {
                    Parameters.SelectedItem = "ААА_Раздел";
                }
            }
        }
        
    
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExecuteTransactionSystemName();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            value = textBox.Text;

        }
        private void Parameters_Selection(object sender, SelectionChangedEventArgs e)
        {
            if (Parameters.SelectedItem != null)
            {
                string selectedParameterName = Parameters.SelectedItem.ToString();
                parName = selectedParameterName;
            }

        }
        public void ExecuteTransactionSystemName()
        {
            Transaction t = new Transaction(Doc);
            t.Start("Start");
            List <Element> elem = new FilteredElementCollector(Doc,Doc.ActiveView.Id).WhereElementIsNotElementType().ToElements().ToList();
            foreach (Element element in elem)
            {
                if (element != null)
                {
                    if (element.LookupParameter(parName)!=null && !element.LookupParameter(parName).IsReadOnly)
                    {
                        element.LookupParameter(parName).Set(value);

                    }

                }
            }
            t.Commit();
        }
    }
}
