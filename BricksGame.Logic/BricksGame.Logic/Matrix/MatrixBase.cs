using System;

namespace BricksGame.Logic.Matrix
{
    public abstract class MatrixBase<T> : IMatrix<T>
    {
        protected T[,] _items;

        public T this[uint x, uint y] => _items[x, y];

        public abstract uint Width { get; }

        public abstract uint Height { get; }

        public T Find(Func<T, uint, uint, bool> func)
        {
            for (uint x = 0; x < Width; x++)
            {
                for (uint y = 0; y < Height; y++)
                {
                    var item = _items[x, y];
                    if (func(item, x, y))
                        return item;
                }
            }

            return default;
        }

        public T Find(Func<T, bool> func) => Find((item, x, y) => func(item));

        public void ForEach(Action<T, uint, uint> action)
        {
            for (uint x = 0; x < Width; x++)
            {
                for (uint y = 0; y < Height; y++)
                {
                    action(_items[x, y], x, y);
                }
            }
        }

        public void ForEach(Action<T> action) => ForEach((item, x, y) => action(item));
    }
}
