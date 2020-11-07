namespace BricksGame.Logic.StateManagers
{
    public interface IStateManager
    {
        void Save();

        void Clear();

        bool BackToPrevious();
    }
}