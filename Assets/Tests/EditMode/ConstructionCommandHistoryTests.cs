using NUnit.Framework;
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
    }
}
