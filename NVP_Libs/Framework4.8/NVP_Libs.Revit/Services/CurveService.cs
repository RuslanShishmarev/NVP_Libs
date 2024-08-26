using Autodesk.Revit.DB;

using System;
using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;

namespace NVP_Libs.Revit.Services
{
    public static class CurveService
    {
        public static List<CurveLoop> CreateCurveLoops(List<Curve> curves)
        {
            List<CurveLoop> curveLoops = new List<CurveLoop>();

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
                        if (startPoint.IsAlmostEqualTo(curves[j].GetEndPoint(1)))
                        {
                            curveLoops.Add(curveLoop);
                            startIndex++;
                            break;
                        }
                        endPoint = curves[j].GetEndPoint(1);
                        j = startIndex;
                    }
                    if (j == curves.Count - 1)
                        throw new Exception("Некорректный профиль");
                }
            } while (startIndex < curves.Count);
            return curveLoops;
        }
    }
}
