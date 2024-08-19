using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("форма стержня", typeof(string))]
    public class SetRebarShape : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var rebarShapeName = (string)inputs[0].Value;
            var rebarShape = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Cast<RebarShape>()
                .FirstOrDefault(s => s.Name == rebarShapeName);
            return new NodeResult(rebarShape);
        }
    }
}
