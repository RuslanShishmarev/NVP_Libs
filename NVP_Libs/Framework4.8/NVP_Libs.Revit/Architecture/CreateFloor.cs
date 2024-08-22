using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using RevitLine = Autodesk.Revit.DB.Line;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("тип перекрытия", typeof(FloorType))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("несущее", typeof(bool))]
    [NodeInput("линия наклона", typeof(RevitLine))]
    [NodeInput("угол наклона", typeof(double))]
    public class CreateFloor : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var floorType = (FloorType)inputs[1].Value;
            var floorTypeId = floorType.Id;
            var levelId = (inputs[2].Value as Element).Id;
            var structural = (bool)inputs[3].Value;
            var slopeArrow = (RevitLine)inputs[4].Value;
            var slope = (double)inputs[5].Value;
            List<CurveLoop> profile = new List<CurveLoop>();

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
                    profile.Add(curveLoop);
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
                        if (startPoint.IsAlmostEqualTo(curves[j].GetEndPoint(1)))
                        {
                            profile.Add(curveLoop);
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

            using (Transaction transaction = new Transaction(doc, "Создание перекрытия"))
            {
                transaction.Start();
                Floor floor = Floor.Create(doc, profile, floorTypeId, levelId, structural, slopeArrow, slope);
                transaction.Commit();
                return new NodeResult(floor);
            }
        }
    }
}
