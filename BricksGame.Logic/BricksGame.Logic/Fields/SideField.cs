using BricksGame.Logic.Matrix;
using BricksGame.Logic.StateManagers;
using System;
using System.ComponentModel;
using System.Linq;

namespace BricksGame.Logic.Fields
{
    public class SideField : Matrix<ISquare>, IMatrix<ISquare>
    {
        private static readonly Random Random = new Random();

        private class SideFieldSquare : SquareBase, ISquare, IStateCopyable<SideFieldSquare>
        {
            public SideFieldSquare(uint x, uint y) : base(x, y) {}

            public void CopyState(SideFieldSquare other)
            {
                SetColor(other.Color);
            }

            public void SetColor(Color color) 
            { 
                Color = color;
                OnStateChanged();
            }
        }

        private readonly Lazy<Matrix<ISquare>> _lazyReadOnlyMatrix;

        public IStateManager StateManager { get; }

        ///<summary>
        /// Direction for thrown squares, it is always opposite to the location
        ///</summary>
        public MoveDirection ThrowDirection { get; private set; }

        protected SideField(ISquare[,] items, MoveDirection direction, int maxSavedStatesCount) : base(items) 
        {
            ThrowDirection = direction;

            StateManager = new MatrixStateManager<SideFieldSquare>(MatrixUpcast(), 
                maxSavedStatesCount);

            _lazyReadOnlyMatrix = new Lazy<Matrix<ISquare>>(() =>
            {
                var items = new ISquare[Width, Height];
                ForEach((item, x, y) => items[x, y] = item);

                return new Matrix<ISquare>(items);
            });

            Generate();
        }

        private IMatrix<SideFieldSquare> MatrixUpcast()
        {
            var items = new SideFieldSquare[this.Width, this.Height];
            this.ForEach((sqare, x, y) => items[x, y] = sqare as SideFieldSquare);

            return new Matrix<SideFieldSquare>(items);
        }

        ///<summary>
        /// Take a color square from the begin of the indicated position.
        /// It shifts other squares in line of this position and add a random color square 
        /// to the end of the line.
        ///</summary>
        public Color Pop(uint posIdx)
        {
            Color color = _items[posIdx, 0].Color;
            for (uint x = 0; x < Height - 1; x++)
            {
                ApplyColorToItem(_items[posIdx, x], _items[posIdx, x + 1].Color);
            }

            ApplyColorToItem(_items[posIdx, Height - 1], GetRandomColor());

            return color;
        }

        ///<summary>
        /// Put a color square to the indicated position. It shifts other squares in this position.
        ///</summary>
        public void Push(uint posIdx, Color color)
        {
            for (uint x = Height - 1; x > 0; x--)
            {
                ApplyColorToItem(_items[posIdx, x], _items[posIdx, x - 1].Color);
            }

            ApplyColorToItem(_items[posIdx, 0], color);
        }

        public void Generate()
        {
            ForEach((item) => ApplyColorToItem(item, GetRandomColor()));
        }

        public Matrix<ISquare> ToReadonlyMatrix()
        {
            return _lazyReadOnlyMatrix.Value;
        }

        private void ApplyColorToItem(ISquare item, Color color)
        {
            var square = item as SideFieldSquare;
            square.SetColor(color);
        }

        private Color GetRandomColor()
        {
            return (Color)Random.Next(1, Enum.GetNames(typeof(Color)).Count());
        }

        private static MoveDirection GetMoveDirectionFromLocation(Side location)
        {
            var direction = new MoveDirection(location switch
            {
                Side.Top => Direction.Down,
                Side.Bottom => Direction.Up,
                Side.Left => Direction.Right,
                Side.Right => Direction.Left,

                _ => throw new InvalidEnumArgumentException($"{location} is invalid argument")
            });

            return direction;
        }

        public static SideField Create(FieldSetting fieldSetting, Side location)
        {
            var direction = GetMoveDirectionFromLocation(location);

            var matrixDimension = direction.IsHorzOrient() ? 
                fieldSetting.VertDimension : 
                fieldSetting.HorzDimension;

            var sideDimension = fieldSetting.SideDimension;

            var items = new SideFieldSquare[matrixDimension, sideDimension];
            for (uint x = 0; x < matrixDimension; x++)
            {
                for (uint y = 0; y < sideDimension; y++)
                {
                    items[x, y] = new SideFieldSquare(x, y);
                }
            }

            return new SideField(items, direction, fieldSetting.MaxSavedStatesCount);
        }
    }
}
