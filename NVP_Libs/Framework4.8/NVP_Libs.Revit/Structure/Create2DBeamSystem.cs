using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;
using System.Linq;

using RevitXYZ = Autodesk.Revit.DB.XYZ;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("индекс направляющей", typeof(int))]
    public class Create2DBeamSystem : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var profile = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var index = Convert.ToInt32((double)inputs[1].Value);
            RevitXYZ point1 = profile[0].GetEndPoint(0);
            RevitXYZ point2 = profile[0].GetEndPoint(1);
            RevitXYZ point3 = profile[1].GetEndPoint(1);

            using (Transaction transaction = new Transaction(doc, "Создание 2D балочной системы по эскизу"))
            {
                transaction.Start();
                Plane plane = Plane.CreateByThreePoints(point1, point2, point3);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                BeamSystem beamSystem = BeamSystem.Create(doc, profile, sketchPlane, index);
                transaction.Commit();
                return new NodeResult(beamSystem);
            }
        }
    }
}
