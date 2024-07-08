using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit
{
    internal class GetElement : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            try
            {
                var uiDoc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument;
                var selection = uiDoc.Selection;
                var doc = uiDoc.Document;

                var elementReference = selection.PickObject(ObjectType.Element, "Выберите элемент");
                var element = doc.GetElement(elementReference);
                return new NodeResult(element);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return new NodeResult(null);
            }
        }
    }
}
