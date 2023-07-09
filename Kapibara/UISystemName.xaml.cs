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

namespace Kapibara
{
   
    public partial class UISystemName : Window
    {
        Document Doc;
        public UISystemName(Document doc)
        {
            InitializeComponent();
            Doc = doc;
        }
        

        

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BlockSystemName.Items.Add("Имя системы");
            BlockSystemName.Items.Add("Сокращение для системы");
            BlockSystemName.Items.Add("Тип системы");
            BlockSystemName.SelectedIndex = 0;

            FilteredElementCollector collector_duct = new FilteredElementCollector(Doc);

            List<Element> elements_duct = (List<Element>)collector_duct
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType().ToElements();

            FilteredElementCollector collector_pipe = new FilteredElementCollector(Doc);

            List<Element> elements_pipe = (List<Element>)collector_pipe
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType().ToElements();

            List<Parameter> parameters_duct = elements_duct[0].Parameters
                .Cast<Parameter>()
                .Where(parameter => parameter.StorageType == StorageType.String && parameter.Id.IntegerValue > 0)
                .Select(Parameter=> Parameter)
                .ToList();
            List<Parameter> parameters_pipes = elements_pipe[0].Parameters
                .Cast<Parameter>()
                .Where(parameter => parameter.StorageType == StorageType.String && parameter.Id.IntegerValue > 0)
                .Select(Parameter => Parameter)
                .ToList();
            List<Parameter> allParameters = new List<Parameter>();
            allParameters.AddRange(parameters_duct);
            allParameters.AddRange(parameters_pipes);

            foreach (Parameter par in allParameters)
            {
                if (!BlockUserParameters.Items.Contains(par.Definition.Name))
                {
                    BlockUserParameters.Items.Add(par.Definition.Name);
                }

                if (par.Definition.Name == "ААА_Имя системы")
                {
                    BlockUserParameters.SelectedItem = "ААА_Имя системы";
                }
            }
        }

        private void BlockSystemName_Selection(object sender, SelectionChangedEventArgs e)
        {
            

        }

        private void BlockUserParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
