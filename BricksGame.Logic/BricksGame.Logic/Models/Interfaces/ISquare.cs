using System;

namespace BricksGame.Logic
{
    public interface ISquare
    {
        uint X { get; }

        uint Y { get; }

        Color Color { get; }

        event Action<ISquare> StateChanged;
    }
}
