using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

using RevitXYZ = Autodesk.Revit.DB.XYZ;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("тип арматуры", typeof(RebarBarType))]
    [NodeInput("элемент", typeof(Element))]
    public class CreateRebarFreeFormCurveLoop : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var rebarBarType = (RebarBarType)inputs[1].Value;
            var host = (Element)inputs[2].Value;
            var validationResult = new RebarFreeFormValidationResult();
            List<CurveLoop> curveLoops = new List<CurveLoop>();

//Создаю объекты CurveLoop из полученных кривых и добавляю в список
            int startIndex = 0;
            List<int> adds = new List<int>();
            do
            {
                if (adds.Contains(startIndex))
                {
                    startIndex++;
                    continue;
                }
                RevitXYZ startPoint = curves[startIndex].GetEndPoint(0);
                RevitXYZ endPoint = curves[startIndex].GetEndPoint(1);
                CurveLoop curveLoop = new CurveLoop();
                curveLoop.Append(curves[startIndex]);
                adds.Add(startIndex);
                if (curves[startIndex].GetEndPoint(1).IsAlmostEqualTo(curves[startIndex].GetEndPoint(0)))
                {
                    curveLoops.Add(curveLoop);
                    startIndex++;
                    continue;
                }
                for (int j = startIndex + 1; j < curves.Count; j++)
                {
                    if (adds.Contains(j))
                        continue;
                    if (curves[j].GetEndPoint(0).IsAlmostEqualTo(endPoint))
                    {
                        curveLoop.Append(curves[j]);
                        adds.Add(j);
                        if(startPoint.IsAlmostEqualTo(curves[j].GetEndPoint(1)))
                        {
                            curveLoops.Add(curveLoop);
                            startIndex++;
                            break;
                        }
                        endPoint = curves[j].GetEndPoint(1);
                        j = startIndex;
                    }
                    if (j == curves.Count - 1)
                        throw new System.Exception("Некорректный профиль");
                }
            } while (startIndex < curves.Count);

            using (Transaction transaction = new Transaction(doc, "Создание арматуры свободной формы"))
            {
                transaction.Start();
                Rebar rebar = Rebar.CreateFreeForm(doc, rebarBarType, host, curveLoops, out validationResult);
                transaction.Commit();
                return new NodeResult(rebar);
            }
        }
    }
}
