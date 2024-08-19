using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("тип отгиба", typeof(string))]
    [NodeInput("лево/право", typeof(bool))]
    public class SetHook : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            List<object> list = new List<object>();
            var hookName = (string)inputs[0].Value;
            if (hookName == null)
            {
                RebarHookType hookNull = null;
                list.Add(hookNull);
            }
            else
            {
                var hook = new FilteredElementCollector(doc)
                 .OfClass(typeof(RebarHookType))
                 .Cast<RebarHookType>()
                 .FirstOrDefault(h => h.Name == hookName);
                list.Add(hook);
            }
            var orientBool = (bool)inputs[1].Value;
            RebarHookOrientation orient = RebarHookOrientation.Right;
            if (orientBool)
            {
                orient = RebarHookOrientation.Left;
            }
            list.Add(orient);
            return new NodeResult(list);
        }
    }
}
