using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using NVP.API.Geometry;
using NVP.API.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using RevitXYZ = Autodesk.Revit.DB.XYZ;
using RevitPlane = Autodesk.Revit.DB.Plane;

using XYZ = NVP.API.Geometry.XYZ;
using Plane = NVP.API.Geometry.Plane;
using System.Xml.Linq;
using NVP_Libs.Revit.Services;

namespace NVP_Libs.Revit
{
    [NodeInput("элемент", typeof(Element))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(XYZ))]
    [NodeInput("вектор", typeof(XYZ))]
    public class PlaceFamilyInstanceOnPlane : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var uiDoc = (commandData as ExternalCommandData).Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            var element = (Element)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var point = (XYZ)inputs[2].Value;
            var vector = (XYZ)inputs[3].Value;
            RevitXYZ revitPoint = ConvertNVPToRevit.ConvertXYZ(point);
            RevitXYZ revitVector = ConvertNVPToRevit.ConvertXYZ(vector);
            Options geometryOptions = new Options();
            geometryOptions.ComputeReferences = true;

            Solid elementGeometry = element.get_Geometry(geometryOptions).FirstOrDefault(it => it is Solid) as Solid;
            var elementFaces = elementGeometry.Faces;

            var minDistance = double.MaxValue;
            Face nearrestFace = null;

            using (Transaction transaction = new Transaction(doc, "Place Family Instance On Plane"))
            {
                foreach (Face face in elementFaces)
                {
                    IntersectionResult result = face.Project(revitPoint);
                    if (result != null)
                    {
                        var resultDistance = result.Distance;

                        if (resultDistance < minDistance)
                        {
                            minDistance = resultDistance;
                            nearrestFace = face;
                            continue;
                        }
                    }
                    continue;
                }
                transaction.Start();
                var instance = doc.Create.NewFamilyInstance(nearrestFace, revitPoint, revitVector, familySymbol);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}