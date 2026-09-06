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

        [Test]
        public void WallPanel_OccupiesTwoWideByTwoHigh()
        {
            var world = new ConstructionWorld();

            Assert.That(world.TryPlace(PieceKind.WallPanel, Vector3Int.zero, 0, out _), Is.True);
            Assert.That(world.CanPlace(PieceKind.Cube, Vector3Int.right, 0), Is.False);
            Assert.That(world.CanPlace(PieceKind.Cube, Vector3Int.up, 0), Is.False);
            Assert.That(world.CanPlace(PieceKind.Cube, Vector3Int.right + Vector3Int.up, 0), Is.False);
            Assert.That(world.CanPlace(PieceKind.Cube, Vector3Int.forward, 0), Is.True);
        }

        [Test]
        public void LongBlockRotation_ChangesOccupiedAxis()
        {
            var world = new ConstructionWorld();

            Assert.That(world.TryPlace(PieceKind.LongBlock, Vector3Int.zero, 1, out _), Is.True);
            Assert.That(world.CanPlace(PieceKind.Cube, Vector3Int.forward, 0), Is.False);
            Assert.That(world.CanPlace(PieceKind.Cube, Vector3Int.right, 0), Is.True);
        }

        [Test]
        public void HalfBlocks_StackWithinOneWorldUnit()
        {
            var world = new ConstructionWorld();

            Assert.That(world.TryPlace(PieceKind.HalfBlock, Vector3Int.zero, 0, out _), Is.True);
            Assert.That(world.TryPlace(PieceKind.HalfBlock, Vector3Int.up, 0, out _), Is.True);
            Assert.That(world.CanPlace(PieceKind.Cube, Vector3Int.zero, 0), Is.False);
            Assert.That(ConstructionGrid.CellToWorld(Vector3Int.up).y, Is.EqualTo(1f));
        }

        [Test]
        public void GetConnectedPieces_ReturnsTouchingComponentOnly()
        {
            var world = new ConstructionWorld();
            world.TryPlace(PieceKind.Cube, Vector3Int.zero, 0, out PlacedPiece first);
            world.TryPlace(PieceKind.Cube, Vector3Int.right, 0, out _);
            world.TryPlace(PieceKind.Cube, Vector3Int.right * 5, 0, out _);

            Assert.That(world.GetConnectedPieces(first.Id), Has.Count.EqualTo(2));
        }

        [Test]
        public void GetSupportedPieceIds_PropagatesSupportFromGround()
        {
            var world = new ConstructionWorld();
            world.TryPlace(PieceKind.Cube, Vector3Int.zero, 0, out PlacedPiece basePiece);
            world.TryPlace(PieceKind.Cube, Vector3Int.up * 2, 0, out PlacedPiece middlePiece);
            world.TryPlace(PieceKind.HalfBlock, Vector3Int.up * 4, 0, out PlacedPiece topPiece);

            var supported = world.GetSupportedPieceIds();

            Assert.That(supported, Does.Contain(basePiece.Id));
            Assert.That(supported, Does.Contain(middlePiece.Id));
            Assert.That(supported, Does.Contain(topPiece.Id));
            Assert.That(world.GetUnsupportedPieces(), Is.Empty);
        }

        [Test]
        public void GetUnsupportedPieces_IdentifiesDisconnectedStructure()
        {
            var world = new ConstructionWorld();
            world.TryPlace(PieceKind.Cube, Vector3Int.up * 4, 0, out PlacedPiece floating);

            Assert.That(world.GetUnsupportedPieces(), Has.Count.EqualTo(1));
            Assert.That(world.GetUnsupportedPieces()[0].Id, Is.EqualTo(floating.Id));
        }
    }
}
