using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stormframe.Construction.Commands
{
    public sealed class PlaceAssemblyCommand : IConstructionCommand
    {
        private readonly ConstructionAssembly _assembly;
        private readonly Vector3Int _origin;
        private readonly int _quarterTurns;
        private readonly List<PlacedPiece> _placed = new();

        public PlaceAssemblyCommand(ConstructionAssembly assembly, Vector3Int origin, int quarterTurns)
        {
            _assembly = assembly;
            _origin = origin;
            _quarterTurns = quarterTurns;
        }

        public bool Execute(ConstructionWorld world)
        {
            if (!_assembly.CanPlace(world, _origin, _quarterTurns)) return false;

            IReadOnlyList<AssemblyPlacement> placements = _assembly.GetPlacements(_origin, _quarterTurns);
            for (int index = 0; index < placements.Count; index++)
            {
                AssemblyPlacement placement = placements[index];
                Guid id = _placed.Count > index ? _placed[index].Id : Guid.NewGuid();
                if (!world.TryPlace(id, placement.Kind, placement.Anchor, placement.QuarterTurns, out PlacedPiece piece))
                {
                    RollBack(world, index);
                    return false;
                }

                if (_placed.Count <= index) _placed.Add(piece);
            }

            return true;
        }

        public void Undo(ConstructionWorld world)
        {
            foreach (PlacedPiece piece in _placed)
            {
                world.TryRemove(piece.Id, out _);
            }
        }

        private void RollBack(ConstructionWorld world, int placedCount)
        {
            for (int index = 0; index < placedCount; index++)
            {
                world.TryRemove(_placed[index].Id, out _);
            }
        }
    }
}
