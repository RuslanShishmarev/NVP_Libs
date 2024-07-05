using System.Collections.Generic;
using System;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using RevitXYZ = Autodesk.Revit.DB.XYZ;

using NVP.API.Nodes;
using XYZ = NVP.API.Geometry.XYZ;
using System.Linq;
using NVP_Libs.Revit.Services;

namespace NVP_Libs.Revit
{
    [NodeInput("элемент", typeof(Element))]
    [NodeInput("верхняя грань", typeof(bool))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(XYZ))]
    [NodeInput("вектор", typeof(XYZ))]
    public class PlaceFamilyInstanceOnFloor : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var uiDoc = (commandData as ExternalCommandData).Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            var element = (Element)inputs[0].Value;
            var side = (bool)inputs[1].Value;
            var family = (FamilySymbol)inputs[2].Value;
            var point = (XYZ)inputs[3].Value;
            var vector = (XYZ)inputs[4].Value;
            RevitXYZ revitPoint = ConvertNVPToRevit.ConvertXYZ(point);
            RevitXYZ revitVector = ConvertNVPToRevit.ConvertXYZ(vector);
            Options geometryOptions = new Options();
            geometryOptions.ComputeReferences = true;

            Solid elementGeometry = element.get_Geometry(geometryOptions).FirstOrDefault(it => it is Solid) as Solid;
            var elementFaces = elementGeometry.Faces;
            var z = RevitXYZ.BasisZ;

            using (Transaction transaction = new Transaction(doc, "Place Family Instance On Floor"))
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
                                var instance = doc.Create.NewFamilyInstance(topFace, pointProjection, revitVector, family);
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
                                var instance = doc.Create.NewFamilyInstance(bottomFace, pointProjection, revitVector, family);
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