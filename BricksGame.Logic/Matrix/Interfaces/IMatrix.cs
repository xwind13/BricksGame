using System;

namespace BricksGame.Logic.Matrix
{
    public interface IMatrix<T>
    {
        public uint Width { get; }

        public uint Height { get; }

        T this[uint x, uint y] { get; }

        void ForEach(Action<T, uint, uint> action);

        void ForEach(Action<T> action);

        T Find(Func<T, uint, uint, bool> func);

        T Find(Func<T, bool> func);
    }
}
