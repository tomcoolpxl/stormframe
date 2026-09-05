using NUnit.Framework;
using Stormframe.Construction;
using UnityEngine;

namespace Stormframe.Tests
{
    public sealed class ConstructionWorldTests
    {
        [Test]
        public void TryPlace_RejectsOccupiedCell()
        {
            var world = new ConstructionWorld();

            Assert.That(world.TryPlace(PieceKind.Cube, Vector3Int.zero, 0, out _), Is.True);
            Assert.That(world.TryPlace(PieceKind.Cube, Vector3Int.zero, 0, out _), Is.False);
            Assert.That(world.PieceCount, Is.EqualTo(1));
        }

        [Test]
        public void BeamRotation_ChangesOccupiedAxis()
        {
            var world = new ConstructionWorld();

            Assert.That(world.TryPlace(PieceKind.Beam, Vector3Int.zero, 1, out _), Is.True);
            Assert.That(world.CanPlace(PieceKind.Cube, Vector3Int.right, 0), Is.True);
            Assert.That(world.CanPlace(PieceKind.Cube, new Vector3Int(0, 0, 1), 0), Is.False);
        }

        [Test]
        public void TryRemoveAt_RemovesEveryOccupiedCell()
        {
            var world = new ConstructionWorld();
            world.TryPlace(PieceKind.Plate, Vector3Int.zero, 0, out _);

            Assert.That(world.TryRemoveAt(new Vector3Int(1, 0, 1), out _), Is.True);
            Assert.That(world.PieceCount, Is.Zero);
            Assert.That(world.CanPlace(PieceKind.Plate, Vector3Int.zero, 0), Is.True);
        }

        [Test]
        public void CanPlace_RejectsCellsBelowGround()
        {
            var world = new ConstructionWorld();

            Assert.That(world.CanPlace(PieceKind.Cube, new Vector3Int(0, -1, 0), 0), Is.False);
        }
    }
}
