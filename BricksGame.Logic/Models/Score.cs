using BricksGame.Logic.StateManagers;

namespace BricksGame.Logic.Models
{
    public class Score : IStateCopyable<Score>
    {
        private int _winScore;

        public int Value { get; private set; }

        public IStateManager StateManager { get; }

        public Score(FieldSetting setting)
        {
            _winScore = setting.WinScore;

            StateManager = new ScoreStateManager(this, setting.MaxSavedStatesCount);
        }

        public void Add(int value)
        {
            Value += value;
        }

        public void AddWinScore()
        {
            Add(_winScore);
        }

        public void Reset()
        {
            Value = 0;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void CopyState(Score other)
        {
            _winScore = other._winScore;
            Value = other.Value;
        }
    }
}
