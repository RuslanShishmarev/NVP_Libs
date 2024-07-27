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
    [NodeInput("тип перекрытия", typeof(string))]
    [NodeInput("уровень", typeof(Level))]
    internal class CreateFloor : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var points = (inputs[0].Value as IEnumerable<object>).Cast<XYZ>().ToList();
            var floorTypeName = (string)inputs[1].Value;
            var levelId = (inputs[2].Value as Element).Id;
            var revitPoints = new List<RevitXYZ>();

            foreach (XYZ point in points)
            {
                RevitXYZ revitPoint = point.ToRevit();
                revitPoints.Add(revitPoint);
            }
            CurveLoop curveLoop = new CurveLoop();
            for (int i = 0; i < revitPoints.Count; i++)
            {
                int nextIndex = (i + 1) % revitPoints.Count;
                RevitLine line = RevitLine.CreateBound(revitPoints[i], revitPoints[nextIndex]);
                curveLoop.Append(line);
            }
            var profile = new List<CurveLoop> { curveLoop };

            FloorType floorType = new FilteredElementCollector(doc)
                .OfClass(typeof(FloorType))
                .OfType<FloorType>()
                .FirstOrDefault(f => f.Name == floorTypeName);
            var floorTypeId = floorType.Id;

            using (Transaction transaction = new Transaction(doc, "Создание перекрытия"))
            {
                transaction.Start();
                Floor floor = Floor.Create(doc, profile, floorTypeId, levelId);
                transaction.Commit();
                return new NodeResult(floor);
            }
        }
    }
}
