namespace BricksGame.Logic
{
    public static class MoveDirectionExtenstions
    {
        public static bool IsNone(this MoveDirection direction)
        {
            return direction.Value == Direction.None;
        }

        public static bool IsLeft(this MoveDirection direction)
        {
            return direction.Value == Direction.Left;
        }

        public static bool IsRight(this MoveDirection direction)
        {
            return direction.Value == Direction.Right;
        }

        public static bool IsUp(this MoveDirection direction)
        {
            return direction.Value == Direction.Up;
        }

        public static bool IsDown(this MoveDirection direction)
        {
            return direction.Value == Direction.Down;
        }

        public static bool IsHorzOrient(this MoveDirection direction)
        {
            return direction.IsLeft() || direction.IsRight();
        }

        public static bool IsVertOrient(this MoveDirection direction)
        {
            return direction.IsUp() || direction.IsDown();
        }
    }
}
