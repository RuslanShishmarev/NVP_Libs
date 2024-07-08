using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit
{
    internal class GetXYZ : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            try
            {
                var uiDoc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument;
                var selection = uiDoc.Selection;

                var pickPoint = selection.PickPoint(ObjectSnapTypes.Nearest, "Выберите точку");
                var NVPPickPoint = context.CreatePoint(pickPoint.X, pickPoint.Y, pickPoint.Z);

                return new NodeResult(NVPPickPoint);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return new NodeResult(null);
            }
        }
    }
}
