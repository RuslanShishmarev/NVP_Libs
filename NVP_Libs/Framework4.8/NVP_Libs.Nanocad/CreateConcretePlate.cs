using BIMStructureMgd.Common;
using BIMStructureMgd.DatabaseObjects;

using HostMgd.ApplicationServices;

using Teigha.DatabaseServices;
using Teigha.Geometry;

using System.Linq;
using System.Collections.Generic;

using NVP.API.Nodes;

using NVPXYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Nanocad
{
    [NodeInput("тип плиты", typeof(string))]
    [NodeInput("высота", typeof(double))]
    [NodeInput("толщина", typeof(double))]
    [NodeInput("точки плиты", typeof(List<NVPXYZ>))]
    public class CreateConcretePlate : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var plateTypeName = (string)inputs[0].Value;
            var height = (double)inputs[1].Value;
            var thickness = (double)inputs[2].Value;
            var points = (List<NVPXYZ>)inputs[3].Value;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            doc.Editor.WriteMessage("Создание плиты");

            var request = LibraryRequest.CreateDatabaseRequest();
            request.AddCategoryCondition(LibraryObject.StructuralSurfaceCategory);
            request.AddCondition(LibraryObject.ObjectName, "=", plateTypeName);

            var plateObject = request.Execute().FirstOrDefault();
            var platePoints = points.Select(p => new Point3d(p.X, p.Y, height)).ToArray();

            var plate = StructuralPlate.Create(platePoints);
            plate.Thickness = thickness;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Utilities.AddEntityToDatabase(db, tr, plate);
                tr.Commit();
            }

            return new NodeResult(plate);
        }
    }
}

