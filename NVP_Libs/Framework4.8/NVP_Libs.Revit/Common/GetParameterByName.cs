using Autodesk.Revit.DB;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("элемент", typeof(Element))]
    [NodeInput("параметр", typeof(string))]
    public class GetParameterByName : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var element = (Element)inputs[0].Value;
            var parameterName = (string)inputs[1].Value;

            Parameter parameter = element.LookupParameter(parameterName);
            return new NodeResult(parameter);
        }
    }
}
