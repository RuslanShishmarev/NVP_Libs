using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("имя", typeof(string))]
    public class GetLevelByName : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var levelName = (string)inputs[0].Value;
            var level = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .FirstOrDefault(l => l.Name == levelName);
            return new NodeResult(level);
        }
    }
}
