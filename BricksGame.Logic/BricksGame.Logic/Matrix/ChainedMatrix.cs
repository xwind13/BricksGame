using System;

namespace BricksGame.Logic.Matrix
{
    public abstract class ChainedMatrix<T> : MatrixBase<IChainLink<T, Direction>>
    {
        protected class MatrixChainLink : ChainLink<T, Direction>, IChainLink<T, Direction>
        {
            public MatrixChainLink(T value) : base(value) { }

            public void LinkTo(Direction direction, IChainLink<T, Direction> link)
            {
                _links[direction] = link;
            }
        }

        public ChainedMatrix(IMatrix<T> matrix)
        {
            Matrix = matrix;

            Initialize();
        }

        public IMatrix<T> Matrix { get; }

        public override uint Height => Matrix.Height;

        public override uint Width => Matrix.Width;

        private void Initialize()
        {
            _items = new IChainLink<T, Direction>[Width, Height];

            for (uint x = 0; x < Width; x++)
            {
                for (uint y = 0; y < Height; y++)
                {
                    _items[x, y] = new MatrixChainLink(Matrix[x,y]);
                }
            }

            ChainLinks();
        }

        protected abstract void ChainLinks();

        public abstract uint GetLastIndexInDirection(Direction direction);
    }
}
