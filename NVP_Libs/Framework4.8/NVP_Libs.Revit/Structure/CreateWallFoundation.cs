using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;


namespace NVP_Libs.Revit.Structure
{
    [NodeInput("тип фундамента", typeof(string))]
    [NodeInput("стена", typeof(Wall))]
    public class CreateWallFoundation : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var foundationTypeName = (string)inputs[0].Value;
            var foundationType = new FilteredElementCollector(doc)
                .OfClass(typeof(WallFoundationType))
                .FirstOrDefault(f => f.Name == foundationTypeName);
            var foundationTypeId = foundationType.Id;
            var wall = inputs[1].Value as Element;
            var wallId = wall.Id;

            using (Transaction transaction = new Transaction(doc, "Создание ленточного фундамента"))
            {
                transaction.Start();
                WallFoundation wallFoundation = WallFoundation.Create(doc, foundationTypeId, wallId);
                transaction.Commit();
                return new NodeResult(wallFoundation);
            }
        }
    }
}
