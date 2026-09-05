using System.Collections.Generic;

namespace Stormframe.Construction.Commands
{
    public sealed class ConstructionCommandHistory
    {
        private readonly Stack<IConstructionCommand> _undo = new();
        private readonly Stack<IConstructionCommand> _redo = new();

        public bool Execute(IConstructionCommand command, ConstructionWorld world)
        {
            if (!command.Execute(world)) return false;
            _undo.Push(command);
            _redo.Clear();
            return true;
        }

        public bool Undo(ConstructionWorld world)
        {
            if (!_undo.TryPop(out IConstructionCommand command)) return false;
            command.Undo(world);
            _redo.Push(command);
            return true;
        }

        public bool Redo(ConstructionWorld world)
        {
            if (!_redo.TryPop(out IConstructionCommand command) || !command.Execute(world)) return false;
            _undo.Push(command);
            return true;
        }

        public void Clear()
        {
            _undo.Clear();
            _redo.Clear();
        }
    }
}
