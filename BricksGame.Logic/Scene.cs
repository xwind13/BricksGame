﻿using BricksGame.Logic.Fields;
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

        public IMatrix<IMainFieldSquare> MainFieldMatrix => _matrixField.Matrix;

        public IMatrix<ISquare> GetSideMatrix(Side side) => _sideFields[side].ToReadonlyMatrix();

        public event Action GameOver;

        public event Action<IEnumerable<IMainFieldSquare>> CombinationDestroyed;

        public Scene(FieldSetting setting)
        {
            _matrixField = MainField.Create(setting);
            _sideFields = new Dictionary<Side, SideField>();

            _sideFields[Side.Top] = SideField.Create(setting, Side.Top);
            _sideFields[Side.Left] = SideField.Create(setting, Side.Left);
            _sideFields[Side.Bottom] = SideField.Create(setting, Side.Bottom);
            _sideFields[Side.Right] = SideField.Create(setting, Side.Right);

            _movingSquare = new MovingSquare();
            _movingSquare.MoveFinished += HandleMovingSquareMoveFinishedEvent;

            _score = new Score(setting);

            _stateManagers = new List<IStateManager>();
            InitStateManagersList();
        }

        private void HandleMovingSquareMoveFinishedEvent(MovingSquare ms)
        {
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
                (uint X, uint Y) point = dir.IsHorzOrient() ? (dest, ms.Y) : (ms.X, dest);
                _matrixField.SetSquareState(point.X, point.Y, ms.Color, ms.Direction);

                var combinations = _matrixField.GetSquareCombinations();
                foreach(var combination in combinations)
                {
                    _score.Add(combination.Count());
                    CombinationDestroyed?.Invoke(combination);

                    foreach (var square in combination)
                    {
                        _matrixField.ResetSquareState(square.X, square.Y);
                    }
                }

                var nextMoveSquare = _matrixField.FindSquareWithDestinationToMove();
                if (nextMoveSquare.Destination != -1)
                {
                    var square = nextMoveSquare.Square;
                    _movingSquare.Start((square.X, square.Y), square.Color, square.State.Direction.Value, (uint)nextMoveSquare.Destination);
                }
            }

            if (!_movingSquare.IsMoving)
            {
                if (!_matrixField.HasActiveSquares())
                {
                    _score.AddWinScore();
                    _matrixField.Reset();

                    ClearStateHistory();
                }
                else if(_matrixField.IsMoveDeadlock())
                {
                    GameOver?.Invoke();
                }
            }
        }

        public void BackToPreviousState()
        {
            _stateManagers.ForEach(sm => sm.BackToPrevious());
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

            var direction = new MoveDirection(side switch
            {
                Side.Top => Direction.Down,
                Side.Bottom => Direction.Up,
                Side.Left => Direction.Right,
                Side.Right => Direction.Left,

                _ => throw new InvalidEnumArgumentException($"{side} is invalid argument")
            });

            var dest = _matrixField.FindAvalibleDestinationInDirection(direction, lineIdx);
            if (dest == -1)
                return false;

            SaveState();

            var color = _sideFields[side].Pop(lineIdx);

            (uint X, uint Y) point = direction.IsHorzOrient() ? ((uint)dest, lineIdx) : (lineIdx, (uint)dest);

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