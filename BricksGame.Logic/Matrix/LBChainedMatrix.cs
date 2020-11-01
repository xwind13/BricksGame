using System;
using System.ComponentModel;

namespace BricksGame.Logic.Matrix
{
    /// <summary>
    /// the class for a chained matrix with the coordinate system that starts in left-bottom angle 
    /// (it sets items left to right and then from bottom to top.)
    /// </summary>
    /// <typeparam name="T">type of an item</typeparam>
    public class LBChainedMatrix<T> : ChainedMatrix<T>
    {
        public LBChainedMatrix(IMatrix<T> matrix) : base(matrix) { }

        protected override void ChainLinks()
        {
            ForEach((chain, x, y) =>
            {
                var chainItem = chain as MatrixChainLink;

                chainItem.LinkTo(Direction.Left, (x != 0) ? _items[x - 1, y] : null);
                chainItem.LinkTo(Direction.Down, (y != 0) ? _items[x, y - 1] : null);
                chainItem.LinkTo(Direction.Right, (x != Width - 1) ? _items[x + 1, y] : null);
                chainItem.LinkTo(Direction.Up, (y != Height - 1) ? _items[x, y + 1] : null);
            });
        }

        public override uint GetLastIndexInDirection(Direction direction) => direction switch
        {
            Direction.Down => 0,
            Direction.Left => 0,
            Direction.Right => Width - 1,
            Direction.Up => Height - 1,

            _ => throw new InvalidEnumArgumentException($"{direction} is invalid argument")
        };
    }
}
