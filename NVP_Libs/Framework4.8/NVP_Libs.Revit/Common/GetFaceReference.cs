using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit.Common
{
    public class GetFaceReference : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var uiDoc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument;
            var selection = uiDoc.Selection;

            var faceReference = selection.PickObject(ObjectType.Face, "Выберите плоскость");
            return new NodeResult(faceReference);
        }
    }
}
