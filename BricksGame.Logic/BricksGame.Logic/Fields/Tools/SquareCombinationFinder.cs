using BricksGame.Logic.Matrix;
using System.Collections.Generic;
using System.Linq;

namespace BricksGame.Logic.Fields.Tools
{
    public class SquareCombinationFinder
    {
        private readonly MainField _mainField;

        private List<SquareCombinationCounter> _counters;

        private SquareCombinationCounter[,] _matrixCounterRefs;

        public SquareCombinationFinder(MainField mainField)
        {
            _mainField = mainField;

            _counters = new List<SquareCombinationCounter>();
            _matrixCounterRefs = new SquareCombinationCounter[mainField.Width, mainField.Height];
        }

        /// <summary>
        /// find combination with 3 or more squares.
        /// </summary>
        public IEnumerable<IEnumerable<IMainFieldSquare>> Find()
        {
            _mainField.ForEach(item => CountCombinationSize(item));

            return _counters.Where(counter => counter.Squares.Count >= 3).Select(counter => counter.Squares);
        }

        private void CountCombinationSize(IChainLink<IMainFieldSquare, Direction> item, SquareCombinationCounter counter = null)
        {
            if (item == null)
                return;

            var value = item.Value;
            // skip if item is not active or it was already counted as part of combination
            if (!value.State.IsActive || _matrixCounterRefs[value.X, value.Y] != null)
                return;

            if (counter == null)
            {
                counter = new SquareCombinationCounter(value.Color);
                _counters.Add(counter);
            }

            if (counter.Color != value.Color)
                return;

            counter.Squares.Add(value);
            _matrixCounterRefs[value.X, value.Y] = counter;

            CountCombinationSize(item.Next(Direction.Left), counter);
            CountCombinationSize(item.Next(Direction.Down), counter);
            CountCombinationSize(item.Next(Direction.Right), counter);
            CountCombinationSize(item.Next(Direction.Up), counter);
        }
    }
}
