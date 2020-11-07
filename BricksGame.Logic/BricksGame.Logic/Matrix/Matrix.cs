using System;

namespace BricksGame.Logic.Matrix
{
    public class Matrix<T> : MatrixBase<T>, IMatrix<T>
    {
        public override uint Width { get; }

        public override uint Height { get; }

        public Matrix(T[,] items)
        {
            Width = (uint)items.GetLength(0);
            Height = (uint)items.GetLength(1);

            _items = items;
        }
    }
}
