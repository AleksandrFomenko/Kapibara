using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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

namespace Kapibara
{
    public partial class NumerationGeneralFamiliesWPF : Window
    {
        public NumerationGeneralFamiliesWPF(Document doc)
        {
            Doc = doc;
            InitializeComponent();
        }
        Document Doc;
        private List<ElementId> elemOnView;
        private float number;
        private string algos;
        private string parameterName;
        private float step;
        private string oneValue;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            algos = Algos.SelectedItem.ToString();
        }
        private void number_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox Number = (System.Windows.Controls.TextBox)sender;
            if (float.TryParse(Number.Text, out float parsedNumber))
            {
                number = parsedNumber;
            }
        }
        private void Step_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox stepTextBox = (System.Windows.Controls.TextBox)sender;

            if (float.TryParse(stepTextBox.Text, out float stepFloat))
            {
                step = stepFloat;
            }
        }
        private void OneValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                oneValue = textBox.Text;
            }
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

                elemOnView = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                .WhereElementIsNotElementType()
                .ToElementIds()
                .ToList();
            }
            Algos.Items.Add("Задать вложенным индекс родительского + 0.1");
            Algos.Items.Add("Задать каждому вложенному индекс родительского + n*0.1");
            Algos.Items.Add("Задать вложенным одно значение");
            Algos.SelectedItem = "Задать вложенным индекс родительского + 0.1";

        }
        private void Parameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedElement = Parameters.SelectedItem.ToString();
            parameterName = selectedElement;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Doc.ActiveView.ViewType == ViewType.Schedule)
            {
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
                        number+=step;
                    }
                    else
                    {
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
                CollectionMethods cm = new CollectionMethods();
                Parameter par = elem.LookupParameter(parameterName);
                if (par != null && par.IsReadOnly == false)
                {
                    if (par.Definition.ParameterType == ParameterType.Integer)
                    {
                        par.Set((int)(number));
                        int n = 1;
                        foreach (Element elemSub in cm.GetSubComponents(elem))
                        {
                            Parameter parSub = elemSub.LookupParameter(parameterName);
                            if (parSub != null && parSub.IsReadOnly == false)
                            {
                                if (algos == "Задать вложенным индекс родительского + 0.1")
                                {
                                    parSub.Set((int)(number + 1));
                                }
                                else if (algos == "Задать каждому вложенному индекс родительского + n*0.1")
                                {

                                    parSub.Set((int)(number+1*n));

                                }
                                else
                                {
                                    if (int.TryParse(oneValue, out int intValue))
                                    {
                                        parSub.Set(intValue);
                                    }
                                    
                                }
                            }
                        }
                    }
                    else if (par.Definition.ParameterType == ParameterType.Number)
                    {
                        if (par != null && par.IsReadOnly == false)
                        {
                            int n = 1;
                            par.Set(number);
                            foreach (Element elemSub in cm.GetSubComponents(elem))
                            {
                                Parameter parSub = elemSub.LookupParameter(parameterName);
                                if (parSub != null && parSub.IsReadOnly == false)
                                {
                                    if (algos == "Задать вложенным индекс родительского + 0.1")
                                    {
                                        parSub.Set(number + 0.1);
                                    }
                                    else if (algos == "Задать каждому вложенному индекс родительского + n*0.1")
                                    {
                                        parSub.Set(number+0.1*n);
                                        n++;
                                    }
                                    else
                                    {
                                        parSub.Set(float.Parse((oneValue).ToString(System.Globalization.CultureInfo.InvariantCulture)));
                                    }
                                }
                            }
                        }
                    }
                    else if (par.Definition.ParameterType == ParameterType.Text)
                    {
                        if (par != null && !par.IsReadOnly)
                        {
                            par.Set(string.Format("{0}", number));
                            int n = 1;
                            foreach (Element elemSub in cm.GetSubComponents(elem))
                            {
                                Parameter parSub = elemSub.LookupParameter(parameterName);
                                if (parSub != null && !parSub.IsReadOnly)
                                {
                                    if (algos == "Задать вложенным индекс родительского + 0.1")
                                    {
                                        parSub.Set(string.Format("{0}", number + 0.1));
                                    }
                                    else if (algos == "Задать каждому вложенному индекс родительского + n*0.1")
                                    {
                                        parSub.Set(string.Format("{0}", (number + 0.1 * n)));
                                        n++;
                                    }
                                    else
                                    {
                                        parSub.Set(string.Format("{0}", oneValue));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }        
}

