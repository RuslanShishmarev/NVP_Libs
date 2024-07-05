using System.Collections.Generic;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using RevitXYZ = Autodesk.Revit.DB.XYZ;

using NVP.API.Nodes;
using XYZ = NVP.API.Geometry.XYZ;
using NVP_Libs.Revit.Services;

namespace NVP_Libs.Revit
{
    [NodeInput("координата", typeof(XYZ))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("уровень", typeof(Level))]
    public class PlaceFamilyInstanceXYZ : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {         
            var doc = (commandData as ExternalCommandData).Application.ActiveUIDocument.Document;

            var point = (XYZ)inputs[0].Value;
            var family = (FamilySymbol)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            RevitXYZ revitPoint = ConvertNVPToRevit.ConvertXYZ(point);

            using (Transaction transaction = new Transaction(doc, "Place Family Instance By Point"))
            {
                transaction.Start();
                var instance = doc.Create.NewFamilyInstance(
                    revitPoint,
                    family,
                    level,
                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}
