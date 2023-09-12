using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
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
        private float number;
        private string prf_text;
        private string sfc_text;
        private float numberFirst;
        
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

            if (Doc.ActiveView.ViewType == ViewType.Schedule)
            {
                numberFirst = number;
                ViewSchedule viewSchedule = Doc.ActiveView as ViewSchedule;
                TableData tableData = viewSchedule.GetTableData();
                TableSectionData sectionData = tableData.GetSectionData(SectionType.Body);
                ExecuteNumeration(sectionData);
                Autodesk.Revit.UI.TaskDialog.Show("Succeeded", "Успешно");
                Close();
            }
            else
            {
                Autodesk.Revit.UI.TaskDialog.Show("Error", "Необходимо открыть спецификацию");
            }
        }
        private void number_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox Number = (System.Windows.Controls.TextBox)sender;
            number = float.Parse(Number.Text);
        }
        private void prf_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox prf = (System.Windows.Controls.TextBox)sender;
            prf_text = prf.Text;

        }

        private void sfc_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox sfc = (System.Windows.Controls.TextBox)sender;
            sfc_text = sfc.Text;

        }
        private void udpate_Checked(object sender, RoutedEventArgs e)
        {
            updateNumbering = true;
        }
        private void udpate_Unchecked(object sender, RoutedEventArgs e)
        {
            updateNumbering = false;
        }
        private void WinLoaded(object sender, RoutedEventArgs e)
        {
            ViewSchedule schedule = Doc.ActiveView as ViewSchedule;

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
        private void ExecuteNumeration(TableSectionData sectionData)
        {
            CollectionMethods cm = new CollectionMethods();
            using (TransactionGroup transGroup = new TransactionGroup(Doc, "Kapibara - TransactionGroup"))
            {
                transGroup.Start();
                    for (int i = 0; i < sectionData.NumberOfRows; i++)
                    {
                    if (sectionData.CanRemoveRow(i))
                    {
                        elementsInRow(i, sectionData, cm);
                        number++;
                    }
                    else
                        {
                        if (updateNumbering)
                        {
                            number = numberFirst;
                        }
                            continue;
                        }
                    }
                transGroup.Assimilate();
            }  
        }
        private void elementsInRow(int x, TableSectionData sectionData, CollectionMethods cm)
        {
            Transaction tr = new Transaction(Doc);
            tr.Start("Transacton for delete");
 
            sectionData.RemoveRow(x);
            List<ElementId> elemAfterDelete = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                .WhereElementIsNotElementType()
                .ToElementIds()
                .ToList();
            tr.RollBack();

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
            }
            elemOnView = elemOnView.Except(result).ToList();
            Transaction T = new Transaction(Doc);
            T.Start("Transacton set");
            foreach (ElementId elementId in result)
            {
                setValue(Doc.GetElement(elementId));

            }
            T.Commit();
        }
        private void setValue(Element elem)
        {
            if (elem != null)
            {
                Parameter par = elem.LookupParameter(ParameterName);
                if (par != null)
                {
                    if (par.Definition.ParameterType== ParameterType.Integer)
                    {
                        if (par !=null && par.IsReadOnly == false)
                        {
                            par.Set((int)(number));
                        }
                    } else if (par.Definition.ParameterType == ParameterType.Number)
                    {
                        if (par != null && par.IsReadOnly == false)
                        {
                            string valueAsString = number.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            par.Set(float.Parse(valueAsString));
                        }
                    }
                    else if (par.Definition.ParameterType == ParameterType.Text)
                    {
                        if (par != null && par.IsReadOnly == false)
                        {
                            string prfText = prf_text != null ? prf_text.ToString() : string.Empty;
                            string sfcText = sfc_text != null ? sfc_text.ToString() : string.Empty;

                            par.Set(string.Format("{0}{1}{2}", prfText, number, sfcText));
                        }
                    }
                }
            }
        }
    }
}