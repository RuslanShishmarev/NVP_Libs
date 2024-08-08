using NVP.API.Nodes;

using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("точка", typeof(XYZ))]
    public class ConvertXYZToRevit : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var point = (XYZ)inputs[0].Value;
            RevitXYZ revitPoint = new RevitXYZ(point.X, point.Y, point.Z);
            return new NodeResult(revitPoint);
        }
    }
}
