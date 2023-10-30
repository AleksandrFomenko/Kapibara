using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class RegisterPipeUpdaterCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiApp = commandData.Application;
        Document doc = uiApp.ActiveUIDocument.Document;

        try
        {
            PipeUpdater pipeUpdater = new PipeUpdater(uiApp.ActiveAddInId);

            UpdaterRegistry.RegisterUpdater(pipeUpdater);

            ElementClassFilter pipeFilter = new ElementClassFilter(typeof(Pipe));
            UpdaterRegistry.AddTrigger(pipeUpdater.GetUpdaterId(), pipeFilter, Element.GetChangeTypeElementAddition());

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            TaskDialog.Show("ошибка", "ошибка " + ex.Message);
            return Result.Failed;
        }
    }
}
