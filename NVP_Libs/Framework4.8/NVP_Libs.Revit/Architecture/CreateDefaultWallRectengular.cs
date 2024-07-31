using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System.Collections.Generic;

using Line = NVP.API.Geometry.Line;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("профиль", typeof(Line))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("несущая", typeof(bool))]
    internal class CreateDefaultWallRectengular : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var line = (Line)inputs[0].Value;
            var levelId = (inputs[1].Value as Element).Id;
            var structural = (bool)inputs[2].Value;
            var revitLine = line.ToRevit();

            using (Transaction transaction = new Transaction(doc, "Создание стены"))
            {
                transaction.Start();
                Wall wall = Wall.Create(doc, revitLine, levelId, structural);
                transaction.Commit();
                return new NodeResult(wall);
            }

        }
    }
}
