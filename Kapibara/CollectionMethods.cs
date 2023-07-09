using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;

namespace Kapibara
{
    internal class CollectionMethods
    {
        public CollectionMethods()
        {
          
        }
        public List<Element> GetSubComponents(Element element)
        {
            List<Element> subComponents = new List<Element>();

            if (element is FamilyInstance familyInstance)
            {
                var subComponentIds = familyInstance.GetSubComponentIds();
                subComponents = subComponentIds.Select(id => element.Document.GetElement(id)).ToList();
            }

            return subComponents;
        }

        public void setParameterValueByNameToElement(Element elem,string parameterName, string value)
        {
            if (elem.LookupParameter(parameterName) != null && !elem.LookupParameter(parameterName).IsReadOnly)
            {
                elem.LookupParameter(parameterName).Set(value);

            }
        }
    }
}
