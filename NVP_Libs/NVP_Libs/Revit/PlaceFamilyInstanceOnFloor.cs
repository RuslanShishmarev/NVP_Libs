using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit
{
    [NodeInput("элемент", typeof(Floor))]
    [NodeInput("верхняя грань", typeof(bool))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(XYZ))]
    [NodeInput("вектор", typeof(XYZ))]
    public class PlaceFamilyInstanceOnFloor : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var element = (Floor)inputs[0].Value;
            var side = (bool)inputs[1].Value;
            var familySymbol = (FamilySymbol)inputs[2].Value;
            var point = (XYZ)inputs[3].Value;
            var vector = (XYZ)inputs[4].Value;

            RevitXYZ revitPoint = ConvertNVPToRevit.ConvertXYZ(point);
            RevitXYZ revitVector = ConvertNVPToRevit.ConvertXYZ(vector);
            Options geometryOptions = new Options();
            geometryOptions.ComputeReferences = true;

            Solid elementGeometry = element.get_Geometry(geometryOptions).FirstOrDefault(it => it is Solid) as Solid;
            var elementFaces = elementGeometry.Faces;
            var z = RevitXYZ.BasisZ;

            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства на перекрытии"))
            {
                if (side)
                {
                    Face topFace = null;
                    foreach (Face face in elementFaces)
                    {
                        if (face.ComputeNormal(new UV()).AngleTo(z) < Math.Pow(10, -6))
                        {
                            topFace = face;
                            IntersectionResult result = topFace.Project(revitPoint);
                            if (result != null)
                            {
                                RevitXYZ pointProjection = result.XYZPoint;
                                transaction.Start();
                                var instance = doc.Create.NewFamilyInstance(topFace, pointProjection, revitVector, familySymbol);
                                transaction.Commit();
                                return new NodeResult(instance);
                            }
                            return null;
                        }
                        continue;
                    }
                    return null;
                }
                else
                {
                    Face bottomFace = null;
                    foreach (Face face in elementFaces)
                    {
                        if (Math.Abs(face.ComputeNormal(new UV()).AngleTo(z) - Math.PI) < Math.Pow(10, -6))
                        {
                            bottomFace = face;
                            IntersectionResult result = bottomFace.Project(revitPoint);
                            if (result != null)
                            {
                                RevitXYZ pointProjection = result.XYZPoint;
                                transaction.Start();
                                var instance = doc.Create.NewFamilyInstance(bottomFace, pointProjection, revitVector, familySymbol);
                                transaction.Commit();
                                return new NodeResult(instance);
                            }
                            return null;
                        }
                        continue;
                    }
                    return null;
                }
            }
        }
    }
}