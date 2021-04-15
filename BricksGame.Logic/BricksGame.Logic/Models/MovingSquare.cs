using System;

namespace BricksGame.Logic.Models
{
    public class MovingSquare : ISquare
    {
        public uint X { get; private set; }

        public uint Y { get; private set; }

        public Color Color { get; private set; }

        public Direction Direction { get; private set; }

        public uint Destination { get; private set; }

        public bool IsMoving { get; private set; }

        public event Action<ISquare> StateChanged;

        public bool Start((uint X, uint Y) point, Color clr, Direction dir, uint dest)
        {
            if (IsMoving)
                return false;

            X = point.X;
            Y = point.Y;
            Color = clr;
            Direction = dir;
            Destination = dest;
            IsMoving = true;

            StateChanged?.Invoke(this);

            return true;
        }

        public void Finish()
        {
            IsMoving = false;

            StateChanged?.Invoke(this);
        }

        public MovingSquare() {}
    }
}
