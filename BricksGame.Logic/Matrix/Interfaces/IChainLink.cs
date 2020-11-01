using System;

namespace BricksGame.Logic.Matrix
{
    public interface IChainLink<T, TD> where TD : IConvertible
    {
        T Value { get; }

        IChainLink<T, TD> Next(TD direction);
    }
}
