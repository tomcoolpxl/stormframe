using NUnit.Framework;
using System.Linq;
using Stormframe.Construction;
using Stormframe.Construction.Commands;
using UnityEngine;

namespace Stormframe.Tests
{
    public sealed class ConstructionCommandHistoryTests
    {
        [Test]
        public void UndoAndRedo_RestorePlacedPieceWithStableIdentity()
        {
            var world = new ConstructionWorld();
            var history = new ConstructionCommandHistory();
            var command = new PlacePieceCommand(PieceKind.Cube, Vector3Int.zero, 0);

            Assert.That(history.Execute(command, world), Is.True);
            Assert.That(world.PieceCount, Is.EqualTo(1));
            Assert.That(history.Undo(world), Is.True);
            Assert.That(world.PieceCount, Is.Zero);
            Assert.That(history.Redo(world), Is.True);
            Assert.That(world.PieceCount, Is.EqualTo(1));
        }

        [Test]
        public void AssemblyCommand_StampsAndUndoesWholeConnectedGroup()
        {
            var source = new ConstructionWorld();
            source.TryPlace(PieceKind.Cube, Vector3Int.zero, 0, out PlacedPiece anchor);
            source.TryPlace(PieceKind.HalfBlock, Vector3Int.right, 0, out _);
            var assembly = new ConstructionAssembly(source.GetConnectedPieces(anchor.Id), anchor.Anchor);
            var target = new ConstructionWorld();
            var history = new ConstructionCommandHistory();

            Assert.That(
                history.Execute(new PlaceAssemblyCommand(assembly, new Vector3Int(4, 0, 0), 1), target),
                Is.True);
            Assert.That(target.PieceCount, Is.EqualTo(2));
            Assert.That(target.Pieces.Any(piece => piece.Anchor.z == 1), Is.True);
            Assert.That(history.Undo(target), Is.True);
            Assert.That(target.PieceCount, Is.Zero);
            Assert.That(history.Redo(target), Is.True);
            Assert.That(target.PieceCount, Is.EqualTo(2));
        }

        [Test]
        public void AssemblyCommand_WhenAnyCellIsBlocked_PlacesNothing()
        {
            var source = new ConstructionWorld();
            source.TryPlace(PieceKind.Cube, Vector3Int.zero, 0, out PlacedPiece anchor);
            source.TryPlace(PieceKind.Cube, Vector3Int.right, 0, out _);
            var assembly = new ConstructionAssembly(source.GetConnectedPieces(anchor.Id), anchor.Anchor);
            var target = new ConstructionWorld();
            target.TryPlace(PieceKind.Cube, new Vector3Int(5, 0, 0), 0, out _);

            bool placed = new PlaceAssemblyCommand(assembly, new Vector3Int(4, 0, 0), 0).Execute(target);

            Assert.That(placed, Is.False);
            Assert.That(target.PieceCount, Is.EqualTo(1));
        }

        [Test]
        public void AssemblyRotation_PreservesAsymmetricPlateFootprint()
        {
            var source = new ConstructionWorld();
            source.TryPlace(PieceKind.Plate, Vector3Int.zero, 0, out PlacedPiece plate);
            var assembly = new ConstructionAssembly(source.GetConnectedPieces(plate.Id), plate.Anchor);

            AssemblyPlacement placement = assembly.GetPlacements(new Vector3Int(10, 0, 10), 1).Single();

            Assert.That(
                PieceGeometry.OccupiedCells(placement.Kind, placement.Anchor, placement.QuarterTurns),
                Is.EquivalentTo(new[]
                {
                    new Vector3Int(10, 0, 10), new Vector3Int(10, 0, 11),
                    new Vector3Int(9, 0, 10), new Vector3Int(9, 0, 11)
                }));
        }

        [Test]
        public void DemolishPiece_RemovingBaseCollapsesUnsupportedStackAsOneCommand()
        {
            var world = new ConstructionWorld();
            var history = new ConstructionCommandHistory();
            world.TryPlace(PieceKind.Cube, Vector3Int.zero, 0, out _);
            world.TryPlace(PieceKind.Cube, Vector3Int.up * 2, 0, out _);
            world.TryPlace(PieceKind.HalfBlock, Vector3Int.up * 4, 0, out _);
            var command = new DemolishPieceCommand(Vector3Int.zero);

            Assert.That(history.Execute(command, world), Is.True);
            Assert.That(command.CollapsedPieces, Has.Count.EqualTo(2));
            Assert.That(world.PieceCount, Is.Zero);
            Assert.That(history.Undo(world), Is.True);
            Assert.That(world.PieceCount, Is.EqualTo(3));
            Assert.That(history.Redo(world), Is.True);
            Assert.That(world.PieceCount, Is.Zero);
        }

        [Test]
        public void DemolishPiece_RemovingTopLeavesSupportedBase()
        {
            var world = new ConstructionWorld();
            world.TryPlace(PieceKind.Cube, Vector3Int.zero, 0, out _);
            world.TryPlace(PieceKind.Cube, Vector3Int.up * 2, 0, out _);
            var command = new DemolishPieceCommand(Vector3Int.up * 2);

            Assert.That(command.Execute(world), Is.True);
            Assert.That(command.CollapsedPieces, Is.Empty);
            Assert.That(world.PieceCount, Is.EqualTo(1));
        }
    }
}
