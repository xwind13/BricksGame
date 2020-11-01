namespace BricksGame.Logic
{
    public struct MoveDirection
    {
        public MoveDirection(Direction value)
        {
            Value = value;

            Opposite = value switch
            {
                Direction.Down => Direction.Up,
                Direction.Up => Direction.Down,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,

                _ => Direction.None
            };
        }

        public Direction Value { get; }

        public Direction Opposite { get; }
    }
}
