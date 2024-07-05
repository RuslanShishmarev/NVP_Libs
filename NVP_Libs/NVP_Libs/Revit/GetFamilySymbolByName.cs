using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using NVP.API.Nodes;

namespace NVP_Libs.Revit
{
    [NodeInput("тип", typeof(string))]
    public class GetFamilySymbolByName : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var doc = (commandData as ExternalCommandData).Application.ActiveUIDocument.Document;

            var symbolName = (string)inputs[0].Value;

            var symbol = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(s => s.Name == symbolName);
            return new NodeResult(symbol);
        }
    }
}

