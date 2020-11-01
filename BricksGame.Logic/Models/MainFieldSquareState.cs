namespace BricksGame.Logic
{
    public struct MainFieldSquareState
    {
        public uint Obstacle { get; }
        public bool IsActive { get; }
        public MoveDirection Direction { get; }

        public MainFieldSquareState(bool isActive, uint obstacle, Direction direction)
        {
            Obstacle = obstacle;
            IsActive = isActive;
            Direction = new MoveDirection(direction);
        }
    }
}
