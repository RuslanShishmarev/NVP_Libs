using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;


namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("контур", typeof(List<Curve>))]
    [NodeInput("тип перекрытия", typeof(string))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("несущая", typeof(bool))]
    public class CreateFloor : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var floorTypeName = (string)inputs[1].Value;
            var level = inputs[2].Value as Level;
            var structural = (bool)inputs[3].Value;
            CurveArray profile = new CurveArray();
            for (int i = 0; i < curves.Count; i++)
            {
                profile.Append(curves[i]);
            }

            FloorType floorType = new FilteredElementCollector(doc)
                .OfClass(typeof(FloorType))
                .OfType<FloorType>()
                .FirstOrDefault(f => f.Name == floorTypeName);

            using (Transaction transaction = new Transaction(doc, "Создание перекрытия"))
            {
                transaction.Start();
                var floor = doc.Create.NewFloor(profile, floorType, level, structural);
                transaction.Commit();
                return new NodeResult(floor);
            }

        }
    }
}
