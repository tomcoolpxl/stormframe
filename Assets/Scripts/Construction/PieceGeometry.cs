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

            if (kind == PieceKind.Beam)
            {
                for (int offset = -1; offset <= 1; offset++)
                {
                    yield return anchor + (alongZ
                        ? new Vector3Int(0, 0, offset)
                        : new Vector3Int(offset, 0, 0));
                }

                yield break;
            }

            if (kind == PieceKind.Plate)
            {
                yield return anchor;
                yield return anchor + Vector3Int.right;
                yield return anchor + new Vector3Int(0, 0, 1);
                yield return anchor + new Vector3Int(1, 0, 1);
                yield break;
            }

            yield return anchor;
        }

        public static Vector3 VisualOffset(PieceKind kind)
        {
            return kind == PieceKind.Plate
                ? new Vector3(0.5f, -0.375f, 0.5f)
                : Vector3.zero;
        }
    }
}
