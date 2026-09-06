using System.Collections.Generic;
using UnityEngine;

namespace Stormframe.Construction.Commands
{
    public sealed class DemolishPieceCommand : IConstructionCommand
    {
        private readonly Vector3Int _cell;
        private readonly List<PlacedPiece> _removed = new();

        public DemolishPieceCommand(Vector3Int cell)
        {
            _cell = cell;
        }

        public IReadOnlyList<PlacedPiece> CollapsedPieces =>
            _removed.Count <= 1 ? System.Array.Empty<PlacedPiece>() : _removed.GetRange(1, _removed.Count - 1);

        public bool Execute(ConstructionWorld world)
        {
            if (_removed.Count > 0)
            {
                if (!world.TryRemove(_removed[0].Id, out _)) return false;
                for (int index = 1; index < _removed.Count; index++)
                {
                    world.TryRemove(_removed[index].Id, out _);
                }

                return true;
            }

            if (!world.TryRemoveAt(_cell, out PlacedPiece removedPiece)) return false;
            _removed.Add(removedPiece);
            foreach (PlacedPiece unsupported in world.GetUnsupportedPieces())
            {
                if (!world.TryRemove(unsupported.Id, out PlacedPiece collapsed)) continue;
                _removed.Add(collapsed);
            }

            return true;
        }

        public void Undo(ConstructionWorld world)
        {
            foreach (PlacedPiece piece in _removed)
            {
                world.TryPlace(
                    piece.Id,
                    piece.Kind,
                    piece.Anchor,
                    piece.QuarterTurns,
                    out _);
            }
        }
    }
}
