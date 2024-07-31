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
    [NodeInput("несущая", typeof(bool))]
    internal class CreateDefaultWallNonRectangular : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var points = (inputs[0].Value as IEnumerable<object>).Cast<XYZ>().ToList();
            var structural = (bool)inputs[1].Value;
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


            using (Transaction transaction = new Transaction(doc, "Создание стены"))
            {
                transaction.Start();
                Wall wall = Wall.Create(doc, profile, structural);
                transaction.Commit();
                return new NodeResult(wall);
            }
        }
    }
}
