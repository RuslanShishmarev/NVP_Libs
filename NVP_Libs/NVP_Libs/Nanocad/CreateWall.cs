using BIMStructureMgd.Common;
//Балки и перекрытия бетон
//Пространства имён nanoCAD BIM Конструкции
using BIMStructureMgd.DatabaseObjects;
using HostMgd.ApplicationServices;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

//Стандартные пространства имён платформы nanoCAD
using Teigha.DatabaseServices;
using Teigha.Geometry;

using NVPLine = NVP.API.Geometry.Line;
using NVPXYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Nanocad
{
    [NodeInput("тип стены", typeof(string))]
    [NodeInput("линия", typeof(NVPLine))]
    [NodeInput("базовая точка", typeof(NVPXYZ))]
    [NodeInput("высота", typeof(double))]
    [NodeInput("толщина", typeof(double))]
    public class CreateWall : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var wallTypeName = (string)inputs[0].Value;
            var line = (NVPLine)inputs[1].Value;
            var length = (double)inputs[2].Value;
            var height = (double)inputs[3].Value;
            var thickness = (double)inputs[4].Value;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            var request = LibraryRequest.CreateDatabaseRequest();
            request.AddCategoryCondition(LibraryObject.StructuralSurfaceCategory);
            request.AddCondition(LibraryObject.ObjectName, "=", wallTypeName);

            var wallObject = request.Execute().FirstOrDefault();
            var wall = StructuralSurface.Create(wallObject);
            wall.BasePoint = new Point3d(line.Start.X, line.Start.Y, line.Start.Z);
            wall.XDir = new Vector3d(line.Vector.X, line.Vector.Y, line.Vector.Z);
            wall.YDir = wall.ZDir.CrossProduct(wall.XDir);

            // Изменение размеров
            var wallData = wall.GetElementData();
            wallData.GetParameter("AEC_PART_LENGTH").Value = length.ToString();
            wallData.GetParameter("AEC_PART_THICKNESS").Value = thickness.ToString();
            wall.UpdateElements();

            using (Transaction tr = doc.Database.TransactionManager.StartTransaction()) 
            {
                Utilities.AddEntityToDatabase(db, tr, wall);
                tr.Commit();
            }

            return new NodeResult(wall);
        }
    }
}
