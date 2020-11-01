using BricksGame.Logic.Matrix;
using System.Collections.Generic;

namespace BricksGame.Logic.StateManagers
{
    public class MatrixStateManager<T> : StateManagerBase<IMatrix<T>>, IStateManager where T: IStateCopyable<T>
    {
        public MatrixStateManager(IMatrix<T> matrix, int maxSavedStatesCount) : base(matrix, maxSavedStatesCount) {}

        protected override void CopyState(IMatrix<T> state)
        {
            _stateObj.ForEach((item, x, y) =>
            {
                var stateItem = state[x, y];
                item.CopyState(stateItem);
            });
        }

        protected override IMatrix<T> CloneStateObj()
        {
            var currentState = new T[_stateObj.Width, _stateObj.Height];
            _stateObj.ForEach((item, x, y) =>
            {
                currentState[x, y] = (T)item.Clone();
            });

            return new Matrix<T>(currentState);
        }
    }
}
