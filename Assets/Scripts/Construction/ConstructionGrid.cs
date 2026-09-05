using UnityEngine;

namespace Stormframe.Construction
{
    public static class ConstructionGrid
    {
        public const float CellSize = 1f;
        public const float VerticalStep = 0.5f;

        public static Vector3 CellToWorld(Vector3Int cell)
        {
            return new Vector3(
                cell.x * CellSize,
                cell.y * VerticalStep + 0.5f,
                cell.z * CellSize);
        }

        public static Vector3Int WorldToCell(Vector3 point)
        {
            return new Vector3Int(
                Mathf.RoundToInt(point.x / CellSize),
                Mathf.FloorToInt(point.y / VerticalStep),
                Mathf.RoundToInt(point.z / CellSize));
        }

        public static Vector3Int SurfaceToCell(Vector3 point, Vector3 normal)
        {
            return WorldToCell(point + normal * (VerticalStep * 0.5f + 0.01f));
        }
    }
}
