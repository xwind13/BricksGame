using BricksGame.Logic.Models;

namespace BricksGame.Logic.StateManagers
{
    public class ScoreStateManager : StateManagerBase<Score>, IStateManager
    {
        public ScoreStateManager(Score score, int maxSavedStatesCount) : base(score, maxSavedStatesCount) {}

        protected override Score CloneStateObj()
        {
            return (Score)_stateObj.Clone();
        }

        protected override void CopyState(Score state)
        {
            _stateObj.CopyState(state);
        }
    }
}
