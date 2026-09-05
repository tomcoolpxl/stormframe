using System;
using UnityEngine;

namespace Stormframe.Construction
{
    public sealed class PlacedPieceView : MonoBehaviour
    {
        public Guid PieceId { get; private set; }
        public Vector3Int Anchor { get; private set; }

        public void Initialize(PlacedPiece piece)
        {
            PieceId = piece.Id;
            Anchor = piece.Anchor;
        }
    }
}
