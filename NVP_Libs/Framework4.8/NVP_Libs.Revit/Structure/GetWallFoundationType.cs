using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("тип фундамента", typeof(string))]
    public class GetWallFoundationType : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var foundationTypeName = (string)inputs[0].Value;
            var foundationType = new FilteredElementCollector(doc)
                .OfClass(typeof(WallFoundationType))
                .FirstOrDefault(f => f.Name == foundationTypeName);
            return new NodeResult(foundationType);
        }
    }
}
