using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("контур", typeof(List<Curve>))]
    [NodeInput("тип крыши", typeof(RoofType))]
    [NodeInput("уровень", typeof(Level))]
    public class CreateRoof : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var roofType = (RoofType)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            CurveArray footPrint = new CurveArray();
            for (int i = 0; i < curves.Count; i++)
            {
                footPrint.Append(curves[i]);
            }

            using (Transaction transaction = new Transaction(doc, "Создание крыши"))
            {
                transaction.Start();
                ModelCurveArray footPrintToModelCurveMapping = new ModelCurveArray();
                FootPrintRoof footPrintRoof = doc.Create.NewFootPrintRoof(footPrint, level, roofType, out footPrintToModelCurveMapping);
                transaction.Commit();
                return new NodeResult(footPrintRoof);
            }
        }
    }
}
