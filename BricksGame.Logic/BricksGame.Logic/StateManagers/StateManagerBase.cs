using System.Collections.Generic;

namespace BricksGame.Logic.StateManagers
{
    public abstract class StateManagerBase<T> : IStateManager
    {
        protected readonly T _stateObj;

        private readonly LinkedList<T> _savedStates;

        private readonly int _maxSavedStatesCount;

        public StateManagerBase(T stateObj, int maxSavedStatesCount)
        {
            _stateObj = stateObj;
            _maxSavedStatesCount = maxSavedStatesCount;
            _savedStates = new LinkedList<T>();
        }

        public bool BackToPrevious()
        {
            if (_savedStates.Count == 0)
                return false;

            var state = _savedStates.First.Value;
            CopyState(state);

            _savedStates.RemoveFirst();

            return true;
        }

        protected abstract void CopyState(T state);

        public void Clear()
        {
            _savedStates.Clear();
        }

        public void Save()
        {
            _savedStates.AddFirst(CloneStateObj());

            if (_savedStates.Count > _maxSavedStatesCount)
                _savedStates.RemoveLast();
        }

        protected abstract T CloneStateObj();
    }
}
