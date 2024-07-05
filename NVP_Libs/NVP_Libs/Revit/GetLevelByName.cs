using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using NVP.API.Nodes;

namespace NVP_Libs.Revit
{
    [NodeInput("имя", typeof(string))]
    public class GetLevelByName : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var doc = (commandData as ExternalCommandData).Application.ActiveUIDocument.Document;

            var levelName = (string)inputs[0].Value;

            var level = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .FirstOrDefault(l => l.Name == levelName);
            return new NodeResult(level);
        }
    }
}
