﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System.Collections.Generic;
using System.Linq;

using Line = NVP.API.Geometry.Line;
using RevitLine = Autodesk.Revit.DB.Line;

namespace NVP_Libs.Revit
{
    [NodeInput("тип стены", typeof(string))]
    [NodeInput("линия", typeof(Line))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("высота", typeof(double))]
    [NodeInput("внешняя сторона", typeof(bool))]
    [NodeInput("несущая", typeof(bool))]
    public class CreateWalls : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var wallName = (string)inputs[0].Value;
            var line = (Line)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            var height = (double)inputs[3].Value * 3.28084;
            var flip = (bool)inputs[4].Value;
            var structural = (bool)inputs[5].Value;

            var wallType = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .FirstOrDefault(w => w.Name.Equals(wallName));
            var wallTypeId = ((WallType)wallType).Id;
            var levelId = level.Id;

            using (Transaction transaction = new Transaction(doc, "Создание стены"))
            {
                transaction.Start();
                RevitLine revitLine = line.ToRevit();
                Wall wall = Wall.Create(doc, revitLine, wallTypeId, levelId, height, 0, flip, structural);
                transaction.Commit();
                return new NodeResult(wall);
            }
        }
    }
}
