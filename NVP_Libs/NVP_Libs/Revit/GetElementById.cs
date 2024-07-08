using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;

namespace NVP_Libs.Revit
{
    [NodeInput("id", typeof(string))]
    public class GetElementById : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var elementIdValue = Int64.Parse((string)inputs[0].Value);
            var elementId = new ElementId(elementIdValue); 

            Element element = doc.GetElement(elementId);
            return new NodeResult(element);
        }
    }
}