using Autodesk.Revit.DB;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit
{
    [NodeInput("элемент", typeof(Element))]
    [NodeInput("свойство", typeof(string))]
    public class GetPropertyByName : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var element = (Element)inputs[0].Value;
            var parameterName = (string)inputs[1].Value;

            Parameter parameter = element.LookupParameter(parameterName);
            
            if (parameter != null)
            {
                return new NodeResult(parameter);
            }
            return null;
        }
    }
}
