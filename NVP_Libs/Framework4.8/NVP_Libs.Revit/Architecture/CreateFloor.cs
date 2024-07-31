using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System.Collections.Generic;
using System.Linq;

using RevitLine = Autodesk.Revit.DB.Line;
using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;


namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("профиль", typeof(List<XYZ>))]
    [NodeInput("тип перекрытия", typeof(string))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("несущая", typeof(bool))]
    internal class CreateFloor : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var points = (inputs[0].Value as IEnumerable<object>).Cast<XYZ>().ToList();
            var floorTypeName = (string)inputs[1].Value;
            var level = inputs[2].Value as Level;
            var structural = (bool)inputs[3].Value;
            var revitPoints = new List<RevitXYZ>();

            foreach (XYZ point in points)
            {
                RevitXYZ revitPoint = point.ToRevit();
                revitPoints.Add(revitPoint);
            }

            using (var curveLoop = new CurveArray())
            {
                for (int i = 0; i < revitPoints.Count; i++)
                {
                    int nextIndex = (i + 1) % revitPoints.Count;
                    RevitLine line = RevitLine.CreateBound(revitPoints[i], revitPoints[nextIndex]);
                    curveLoop.Append(line);
                }

                FloorType floorType = new FilteredElementCollector(doc)
                    .OfClass(typeof(FloorType))
                    .OfType<FloorType>()
                    .FirstOrDefault(f => f.Name == floorTypeName);

                using (Transaction transaction = new Transaction(doc, "Создание перекрытия"))
                {
                    transaction.Start();
                    var floor = doc.Create.NewFloor(curveLoop, floorType, level, structural);
                    transaction.Commit();
                    return new NodeResult(floor);
                }
            }
        }
    }
}
