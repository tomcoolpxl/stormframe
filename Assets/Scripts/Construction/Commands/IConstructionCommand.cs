namespace Stormframe.Construction.Commands
{
    public interface IConstructionCommand
    {
        bool Execute(ConstructionWorld world);
        void Undo(ConstructionWorld world);
    }
}
