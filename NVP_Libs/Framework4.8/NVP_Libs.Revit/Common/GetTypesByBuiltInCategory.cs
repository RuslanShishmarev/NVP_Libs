using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("категория", typeof(string))]
    public class GetTypesByBuiltInCategory : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            string categoryName = (string)inputs[0].Value;

            BuiltInCategory category = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), categoryName);
            var collector = new FilteredElementCollector(doc)
                .OfCategory(category)
                .WhereElementIsElementType();

            return new NodeResult(collector.ToElements());
        }
    }
}
