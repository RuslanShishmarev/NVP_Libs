﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

using RevitXYZ = Autodesk.Revit.DB.XYZ;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("направление", typeof(RevitXYZ))]
    [NodeInput("3D", typeof(bool))]
    public class CreateBeamSystem : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var profile = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var level = (Level)inputs[1].Value;
            var direction = (RevitXYZ)inputs[2].Value;
            var is3D = (bool)inputs[3].Value;

            using (Transaction transaction = new Transaction(doc, "Создание балочной системы"))
            {
                transaction.Start();
                BeamSystem beamSystem = BeamSystem.Create(doc, profile, level, direction, is3D);
                transaction.Commit();
                return new NodeResult(beamSystem);
            }
        }
    }
}
