using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;


namespace NVP_Libs.Revit.Structure
{
    [NodeInput("тип арматуры", typeof(string))]
    public class GetBarTypeByName : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var rebarBarTypeName = (string)inputs[0].Value;
            var rebarBarType = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .FirstOrDefault(bt => bt.Name == rebarBarTypeName);
            return new NodeResult(rebarBarType);
        }
    }
}
