using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stormframe.Construction
{
    public sealed class ConstructionAssembly
    {
        private readonly List<AssemblyPiece> _pieces;

        public ConstructionAssembly(IEnumerable<PlacedPiece> pieces, Vector3Int origin)
        {
            _pieces = pieces
                .Select(piece => new AssemblyPiece(piece.Kind, piece.Anchor - origin, piece.QuarterTurns))
                .ToList();
        }

        public int PieceCount => _pieces.Count;

        public IReadOnlyList<AssemblyPlacement> GetPlacements(Vector3Int origin, int quarterTurns)
        {
            var placements = new List<AssemblyPlacement>(_pieces.Count);
            foreach (AssemblyPiece piece in _pieces)
            {
                HashSet<Vector3Int> desiredCells = PieceGeometry
                    .OccupiedCells(piece.Kind, piece.AnchorOffset, piece.QuarterTurns)
                    .Select(cell => origin + Rotate(cell, quarterTurns))
                    .ToHashSet();
                int rotatedTurns = piece.QuarterTurns + quarterTurns;
                Vector3Int resolvedAnchor = ResolveAnchor(piece.Kind, rotatedTurns, desiredCells);
                placements.Add(new AssemblyPlacement(piece.Kind, resolvedAnchor, rotatedTurns));
            }

            return placements;
        }

        public bool CanPlace(ConstructionWorld world, Vector3Int origin, int quarterTurns)
        {
            var occupied = new HashSet<Vector3Int>();
            foreach (AssemblyPlacement placement in GetPlacements(origin, quarterTurns))
            {
                if (!world.CanPlace(placement.Kind, placement.Anchor, placement.QuarterTurns)) return false;
                foreach (Vector3Int cell in PieceGeometry.OccupiedCells(
                             placement.Kind,
                             placement.Anchor,
                             placement.QuarterTurns))
                {
                    if (!occupied.Add(cell)) return false;
                }
            }

            return true;
        }

        private static Vector3Int ResolveAnchor(
            PieceKind kind,
            int quarterTurns,
            HashSet<Vector3Int> desiredCells)
        {
            foreach (Vector3Int candidate in desiredCells)
            {
                if (PieceGeometry.OccupiedCells(kind, candidate, quarterTurns).ToHashSet().SetEquals(desiredCells))
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException($"Could not rotate {kind} assembly piece.");
        }

        private static Vector3Int Rotate(Vector3Int value, int quarterTurns)
        {
            int turns = ((quarterTurns % 4) + 4) % 4;
            for (int index = 0; index < turns; index++)
            {
                value = new Vector3Int(-value.z, value.y, value.x);
            }

            return value;
        }

        private readonly struct AssemblyPiece
        {
            public AssemblyPiece(PieceKind kind, Vector3Int anchorOffset, int quarterTurns)
            {
                Kind = kind;
                AnchorOffset = anchorOffset;
                QuarterTurns = quarterTurns;
            }

            public PieceKind Kind { get; }
            public Vector3Int AnchorOffset { get; }
            public int QuarterTurns { get; }
        }
    }

    public readonly struct AssemblyPlacement
    {
        public AssemblyPlacement(PieceKind kind, Vector3Int anchor, int quarterTurns)
        {
            Kind = kind;
            Anchor = anchor;
            QuarterTurns = ((quarterTurns % 4) + 4) % 4;
        }

        public PieceKind Kind { get; }
        public Vector3Int Anchor { get; }
        public int QuarterTurns { get; }
    }
}
