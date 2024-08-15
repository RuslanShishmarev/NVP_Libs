using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("эскиз", typeof(string))]
    [NodeInput("индекс линии", typeof(double))]

    internal class Create2DBeamSystem : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var profile = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var sketchPlaneName = (string)inputs[1].Value;
            var sketchPlane = new FilteredElementCollector(doc)
                .OfClass(typeof(SketchPlane))
                .Cast<SketchPlane>()
                .FirstOrDefault(sp => sp.Name == sketchPlaneName);
            var index = Convert.ToInt32((double)inputs[2].Value);

            using (Transaction transaction = new Transaction(doc, "Создание 2D балочной системы"))
            {
                transaction.Start();
                BeamSystem beamSystem = BeamSystem.Create(doc, profile, sketchPlane, index);
                transaction.Commit();
                return new NodeResult(beamSystem);
            }
        }
    }
}
