using NVP.API.Geometry;
using NVP.API.Nodes;

using System.Collections.Generic;

using RevitLine = Autodesk.Revit.DB.Line;
using RevitXYZ = Autodesk.Revit.DB.XYZ;

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
