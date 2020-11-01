namespace BricksGame.Logic
{
    public interface IMainFieldSquare : ISquare
    {
        MainFieldSquareState State { get; }
    }
}
