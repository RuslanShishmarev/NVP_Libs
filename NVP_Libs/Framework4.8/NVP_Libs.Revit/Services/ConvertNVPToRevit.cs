using NVP.API.Geometry;

using RevitLine = Autodesk.Revit.DB.Line;
using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit.Services
{
    public static class ConvertNVPToRevit
    {
        public static RevitXYZ ToRevit(this XYZ point)
        {
            RevitXYZ revitPoint = new RevitXYZ(point.X, point.Y, point.Z);
            return revitPoint;
        }
        public static RevitLine ToRevit(this Line line)
        {
            var start = new RevitXYZ(line.Start.X, line.Start.Y, line.Start.Z);
            var end = new RevitXYZ(line.End.X, line.End.Y, line.End.Z);

            RevitLine revitLine = RevitLine.CreateBound(start, end);
            return revitLine;
        }
    }
}
