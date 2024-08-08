using NVP.API.Nodes;
using NVP.API.Geometry;

using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;
using RevitLine = Autodesk.Revit.DB.Line;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("линия", typeof(Line))]
    public class ConvertLineToRevit : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var line = (Line)inputs[0].Value;
            var start = new RevitXYZ(line.Start.X, line.Start.Y, line.Start.Z);
            var end = new RevitXYZ(line.End.X, line.End.Y, line.End.Z);

            RevitLine revitLine = RevitLine.CreateBound(start, end);
            return new NodeResult(revitLine);
        }
    }
}
