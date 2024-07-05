using Autodesk.Revit.DB;
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
    public class CreateWalls : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var doc = (commandData as ExternalCommandData).Application.ActiveUIDocument.Document;

            var wallName = (string)inputs[0].Value;
            var line = (Line)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            var height = (double)inputs[3].Value * 3.28084;

            var wallType = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .FirstOrDefault(w => w.Name == wallName);
            var wallTypeId = ((WallType)wallType).Id;

            var levelId = level.Id;


            using (Transaction transaction = new Transaction(doc, "Create Walls"))
            {
                transaction.Start();
                RevitLine revitLine = ConvertNVPToRevit.ConvertLine(line);
                Wall wall = Wall.Create(doc, revitLine, wallTypeId, levelId, height, 0, false, false);
                transaction.Commit();
                return new NodeResult(wall);
            }
        }
    }
}
