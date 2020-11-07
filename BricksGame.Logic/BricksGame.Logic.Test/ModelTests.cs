using Shouldly;
using System.Reflection;
using Xunit;

namespace BricksGame.Logic.Test
{
    public class ModelTests
    {
        [Theory]
        [InlineData(Direction.Down, Direction.Up)]
        [InlineData(Direction.Up, Direction.Down)]
        [InlineData(Direction.Left, Direction.Right)]
        [InlineData(Direction.Right, Direction.Left)]
        [InlineData(Direction.None, Direction.None)]
        public void ShouldChangeDirection(Direction value, Direction opposite)
        {
            var direction = new MoveDirection(value);

            direction.Value.ShouldBe(value);
            direction.Opposite.ShouldBe(opposite);
        }

        [Fact]
        public void ShouldCopyAllMainFieldSquareStateProperties()
        {
            var squareState = new MainFieldSquareState(true, 2, Direction.Left);
            var clonedObj = squareState;

            ReferenceEqualsForItemProperties(squareState, clonedObj);
        }

        private void ReferenceEqualsForItemProperties(object initItem, object clonedItem)
        {
            ReferenceEquals(initItem, clonedItem).ShouldBeFalse();

            var properties = initItem.GetType().GetProperties();
            foreach (var property in properties)
            {
                ReferenceEqualsForItemProperties(property.GetValue(initItem), property.GetValue(clonedItem));
            }
        }

    }
}
