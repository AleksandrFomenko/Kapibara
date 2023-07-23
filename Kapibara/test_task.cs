using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Linq;

namespace Kapibara
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class test_task : IExternalCommand

    {
        private Document doc;
        private Element element1;
        private Element element2;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            UIDocument uiDoc = new UIDocument(doc);
            IList<Reference> pickedReferences = uiDoc.Selection.PickObjects(ObjectType.Element, "Выберите два элемента.");
            if (pickedReferences.Count == 2)
            {
                element1 = doc.GetElement(pickedReferences[0]);
                element2 = doc.GetElement(pickedReferences[1]);

                // Открываем окно TestTask и передаем выбранные элементы через его конструктор
                TestTask wpfForm = new TestTask(doc,element1,element2);
                wpfForm.ShowDialog();
            }
            else
            {
                TaskDialog.Show("Ошибка", "Пожалуйста, выберите ровно два элемента.");
            }


            return Result.Succeeded;

        }
       

    }
}
