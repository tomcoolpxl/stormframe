using System;
using System.Linq;
using NUnit.Framework;
using Stormframe.Construction;
using UnityEngine;

namespace Stormframe.Tests
{
    public sealed class ConstructionLinePlannerTests
    {
        [Test]
        public void CreateLine_UsesOnlyTheLockedAxis()
        {
            ConstructionAssembly cube = SinglePiece(PieceKind.Cube);

            ConstructionAssembly line = ConstructionLinePlanner.CreateLine(
                cube,
                new Vector3Int(3, 0, 4),
                0,
                ConstructionAxis.Z,
                3);

            Assert.That(line.PieceCount, Is.EqualTo(4));
            Assert.That(
                line.GetPlacements(Vector3Int.zero, 0).Select(piece => piece.Anchor),
                Is.EquivalentTo(new[]
                {
                    new Vector3Int(3, 0, 4), new Vector3Int(3, 0, 5),
                    new Vector3Int(3, 0, 6), new Vector3Int(3, 0, 7)
                }));
        }

        [Test]
        public void CreateLine_VerticalCubesDoNotOverlap()
        {
            ConstructionAssembly cube = SinglePiece(PieceKind.Cube);

            ConstructionAssembly line = ConstructionLinePlanner.CreateLine(
                cube,
                Vector3Int.zero,
                0,
                ConstructionAxis.Y,
                2);

            Assert.That(
                line.GetPlacements(Vector3Int.zero, 0).Select(piece => piece.Anchor.y),
                Is.EquivalentTo(new[] { 0, 2, 4 }));
            Assert.That(line.CanPlace(new ConstructionWorld(), Vector3Int.zero, 0), Is.True);
        }

        [Test]
        public void CreateLine_SpacesBeamByItsRotatedFootprint()
        {
            ConstructionAssembly beam = SinglePiece(PieceKind.Beam);

            ConstructionAssembly line = ConstructionLinePlanner.CreateLine(
                beam,
                Vector3Int.zero,
                0,
                ConstructionAxis.X,
                1);

            Assert.That(
                line.GetPlacements(Vector3Int.zero, 0).Select(piece => piece.Anchor.x),
                Is.EquivalentTo(new[] { 0, 3 }));
            Assert.That(line.CanPlace(new ConstructionWorld(), Vector3Int.zero, 0), Is.True);
        }

        [Test]
        public void CreateLine_SupportsNegativeDragDirection()
        {
            ConstructionAssembly cube = SinglePiece(PieceKind.HalfBlock);

            ConstructionAssembly line = ConstructionLinePlanner.CreateLine(
                cube,
                Vector3Int.zero,
                0,
                ConstructionAxis.X,
                -2);

            Assert.That(
                line.GetPlacements(Vector3Int.zero, 0).Select(piece => piece.Anchor.x),
                Is.EquivalentTo(new[] { 0, -1, -2 }));
        }

        private static ConstructionAssembly SinglePiece(PieceKind kind)
        {
            return new ConstructionAssembly(
                new[] { new PlacedPiece(Guid.Empty, kind, Vector3Int.zero, 0) },
                Vector3Int.zero);
        }
    }
}
