using BricksGame.Logic.Fields.Tools;
using BricksGame.Logic.Matrix;
using BricksGame.Logic.StateManagers;
using System.Collections.Generic;

namespace BricksGame.Logic.Fields
{
    public class MainField : LBChainedMatrix<IMainFieldSquare>
    {
        protected class MainFieldSquare : SquareBase, IMainFieldSquare, IStateCopyable<MainFieldSquare>
        {
            public MainFieldSquareState State { get; private set; }

            public MainFieldSquare(uint x, uint y) : base(x, y)
            {
                SetState(new MainFieldSquareState(false, 0, Direction.None), Color.None);
            }

            // color is base state!
            public void SetState(MainFieldSquareState state, Color color)
            {
                State = state; Color = color;
                OnStateChanged();
            }

            public void CopyState(MainFieldSquare other)
            {
                SetState(other.State, other.Color);
            }
        }

        private readonly Dictionary<uint, InitialSquare> _initialObstacles;

        public IStateManager StateManager { get; }

        protected MainField(Matrix<IMainFieldSquare> matrix, 
            List<InitialSquare> initialObstacles, int maxSavedStatesCount) : base(matrix) 
        {
            StateManager = new MatrixStateManager<MainFieldSquare>(Matrix as Matrix<MainFieldSquare>,
                maxSavedStatesCount);

            _initialObstacles = new Dictionary<uint, InitialSquare>();

            uint index = 0;
            initialObstacles.ForEach(square =>
            {
                _initialObstacles[++index] = square;

                var item = Matrix[square.X, square.Y];
                var state = new MainFieldSquareState(true, index, Direction.None);

                ApplyStateToItem(item, state, square.Color);
            });
        }

        public void ResetSquareState(uint x, uint y)
        {
            ResetItem(Matrix[x, y]);
        }

        public void SetSquareState(uint x, uint y, Color color, Direction direction)
        {
            if (color == Color.None)
                throw new GameException($"it is prohibited to set none as value for color");

            if (direction == Direction.None)
                throw new GameException($"it is prohibited to set none as value for direction");

            var item = this[x, y];

            var nextValue = item.Next(direction);
            if (nextValue == null)
                throw new GameException($"state ({color}, {direction}) cannot be set for the indicated square ({x}, {y})");

            if (!nextValue.Value.State.IsActive)
                throw new GameException($"state ({color}, {direction}) cannot be set for the indicated square ({x}, {y}) as it doesn't adjacent to anything.");

            var state = new MainFieldSquareState(true, nextValue.Value.State.Obstacle, direction);
            ApplyStateToItem(item.Value, state, color);
        }

        public void Reset()
        {
            ForEach(item => ResetItem(item.Value));
            foreach (var valuePair in _initialObstacles)
            {
                var initialSquare = valuePair.Value;

                var item = Matrix[initialSquare.X, initialSquare.Y];
                var state = new MainFieldSquareState(true, valuePair.Key, Direction.None);

                ApplyStateToItem(item, state, initialSquare.Color);
            }
        }

        /// <summary>
        /// get list of square combinations with 3 or more squares
        /// </summary>
        public IEnumerable<IEnumerable<IMainFieldSquare>> GetSquareCombinations() => 
            new SquareCombinationFinder(this).Find();

        public bool HasActiveSquares() =>
            Matrix.Find(square => square.State.IsActive) != null;

        /// <summary>
        /// check if it is possible to set state of square on any of lines.
        /// </summary>
        public bool IsMoveDeadlock()
        {
            var hasAvalibleDestinationsToMove = false;
            for (uint x = 0; x < Width && !hasAvalibleDestinationsToMove; x++)
            {
                hasAvalibleDestinationsToMove = FindAvalibleDestinationInDirection(new MoveDirection(Direction.Down), x) != -1;
            }

            for (uint x = 0; x < Width && !hasAvalibleDestinationsToMove; x++)
            {
                hasAvalibleDestinationsToMove = FindAvalibleDestinationInDirection(new MoveDirection(Direction.Up), x) != -1;
            }

            for (uint y = 0; y < Height && !hasAvalibleDestinationsToMove; y++)
            {
                hasAvalibleDestinationsToMove = FindAvalibleDestinationInDirection(new MoveDirection(Direction.Right), y) != -1;
            }

            for (uint y = 0; y < Height && !hasAvalibleDestinationsToMove; y++)
            {
                hasAvalibleDestinationsToMove = FindAvalibleDestinationInDirection(new MoveDirection(Direction.Left), y) != -1;
            }

            return !hasAvalibleDestinationsToMove;
        }

        /// <summary>
        /// find the first avalible destination index on a line in an indicated direction.
        /// the start of the search is the last index in opposite direction.
        /// </summary>
        public int FindAvalibleDestinationInDirection(MoveDirection direction, uint lineIdx)
        {
            var destination = -1;

            var startIndex = GetLastIndexInDirection(direction.Opposite);
            var square = direction.IsHorzOrient() ? this[startIndex, lineIdx] : this[lineIdx, startIndex];

            while (square != null  && !square.Value.State.IsActive)
            {
                destination = (int)(direction.IsHorzOrient() ? square.Value.X : square.Value.Y);
                square = square.Next(direction.Value);
            }
                
            return square != null ? destination : -1;
        }

        public (IMainFieldSquare square, int destination) FindSquareWithDestinationToMove()
        {
            var destinationIdx = -1;
            var value = Find((item) => HasDestinationToMove(item, out destinationIdx));

            return value != null ? (value.Value, destinationIdx) : (null, destinationIdx);
        }

        private bool HasDestinationToMove(IChainLink<IMainFieldSquare, Direction> item, out int destination)
        {
            destination = -1;

            if (!item.Value.State.IsActive || item.Value.State.Direction.IsNone())
                return false;

            var direction = item.Value.State.Direction;

            // look for an active square;
            var nextItem = item.Next(direction.Value);
            while (nextItem != null && !nextItem.Value.State.IsActive)
            {
                nextItem = nextItem.Next(direction.Value);
            }

            if (nextItem != null && nextItem != item.Next(direction.Value))
            {
                // destination is last inactive square (before first active square!) on the way. 
                var prevItem = nextItem.Next(direction.Opposite).Value;
                destination = direction.IsHorzOrient() ? (int)prevItem.X : (int)prevItem.Y;
            }
            else if (nextItem == null || CanIgnoreObstacles(item))
            {
                // if there aren't any obstacle or they can be ignored,
                // so destination is the last square on the way
                destination = (int)GetLastIndexInDirection(direction.Value);
            }
            
            return destination != -1;
        }

        /// <summary>
        /// Find out if squares on the way can be ignored.
        /// There are two consecutive conditions: 
        ///    1) all active squares on the way move opposite direction;
        ///    2) initial obstacle that the square adjacent to was destroyed and 
        ///       all squares on the way are adjacent to the same initial obstacle;
        ///    
        /// If the first case is false so the second one should be false as well to
        /// return negative result otherwise there is positive result.
        /// </summary>
        private bool CanIgnoreObstacles(IChainLink<IMainFieldSquare, Direction> item)
        {
            var direction = item.Value.State.Direction;
            var obstacle = item.Value.State.Obstacle;

            var nextItem = item.Next(direction.Value);

            var initialObstacle = _initialObstacles[item.Value.State.Obstacle];
            var obstaclesIgnored = !this[initialObstacle.X, initialObstacle.Y].Value.State.IsActive;

            while (nextItem != null)
            {
                var square = nextItem.Value;
                if (square.State.IsActive)
                {
                    obstaclesIgnored = obstaclesIgnored && square.State.Obstacle == obstacle;
                    if ((square.State.Direction.Value != direction.Opposite) && !obstaclesIgnored)
                        return false;
                }

                nextItem = nextItem.Next(direction.Value);
            }

            return true;
        }

        protected void ApplyStateToItem(IMainFieldSquare item, MainFieldSquareState state, Color color)
        {
            var square = item as MainFieldSquare;
            square.SetState(state, color);
        }

        private void ResetItem(IMainFieldSquare item)
        {
            ApplyStateToItem(item, new MainFieldSquareState(false, 0, Direction.None), Color.None);
        }

        public static MainField Create(FieldSetting fieldSetting)
        {
            var items = new MainFieldSquare[fieldSetting.HorzDimension, fieldSetting.VertDimension];
            for (uint x = 0; x < fieldSetting.HorzDimension; x++)
            {
                for (uint y = 0; y < fieldSetting.VertDimension; y++)
                {
                    items[x, y] = new MainFieldSquare(x, y);
                }
            }

            return new MainField(new Matrix<IMainFieldSquare>(items), fieldSetting.InitialSquares, 
                fieldSetting.MaxSavedStatesCount);
        } 
    }
}
