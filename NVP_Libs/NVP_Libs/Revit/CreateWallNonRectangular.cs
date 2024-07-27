using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System.Collections.Generic;
using System.Linq;

using RevitLine = Autodesk.Revit.DB.Line;
using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit
{
    [NodeInput("профиль", typeof(List<XYZ>))]
    [NodeInput("тип стены", typeof(string))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("несущая", typeof(bool))]
    internal class CreateWallNonRectangular : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var points = (inputs[0].Value as IEnumerable<object>).Cast<XYZ>().ToList();
            var wallTypeName = (string)inputs[1].Value;
            var levelId = (inputs[2].Value as Element).Id;
            var structural = (bool)inputs[3].Value;
            var revitPoints = new List<RevitXYZ>();
            IList<Curve> profile = new List<Curve>();

            foreach (XYZ point in points)
            {
                RevitXYZ revitPoint = point.ToRevit();
                revitPoints.Add(revitPoint);
            }
            for (int i = 0; i < revitPoints.Count; i++)
            {
                int nextIndex = (i + 1) % revitPoints.Count;
                RevitLine line = RevitLine.CreateBound(revitPoints[i], revitPoints[nextIndex]);
                profile.Add(line);
            }

            WallType wallType = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .OfType<WallType>()
                .FirstOrDefault(f => f.Name == wallTypeName);
            var wallTypeId = wallType.Id;

            using (Transaction transaction = new Transaction(doc, "Создание стены"))
            {
                transaction.Start();
                Wall wall = Wall.Create(doc, profile, wallTypeId, levelId, structural);
                transaction.Commit();
                return new NodeResult(wall);
            }
        }
    }
}
