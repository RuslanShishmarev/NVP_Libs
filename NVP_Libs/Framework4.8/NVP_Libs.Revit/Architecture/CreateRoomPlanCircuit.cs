using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("уровень", typeof(Level))]
    internal class CreateRoomPlanCircuit : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;
            var level = (Level)inputs[0].Value;

            using (Transaction transaction = new Transaction(doc, "Создание помещения по контуру плана"))
            {
                transaction.Start();
                PlanCircuit planCircuit = null;
                PlanTopology planTopology = doc.get_PlanTopology(level);

                foreach (PlanCircuit circuit in planTopology.Circuits)
                {
                    if (circuit != null)
                    {
                        planCircuit = circuit;
                        break;
                    }
                }
                Room room = null;
                Room newRoom = doc.Create.NewRoom(room, planCircuit);
                transaction.Commit();
                return new NodeResult(newRoom);
            }
        }
    }
}
