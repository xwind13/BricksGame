using BricksGame.Logic.StateManagers;
using System;

namespace BricksGame.Logic.Models
{
    public class Score : IStateCopyable<Score>
    {
        private int _winScore;

        public int _value;
        public int Value 
        { 
            get { return _value; }
            private set
            {
                _value = value;
                ScoreUpdated?.Invoke(_value);
            } 
        }

        public event Action<int> ScoreUpdated; 

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
