using Autodesk.Revit.DB;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("элемент", typeof(Element))]
    public class GetCurveByElement : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var element = (Element)inputs[0].Value;
            var curve = (element.Location as LocationCurve).Curve;
            return new NodeResult(curve);
        }
    }
}
