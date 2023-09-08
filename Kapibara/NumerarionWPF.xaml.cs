using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;



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


        private bool updateNumbering;
        private string ParameterName;
        private List<ElementId> elemOnView;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Doc.ActiveView.ViewType == ViewType.Schedule)
            {
                
                ViewSchedule viewSchedule = Doc.ActiveView as ViewSchedule;
                TableData tableData = viewSchedule.GetTableData();
                TableSectionData sectionData = tableData.GetSectionData(SectionType.Body);
                ExecuteNumeration(sectionData);

            } else
            {
                Autodesk.Revit.UI.TaskDialog.Show("Error", "Необходимо открыть спецификацию");
            }
        }
        private void WinLoaded(object sender, RoutedEventArgs e)
        {
            ViewSchedule schedule = Doc.ActiveView as ViewSchedule; // Получите ссылку на вашу спецификацию

            if (schedule != null)
            {
                ScheduleDefinition definition = schedule.Definition;

                foreach (ScheduleFieldId fieldId in definition.GetFieldOrder())
                {
                    ScheduleField field = definition.GetField(fieldId);
                    string paramName = field.GetName();
                    Parameters.Items.Add(paramName);
                }
                foreach (string par in Parameters.Items)
                {
                    if (par == "ADSK_Позиция")
                    {
                        Parameters.SelectedItem = "ADSK_Позиция";
                    }
                }
                elemOnView = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                .WhereElementIsNotElementType()
                .ToElementIds()
                .ToList();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedElement = Parameters.SelectedItem.ToString();
            ParameterName = selectedElement;
        }
        private void ExecuteNumeration(TableSectionData sectionData) {
            CollectionMethods cm = new CollectionMethods();
            for (int i = 0; i < sectionData.NumberOfRows; i++)
            {
                if (sectionData.CanRemoveRow(i))
                {
                    elementsInRow(i, sectionData,cm);
                } else
                {
                    continue;
                }
            }
        }

        private void elementsInRow (int x,TableSectionData sectionData, CollectionMethods cm)
        {
            Transaction t = new Transaction(Doc);
            t.Start("Transacton for delete");

            sectionData.RemoveRow(x);
            List<ElementId> elemAfterDelete = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                .WhereElementIsNotElementType()
                .ToElementIds()
                .ToList();
            t.RollBack();

            List<ElementId> result = elemOnView.Except(elemAfterDelete).ToList();
            foreach (ElementId elemId in result)
            {
                Element element = Doc.GetElement(elemId);
                if (element != null)
                {
                    List<Element> generalFamilies = cm.GetSubComponents(element);
                    List<ElementId> generalFamilyIds = generalFamilies.Select(e => e.Id).ToList();
                    result = result.Except(generalFamilyIds).ToList();
                }
                /*
                foreach (ElementId elemIdSecond in result)
                {
                    Element elementSecond = Doc.GetElement(elemId);
                    if (element != null)
                    {
                        List<Element> generalFamiliesSecond = cm.GetSubComponents(elementSecond);
                        List<ElementId> generalFamilyIdsSecond = generalFamiliesSecond.Select(e => e.Id).ToList();
                        result = result.Except(generalFamilyIdsSecond).ToList();
                    }
                }
                */
            }

            Transaction T = new Transaction(Doc);
            T.Start("Transacton set");
            foreach (ElementId elementId in result)
            {
                Doc.GetElement(elementId).LookupParameter(ParameterName).Set(x.ToString());
            }
            T.Commit();
        }

       
    }


    

        
}
