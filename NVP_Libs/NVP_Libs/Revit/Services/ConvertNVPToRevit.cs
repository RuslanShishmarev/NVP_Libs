using XYZ = NVP.API.Geometry.XYZ;
using RevitXYZ = Autodesk.Revit.DB.XYZ;
using RevitLine = Autodesk.Revit.DB.Line;
using NVP.API.Geometry;

namespace NVP_Libs.Revit.Services
{
    public class ConvertNVPToRevit
    {
        public static RevitXYZ ConvertXYZ(XYZ point)
        {
            RevitXYZ revitPoint = new RevitXYZ(point.X, point.Y, point.Z);
            return revitPoint;
        }
        public static RevitLine ConvertLine(Line line)
        {
            var start = new RevitXYZ(line.Start.X, line.Start.Y, line.Start.Z);
            var end = new RevitXYZ(line.End.X, line.End.Y, line.End.Z);

            RevitLine revitLine = RevitLine.CreateBound(start, end);
            return revitLine;
        }
    }
}
