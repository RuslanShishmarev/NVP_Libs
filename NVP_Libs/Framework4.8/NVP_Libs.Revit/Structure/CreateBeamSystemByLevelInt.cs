using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("индекс линии", typeof(double))]
    [NodeInput("3D", typeof(bool))]
    public class CreateBeamSystemByLevelInt : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var profile = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var level = (Level)inputs[1].Value;
            var index = Convert.ToInt32((double)inputs[2].Value);
            var is3D = (bool)inputs[3].Value;

            using (Transaction transaction = new Transaction(doc, "Создание балочной системы по уровню и индексу"))
            {
                transaction.Start();
                BeamSystem beamSystem = BeamSystem.Create(doc, profile, level, index, is3D);
                transaction.Commit();
                return new NodeResult(beamSystem);
            }
        }
    }
}
