using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;

public class PipeUpdater : IUpdater
{
    static AddInId _appId;
    static UpdaterId _updaterId;

    public PipeUpdater(AddInId id)
    {
        _appId = id;
        _updaterId = new UpdaterId(_appId, new Guid("2a324547-a455-467f-9aa4-54c2dbfa8522"));
    }

    public void Execute(UpdaterData data)
    {
        try
        {
            Document doc = data.GetDocument();
            
            foreach (ElementId id in data.GetAddedElementIds())
            {
                Element elem = doc.GetElement(id);

                if (elem is Pipe)
                {
                    Pipe pipe = elem as Pipe;

                    using (Transaction trans = new Transaction(doc, "Update Pipe Parameter"))
                    {
                        trans.Start();
                        Parameter parameter = pipe.LookupParameter("тест");
                        
                        parameter.Set("Значение_параметра");
                        
                        trans.Commit();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Error", "Eror" + ex.Message);
        }
    }

    public string GetAdditionalInformation()
    {
        return "The";
    }

    public ChangePriority GetChangePriority()
    {
        return ChangePriority.MEPFixtures;
    }

    public UpdaterId GetUpdaterId()
    {
        return _updaterId;
    }

    public string GetUpdaterName()
    {
        return "ElevationWatcherUpdater";
    }

}