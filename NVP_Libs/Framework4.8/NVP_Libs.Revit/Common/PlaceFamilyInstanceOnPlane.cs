using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("грань", typeof(Reference))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(RevitXYZ))]
    [NodeInput("вектор", typeof(RevitXYZ))]
    public class PlaceFamilyInstanceOnPlane : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var faceReference = (Reference)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var point = (RevitXYZ)inputs[2].Value;
            var vector = (RevitXYZ)inputs[3].Value;

            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства на плоскости"))
            {
                transaction.Start();
                var instance = doc.Create.NewFamilyInstance(faceReference, point, vector, familySymbol);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}