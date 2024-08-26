using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("тип стены", typeof(string))]
    public class GetWallTypeByName : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var wallTypeName = (string)inputs[0].Value;
            WallType wallType = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .OfType<WallType>()
                .FirstOrDefault(w => w.Name == wallTypeName);
            return new NodeResult(wallType);
        }
    }
}
