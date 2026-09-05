using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stormframe.Construction
{
    public sealed class ConstructionWorld
    {
        private readonly Dictionary<Guid, PlacedPiece> _pieces = new();
        private readonly Dictionary<Vector3Int, Guid> _occupancy = new();

        public int PieceCount => _pieces.Count;
        public IReadOnlyCollection<PlacedPiece> Pieces => _pieces.Values;

        public bool CanPlace(PieceKind kind, Vector3Int anchor, int quarterTurns)
        {
            foreach (Vector3Int cell in PieceGeometry.OccupiedCells(kind, anchor, quarterTurns))
            {
                if (cell.y < 0 || _occupancy.ContainsKey(cell))
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryPlace(
            PieceKind kind,
            Vector3Int anchor,
            int quarterTurns,
            out PlacedPiece piece)
        {
            return TryPlace(Guid.NewGuid(), kind, anchor, quarterTurns, out piece);
        }

        public bool TryPlace(
            Guid id,
            PieceKind kind,
            Vector3Int anchor,
            int quarterTurns,
            out PlacedPiece piece)
        {
            piece = null;
            if (_pieces.ContainsKey(id) || !CanPlace(kind, anchor, quarterTurns))
            {
                return false;
            }

            piece = new PlacedPiece(id, kind, anchor, quarterTurns);
            _pieces.Add(piece.Id, piece);
            foreach (Vector3Int cell in PieceGeometry.OccupiedCells(kind, anchor, quarterTurns))
            {
                _occupancy.Add(cell, piece.Id);
            }

            return true;
        }

        public bool TryRemoveAt(Vector3Int cell, out PlacedPiece piece)
        {
            piece = null;
            return _occupancy.TryGetValue(cell, out Guid id) && TryRemove(id, out piece);
        }

        public bool TryRemove(Guid id, out PlacedPiece piece)
        {
            if (!_pieces.Remove(id, out piece))
            {
                return false;
            }

            foreach (Vector3Int occupiedCell in PieceGeometry.OccupiedCells(
                         piece.Kind,
                         piece.Anchor,
                         piece.QuarterTurns))
            {
                _occupancy.Remove(occupiedCell);
            }

            return true;
        }

        public void Clear()
        {
            _pieces.Clear();
            _occupancy.Clear();
        }
    }
}
