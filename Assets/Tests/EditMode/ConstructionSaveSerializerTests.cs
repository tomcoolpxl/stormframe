using System.Linq;
using NUnit.Framework;
using Stormframe.Construction;
using Stormframe.Construction.Persistence;
using UnityEngine;

namespace Stormframe.Tests
{
    public sealed class ConstructionSaveSerializerTests
    {
        [Test]
        public void SerializeAndRestore_RebuildsLogicalWorld()
        {
            var source = new ConstructionWorld();
            source.TryPlace(PieceKind.Beam, new Vector3Int(2, 1, 3), 1, out PlacedPiece original);
            string json = ConstructionSaveSerializer.Serialize(source);
            var restored = new ConstructionWorld();

            Assert.That(ConstructionSaveSerializer.TryRestore(json, restored, out string error), Is.True, error);
            Assert.That(restored.PieceCount, Is.EqualTo(1));
            PlacedPiece restoredPiece = restored.Pieces.Single();
            Assert.That(restoredPiece.Id, Is.EqualTo(original.Id));
            Assert.That(restoredPiece.Kind, Is.EqualTo(PieceKind.Beam));
            Assert.That(restoredPiece.Anchor, Is.EqualTo(new Vector3Int(2, 1, 3)));
            Assert.That(restoredPiece.QuarterTurns, Is.EqualTo(1));
        }
    }
}
