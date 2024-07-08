using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit
{
    [NodeInput("грань", typeof(Face))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(XYZ))]
    [NodeInput("вектор", typeof(XYZ))]
    public class PlaceFamilyInstanceOnPlane : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var face = (Face)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var point = (XYZ)inputs[2].Value;
            var vector = (XYZ)inputs[3].Value;
            RevitXYZ revitPoint = ConvertNVPToRevit.ConvertXYZ(point);
            RevitXYZ revitVector = ConvertNVPToRevit.ConvertXYZ(vector);
            
            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства на плоскости"))
            { 
                IntersectionResult result = face.Project(revitPoint);
                if (result != null) 
                {
                    RevitXYZ pointProjection = result.XYZPoint;
                    transaction.Start();
                    var instance = doc.Create.NewFamilyInstance(face, pointProjection, revitVector, familySymbol);
                    transaction.Commit();
                    return new NodeResult(instance);
                }
                return null;
            }
        }
    }
}