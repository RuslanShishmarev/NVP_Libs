using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("категория", typeof(string))]
    [NodeInput("без типов", typeof(bool))]
    public class GetElementsByBuiltInCategory : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            string categoryName = (string)inputs[0].Value;
            bool withoutTypes = true;
            if (inputs[1]?.Value != null) 
            {
                withoutTypes = (bool)inputs[1].Value;
            }

            BuiltInCategory category = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), categoryName);
            var collector = new FilteredElementCollector(doc)
                .OfCategory(category);

            if (withoutTypes)
            {
                collector = collector.WhereElementIsNotElementType();
            }

            return new NodeResult(collector.ToElements());
        }
    }
}
