using System;
using System.Collections.Generic;
using System.Linq;

using nanoCAD;

using Multicad;
using Multicad.AplicationServices;
using Multicad.DatabaseServices;
using Multicad.DatabaseServices.StandardObjects;
using Multicad.Geometry;
using Multicad.CustomObjectBase;
using Multicad.Mc3D;
using Multicad.Architecture;
using Multicad.BimAccess;
using Multicad.BimControls;

using Line = NVP.API.Geometry.Line;

using HostMgd.Native;

using Teigha.DatabaseServices;

using System.Reflection.Emit;
using HostMgd.ApplicationServices;
using Multicad.Text;

public class CreateWall
{
    public static void Main()
    {
        // Получение текущего документа
        var doc = McDocumentsManager.GetCurrentDocument();
        var db = doc.Database;

        // Начало транзакции
        using (var trans = db.TransactionManager.StartTransaction())
        {
            // Получение пространственной модели
            var modelSpace = trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite) as BlockTableRecord;

            // Создание линии для стены
            var startPt = new Point3d(0, 0, 0);
            var endPt = new Point3d(5000, 0, 0); // Длина стены 5 метров

            LineSegment wallLine = new LineSegment(startPt, endPt);

            // Создание объекта стены
            var wall = new ArchWall();
            wall.Position = wallLine;
            wall.Width = 200; // Ширина стены 200 мм
            wall.Height = 3000; // Высота стены 3 метра

            // Добавление стены в пространственную модель
            modelSpace.AppendEntity(wall);
            trans.AddNewlyCreatedDBObject(wall, true);

            // Завершение транзакции
            trans.Commit();
        }
    }
}
