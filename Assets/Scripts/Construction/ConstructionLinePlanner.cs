using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stormframe.Construction
{
    public enum ConstructionAxis
    {
        X,
        Y,
        Z
    }

    public static class ConstructionLinePlanner
    {
        public static int GetStampSpacing(
            ConstructionAssembly source,
            int quarterTurns,
            ConstructionAxis axis)
        {
            List<Vector3Int> cells = OccupiedCells(source, Vector3Int.zero, quarterTurns).ToList();
            int minimum = cells.Min(cell => Component(cell, axis));
            int maximum = cells.Max(cell => Component(cell, axis));
            return maximum - minimum + 1;
        }

        public static ConstructionAssembly CreateLine(
            ConstructionAssembly source,
            Vector3Int start,
            int quarterTurns,
            ConstructionAxis axis,
            int signedSegments)
        {
            int spacing = GetStampSpacing(source, quarterTurns, axis);
            int direction = Math.Sign(signedSegments);
            int count = Math.Abs(signedSegments) + 1;
            Vector3Int step = AxisVector(axis) * spacing * direction;
            var pieces = new List<PlacedPiece>();

            for (int index = 0; index < count; index++)
            {
                Vector3Int stampOrigin = start + step * index;
                foreach (AssemblyPlacement placement in source.GetPlacements(stampOrigin, quarterTurns))
                {
                    pieces.Add(new PlacedPiece(
                        Guid.NewGuid(),
                        placement.Kind,
                        placement.Anchor,
                        placement.QuarterTurns));
                }
            }

            return new ConstructionAssembly(pieces, Vector3Int.zero);
        }

        public static Vector3Int AxisVector(ConstructionAxis axis)
        {
            return axis switch
            {
                ConstructionAxis.X => Vector3Int.right,
                ConstructionAxis.Y => Vector3Int.up,
                _ => Vector3Int.forward
            };
        }

        private static IEnumerable<Vector3Int> OccupiedCells(
            ConstructionAssembly source,
            Vector3Int origin,
            int quarterTurns)
        {
            foreach (AssemblyPlacement placement in source.GetPlacements(origin, quarterTurns))
            {
                foreach (Vector3Int cell in PieceGeometry.OccupiedCells(
                             placement.Kind,
                             placement.Anchor,
                             placement.QuarterTurns))
                {
                    yield return cell;
                }
            }
        }

        private static int Component(Vector3Int value, ConstructionAxis axis)
        {
            return axis switch
            {
                ConstructionAxis.X => value.x,
                ConstructionAxis.Y => value.y,
                _ => value.z
            };
        }
    }
}
