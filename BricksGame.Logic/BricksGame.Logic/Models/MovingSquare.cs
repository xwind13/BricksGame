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

        public event Action<MovingSquare> MoveStarted;

        public event Action<MovingSquare> MoveFinished;

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

            MoveStarted?.Invoke(this);

            return true;
        }

        public void Finish()
        {
            IsMoving = false;

            MoveFinished?.Invoke(this);
        }

        public MovingSquare() {}
    }
}
