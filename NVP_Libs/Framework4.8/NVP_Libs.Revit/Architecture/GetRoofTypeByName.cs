using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("тип крыши", typeof(string))]
    public class GetRoofTypeByName : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var roofTypeName = (string)inputs[0].Value;
            RoofType roofType = new FilteredElementCollector(doc)
               .OfClass(typeof(RoofType))
               .OfType<RoofType>()
               .FirstOrDefault(f => f.Name == roofTypeName);
            return new NodeResult(roofType);
        }
    }
}
