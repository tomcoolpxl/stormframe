using System;
using UnityEngine;

namespace Stormframe.Construction
{
    public sealed class PlacedPiece
    {
        public PlacedPiece(Guid id, PieceKind kind, Vector3Int anchor, int quarterTurns)
        {
            Id = id;
            Kind = kind;
            Anchor = anchor;
            QuarterTurns = ((quarterTurns % 4) + 4) % 4;
        }

        public Guid Id { get; }
        public PieceKind Kind { get; }
        public Vector3Int Anchor { get; }
        public int QuarterTurns { get; }
    }
}
