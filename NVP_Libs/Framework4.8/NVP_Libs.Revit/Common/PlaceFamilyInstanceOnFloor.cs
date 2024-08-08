using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;
using System.Linq;

using RevitXYZ = Autodesk.Revit.DB.XYZ;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("элемент", typeof(Floor))]
    [NodeInput("верхняя грань", typeof(bool))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(RevitXYZ))]
    [NodeInput("вектор", typeof(RevitXYZ))]
    public class PlaceFamilyInstanceOnFloor : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var element = (Floor)inputs[0].Value;
            var side = (bool)inputs[1].Value;
            var familySymbol = (FamilySymbol)inputs[2].Value;
            var point = (RevitXYZ)inputs[3].Value;
            var vector = (RevitXYZ)inputs[4].Value;

            Options geometryOptions = new Options();
            geometryOptions.ComputeReferences = true;

            Solid elementGeometry = element.get_Geometry(geometryOptions).FirstOrDefault(it => it is Solid) as Solid;
            var elementFaces = elementGeometry.Faces;

            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства на перекрытии"))
            {
                foreach (Face face in elementFaces)
                {
                    if (CheckAngle(side, face))
                    {
                        IntersectionResult result = face.Project(point);
                        if (result != null)
                        {
                            RevitXYZ pointProjection = result.XYZPoint;
                            transaction.Start();
                            var instance = doc.Create.NewFamilyInstance(face, pointProjection, vector, familySymbol);
                            transaction.Commit();
                            return new NodeResult(instance);
                        }
                        return new NodeResult(null);
                    }    
                }
                return new NodeResult(null);
            }
        }
        public bool CheckAngle(bool side, Face face)
        {
            var z = RevitXYZ.BasisZ;
            var zero = Math.Pow(10, -6);

            if (side)
            {
                return face.ComputeNormal(new UV()).AngleTo(z) < zero;
            }
            else
            {
                return Math.Abs(face.ComputeNormal(new UV()).AngleTo(z) - Math.PI) < zero;
            }
        }
    }
}