using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("точка", typeof(XYZ))]
    [NodeInput("уровень", typeof(Level))]
    internal class CreateRoomByLevel : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var point = (XYZ)inputs[0].Value;
            var level = (Level)inputs[1].Value;
            RevitXYZ revitPoint = point.ToRevit();
            UV uvPoint = new UV();
            double distance = 0;
            RevitXYZ origin = new RevitXYZ(0, 0, level.Elevation);
            RevitXYZ normal = new RevitXYZ(0, 0, 1);

            Plane levelPlane = Plane.CreateByNormalAndOrigin(normal, origin);
            levelPlane.Project(revitPoint,out uvPoint,out distance);

            using (Transaction transaction = new Transaction(doc, "Создание помещения по точке"))
            {
                transaction.Start();
                Room room = doc.Create.NewRoom(level, uvPoint);
                transaction.Commit();
                return new NodeResult(room);
            }
        }
    }
}
