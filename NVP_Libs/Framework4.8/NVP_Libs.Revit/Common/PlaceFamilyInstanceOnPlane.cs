using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("грань", typeof(Reference))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(XYZ))]
    [NodeInput("вектор", typeof(XYZ))]
    public class PlaceFamilyInstanceOnPlane : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var faceReference = (Reference)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var point = (XYZ)inputs[2].Value;
            var vector = (XYZ)inputs[3].Value;

            RevitXYZ revitPoint = point.ToRevit();
            RevitXYZ revitVector = vector.ToRevit();

            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства на плоскости"))
            {
                transaction.Start();
                var instance = doc.Create.NewFamilyInstance(faceReference, revitPoint, revitVector, familySymbol);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}