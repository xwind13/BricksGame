using BricksGame.Logic.Fields;
using BricksGame.Logic.Matrix;
using BricksGame.Logic.Models;
using BricksGame.Logic.StateManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BricksGame.Logic
{
    public class Scene
    {
        private readonly MainField _matrixField;

        private readonly Dictionary<Side, SideField> _sideFields;

        private readonly MovingSquare _movingSquare;

        private readonly Score _score;

        private readonly List<IStateManager> _stateManagers;

        public MovingSquare MovingSquare => _movingSquare;

        public IMatrix<IMainFieldSquare> MainFieldMatrix => _matrixField.Matrix;

        public IMatrix<ISquare> GetSideMatrix(Side side) => _sideFields[side].ToReadonlyMatrix();

        public event Action GameOver;

        public event Action<IEnumerable<IMainFieldSquare>> CombinationDestroyed;

        public event Action<int> ScoreUpdated;

        public event Action GameWon;

        public Scene(FieldSetting setting)
        {
            _matrixField = MainField.Create(setting);
            _sideFields = new Dictionary<Side, SideField>
            {
                [Side.Top] = SideField.Create(setting, Side.Top),
                [Side.Left] = SideField.Create(setting, Side.Left),
                [Side.Bottom] = SideField.Create(setting, Side.Bottom),
                [Side.Right] = SideField.Create(setting, Side.Right)
            };

            _movingSquare = new MovingSquare();
            _movingSquare.StateChanged += HandleMovingSquareStateChangedEvent;

            _score = new Score(setting);
            _score.ScoreUpdated += HandleScoreUpdatedEvent;

            _stateManagers = new List<IStateManager>();
            InitStateManagersList();
        }

        private void HandleScoreUpdatedEvent(int score)
        {
            ScoreUpdated?.Invoke(score);
        }

        private void HandleMovingSquareStateChangedEvent(ISquare sq)
        {
            var ms = sq as MovingSquare;
            if (ms.IsMoving == true)
                return;

            var dir = new MoveDirection(ms.Direction);
            var dest = ms.Destination;

            var lastIndex = _matrixField.GetLastIndexInDirection(ms.Direction);

            if (dest == lastIndex)
            {
                var side = ms.Direction switch
                {
                    Direction.Down => Side.Bottom,
                    Direction.Up => Side.Top,
                    Direction.Left => Side.Left,
                    Direction.Right => Side.Right,

                    _ => throw new InvalidEnumArgumentException($"{ms.Direction} is invalid argument")
                };

                var lineIdx = dir.IsHorzOrient() ? ms.Y : ms.X;
                _sideFields[side].Push(lineIdx, ms.Color);
            }
            else
            {
                (uint x, uint y) = dir.IsHorzOrient() ? (dest, ms.Y) : (ms.X, dest);
                _matrixField.SetSquareState(x, y, ms.Color, ms.Direction);

                var combinations = _matrixField.GetSquareCombinations();
                foreach(var combination in combinations)
                {
                    _score.Add(combination.Count());
                    CombinationDestroyed?.Invoke(combination);

                    foreach (var item in combination)
                    {
                        _matrixField.ResetSquareState(item.X, item.Y);
                    }
                }
            }

            var (square, destination) = _matrixField.FindSquareWithDestinationToMove();
            if (destination != -1)
            {
                _movingSquare.Start((square.X, square.Y), square.Color, square.State.Direction.Value, (uint)destination);
                _matrixField.ResetSquareState(square.X, square.Y);
            }

            if (!_movingSquare.IsMoving)
            {
                if (!_matrixField.HasActiveSquares())
                {
                    _score.AddWinScore();
                    _matrixField.Reset();

                    GameWon?.Invoke();

                    //ClearStateHistory();
                }
                else if(_matrixField.IsMoveDeadlock())
                {
                    GameOver?.Invoke();
                }
            }
        }

        public bool BackToPreviousState()
        {
            // if there is any of state that returned to previous one.
            var result = false;
            _stateManagers.ForEach(sm => result |= sm.BackToPrevious());

            return result;
        }

        private void SaveState()
        {
            _stateManagers.ForEach(sm => sm.Save());
        }

        private void ClearStateHistory()
        {
            _stateManagers.ForEach(sm => sm.Clear());
        }

        public void Restart()
        {
            _score.Reset();
            _matrixField.Reset();

            foreach (var field in _sideFields.Values)
            {
                field.Generate();
            }

            ClearStateHistory();
        }

        public bool ThrowSquare(Side side, uint lineIdx)
        {
            if (_movingSquare.IsMoving)
                return false;

            var direction = _sideFields[side].ThrowDirection;

            var dest = _matrixField.FindAvalibleDestinationInDirection(direction, lineIdx);
            if (dest == -1)
                return false;

            SaveState();

            var color = _sideFields[side].Pop(lineIdx);

            var startPos = _matrixField.GetLastIndexInDirection(direction.Opposite);

            (uint X, uint Y) point = direction.IsHorzOrient() ? (startPos, lineIdx) : (lineIdx, startPos);

            _movingSquare.Start(point, color, direction.Value, (uint)dest);

            return true;
        }

        private void InitStateManagersList()
        {
            _stateManagers.Add(_score.StateManager);
            _stateManagers.Add(_matrixField.StateManager);
            foreach (var field in _sideFields.Values)
            {
                _stateManagers.Add(field.StateManager);
            }
        }
    }
}
