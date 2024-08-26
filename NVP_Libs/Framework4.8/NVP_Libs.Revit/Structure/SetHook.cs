using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("тип отгиба", typeof(string))]
    public class SetHook : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var hookName = (string)inputs[0].Value;
            RebarHookType hookType = null;
            if (hookName != null)
            {
                hookType = new FilteredElementCollector(doc)
                 .OfClass(typeof(RebarHookType))
                 .Cast<RebarHookType>()
                 .FirstOrDefault(h => h.Name == hookName);
            }
            return new NodeResult(hookType);
        }
    }
}
