using UnityEngine;

namespace Stormframe.Construction
{
    public static class ConstructionGrid
    {
        public const float CellSize = 1f;

        public static Vector3 CellToWorld(Vector3Int cell)
        {
            return new Vector3(
                cell.x * CellSize,
                (cell.y + 0.5f) * CellSize,
                cell.z * CellSize);
        }

        public static Vector3Int WorldToCell(Vector3 point)
        {
            return new Vector3Int(
                Mathf.RoundToInt(point.x / CellSize),
                Mathf.FloorToInt(point.y / CellSize),
                Mathf.RoundToInt(point.z / CellSize));
        }
    }
}
