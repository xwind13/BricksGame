using System.Collections.Generic;

namespace BricksGame.Logic.Fields.Tools
{
    public class SquareCombinationCounter
    {
        public Color Color { get; }

        public List<IMainFieldSquare> Squares { get; }

        public SquareCombinationCounter(Color color)
        {
            Color = color;
            Squares = new List<IMainFieldSquare>();
        }
    }
}
