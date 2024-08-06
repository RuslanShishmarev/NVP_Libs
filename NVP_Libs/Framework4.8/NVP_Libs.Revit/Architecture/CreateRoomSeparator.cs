using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("кривые", typeof(List<Curve>))]
    [NodeInput("название эскиза", typeof(string))]
    [NodeInput("название вида", typeof(string))]
    internal class CreateRoomSeparator : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;
            CurveArray curveArray = new CurveArray();

            var sketchPlaneName = (string)inputs[1].Value;
            var sketchPlane = new FilteredElementCollector(doc)
                .OfClass(typeof(SketchPlane))
                .Cast<SketchPlane>()
                .FirstOrDefault(sp => sp.Name == sketchPlaneName);
            var viewName = (string)inputs[2].Value;
            var view = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .FirstOrDefault(v => v.Name == viewName);

            var inputType = inputs[0].ValueType;
            if (inputType == typeof(Curve)) 
            {
                var curve = (Curve)inputs[0].Value;
                curveArray.Append(curve);
            }
            else
            {
                var curves = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
                for (int i = 0; i < curves.Count; i++)
                {
                    curveArray.Append(curves[i]);
                }
            }

            using (Transaction transaction = new Transaction(doc, "Создание разделителя помещений"))
            {
                transaction.Start();
                ModelCurveArray roomSeparators = doc.Create.NewRoomBoundaryLines(sketchPlane, curveArray, view);
                transaction.Commit();
                return new NodeResult(roomSeparators);
            }
        }
    }
}
