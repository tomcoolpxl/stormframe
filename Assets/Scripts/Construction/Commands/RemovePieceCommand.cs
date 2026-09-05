using UnityEngine;

namespace Stormframe.Construction.Commands
{
    public sealed class RemovePieceCommand : IConstructionCommand
    {
        private readonly Vector3Int _cell;
        private PlacedPiece _piece;

        public RemovePieceCommand(Vector3Int cell)
        {
            _cell = cell;
        }

        public bool Execute(ConstructionWorld world)
        {
            return _piece == null
                ? world.TryRemoveAt(_cell, out _piece)
                : world.TryRemove(_piece.Id, out _);
        }

        public void Undo(ConstructionWorld world)
        {
            if (_piece == null) return;
            world.TryPlace(
                _piece.Id,
                _piece.Kind,
                _piece.Anchor,
                _piece.QuarterTurns,
                out _);
        }
    }
}
