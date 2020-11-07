using System;

namespace BricksGame.Logic.StateManagers
{
    public interface IStateCopyable<T> : ICloneable
    {
        public void CopyState(T other);
    }
}
