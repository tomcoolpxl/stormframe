using System.Collections.Generic;
using UnityEngine;

namespace Stormframe.Construction
{
    public static class PieceGeometry
    {
        public static IEnumerable<Vector3Int> OccupiedCells(
            PieceKind kind,
            Vector3Int anchor,
            int quarterTurns)
        {
            bool alongZ = Mathf.Abs(quarterTurns) % 2 == 1;
            Vector3Int axis = alongZ ? new Vector3Int(0, 0, 1) : Vector3Int.right;
            var horizontalOffsets = new List<Vector3Int>();

            if (kind == PieceKind.Beam)
            {
                for (int offset = -1; offset <= 1; offset++)
                {
                    horizontalOffsets.Add(axis * offset);
                }
            }
            else if (kind == PieceKind.LongBlock || kind == PieceKind.WallPanel)
            {
                horizontalOffsets.Add(Vector3Int.zero);
                horizontalOffsets.Add(axis);
            }
            else if (kind == PieceKind.Plate)
            {
                horizontalOffsets.Add(Vector3Int.zero);
                horizontalOffsets.Add(Vector3Int.right);
                horizontalOffsets.Add(new Vector3Int(0, 0, 1));
                horizontalOffsets.Add(new Vector3Int(1, 0, 1));
            }
            else
            {
                horizontalOffsets.Add(Vector3Int.zero);
            }

            int verticalLayers = kind switch
            {
                PieceKind.HalfBlock => 1,
                PieceKind.Plate => 1,
                PieceKind.Pillar => 4,
                PieceKind.WallPanel => 4,
                PieceKind.Rod => 4,
                _ => 2
            };

            foreach (Vector3Int horizontalOffset in horizontalOffsets)
            {
                for (int layer = 0; layer < verticalLayers; layer++)
                {
                    yield return anchor + horizontalOffset + Vector3Int.up * layer;
                }
            }
        }

        public static Vector3 VisualOffset(PieceKind kind, int quarterTurns)
        {
            bool alongZ = Mathf.Abs(quarterTurns) % 2 == 1;
            Vector3 axis = alongZ ? Vector3.forward : Vector3.right;
            return kind switch
            {
                PieceKind.Plate => new Vector3(0.5f, -0.375f, 0.5f),
                PieceKind.HalfBlock => Vector3.down * 0.25f,
                PieceKind.LongBlock => axis * 0.5f,
                PieceKind.Pillar => Vector3.up * 0.5f,
                PieceKind.WallPanel => axis * 0.5f + Vector3.up * 0.5f,
                PieceKind.Rod => Vector3.up * 0.5f,
                _ => Vector3.zero
            };
        }
    }
}
