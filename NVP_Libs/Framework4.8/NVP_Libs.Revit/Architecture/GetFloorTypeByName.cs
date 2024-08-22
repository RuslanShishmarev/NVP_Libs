using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("тип перекрытия", typeof(string))]
    public class GetFloorTypeByName : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var floorTypeName = (string)inputs[0].Value;
            FloorType floorType = new FilteredElementCollector(doc)
                .OfClass(typeof(FloorType))
                .OfType<FloorType>()
                .FirstOrDefault(f => f.Name == floorTypeName);
            return new NodeResult(floorType);
        }
    }
}
