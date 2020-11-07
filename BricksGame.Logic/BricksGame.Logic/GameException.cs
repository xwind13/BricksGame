using System;

namespace BricksGame.Logic
{
    public class GameException : Exception
    {
        public GameException(string message) : base(message) {}
    }
}
