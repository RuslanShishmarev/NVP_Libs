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
    [NodeInput("тип крыши", typeof(string))]
    [NodeInput("уровень", typeof(Level))]
    internal class CreateRoof : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var points = (inputs[0].Value as IEnumerable<object>).Cast<XYZ>().ToList();
            var roofTypeName = (string)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            var revitPoints = new List<RevitXYZ>();

            foreach (XYZ point in points)
            {
                RevitXYZ revitPoint = point.ToRevit();
                revitPoints.Add(revitPoint);
            }
            CurveArray footPrint = new CurveArray();
            for (int i = 0; i < revitPoints.Count; i++)
            {
                int nextIndex = (i + 1) % points.Count;
                RevitLine line = RevitLine.CreateBound(revitPoints[i], revitPoints[nextIndex]);
                footPrint.Append(line);
            }

            RoofType roofType = new FilteredElementCollector(doc)
                .OfClass(typeof(RoofType))
                .OfType<RoofType>()
                .FirstOrDefault(f => f.Name == roofTypeName);

            using (Transaction transaction = new Transaction(doc, "Создание крыши"))
            {
                transaction.Start();
                ModelCurveArray footPrintToModelCurveMapping = new ModelCurveArray();
                FootPrintRoof footPrintRoof = doc.Create.NewFootPrintRoof(footPrint, level, roofType, out footPrintToModelCurveMapping);
                transaction.Commit();
                return new NodeResult(footPrintRoof);
            }
        }
    }
}
