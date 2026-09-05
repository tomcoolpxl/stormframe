using UnityEngine;

namespace Stormframe.Construction.Commands
{
    public sealed class PlacePieceCommand : IConstructionCommand
    {
        private readonly PieceKind _kind;
        private readonly Vector3Int _anchor;
        private readonly int _quarterTurns;
        private PlacedPiece _piece;

        public PlacePieceCommand(PieceKind kind, Vector3Int anchor, int quarterTurns)
        {
            _kind = kind;
            _anchor = anchor;
            _quarterTurns = quarterTurns;
        }

        public bool Execute(ConstructionWorld world)
        {
            return _piece == null
                ? world.TryPlace(_kind, _anchor, _quarterTurns, out _piece)
                : world.TryPlace(
                    _piece.Id,
                    _piece.Kind,
                    _piece.Anchor,
                    _piece.QuarterTurns,
                    out _);
        }

        public void Undo(ConstructionWorld world)
        {
            if (_piece != null) world.TryRemove(_piece.Id, out _);
        }
    }
}
