using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("кривые", typeof(List<Curve>))]
    [NodeInput("название вида", typeof(string))]
    public class CreateRoomSeparator : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;
            CurveArray curveArray = new CurveArray();

            var viewName = (string)inputs[1].Value;
            var view = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .FirstOrDefault(v => v.Name == viewName);
            var sketchPlane = view.SketchPlane;

            var inputType = inputs[0].ValueType;
            if (inputType == typeof(Line) 
                || inputType == typeof(Arc) 
                || inputType == typeof(Curve) 
                || inputType == typeof(Ellipse) 
                || inputType == typeof(HermiteSpline) 
                || inputType == typeof(NurbSpline) 
                || inputType == typeof(CylindricalHelix)) 
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
