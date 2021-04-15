using System;

namespace BricksGame.Logic
{
    public abstract class SquareBase : ISquare, ICloneable
    {
        public SquareBase(uint x, uint y) : this(x, y, Color.None) {}

        public SquareBase(uint x, uint y, Color color)
        {
            X = x; Y = y; Color = color;
        }

        public uint X { get; }

        public uint Y { get; }

        public Color Color { get; protected set; }

        public event Action<ISquare> StateChanged;

        protected void OnStateChanged()
        {
            StateChanged?.Invoke(this);
        }

        public virtual object Clone() => MemberwiseClone();
    }
}
