using BricksGame.Logic.Fields;
using Shouldly;
using Xunit;

namespace BricksGame.Logic.Test
{
    public class SideFieldTests
    {
        [Theory]
        [InlineData(Side.Bottom, Direction.Up)]
        [InlineData(Side.Left, Direction.Right)]
        [InlineData(Side.Right, Direction.Left)]
        [InlineData(Side.Top, Direction.Down)]
        public void ShouldCreateSideField(Side location, Direction direction)
        {
            var fieldSetting = new FieldSetting() { HorzDimension = 5, VertDimension = 4, SideDimension = 3 };
            var sideField = SideField.Create(fieldSetting, location);

            var width = (location == Side.Left || location == Side.Right) ? fieldSetting.VertDimension : fieldSetting.HorzDimension;

            sideField.Width.ShouldBe(width);
            sideField.Height.ShouldBe(fieldSetting.SideDimension);
            sideField.ThrowDirection.Value.ShouldBe(direction);
        }

        [Fact]
        public void SholdGenerateValuesForSideFields()
        {
            var fieldSetting = new FieldSetting() { HorzDimension = 5, VertDimension = 4, SideDimension = 3 };
            var sideField = SideField.Create(fieldSetting, Side.Bottom);

            sideField.Generate();

            sideField.ForEach((item) =>
            {
                item.Color.ShouldNotBe(Color.None);
            });
        }


        [Theory]
        [InlineData(Side.Bottom)]
        [InlineData(Side.Left)]
        [InlineData(Side.Right)]
        [InlineData(Side.Top)]
        public void ShouldSideFieldThrowsSquare(Side location)
        {
            var fieldSetting = new FieldSetting() { HorzDimension = 5, VertDimension = 4, SideDimension = 3 };
            var sideField = SideField.Create(fieldSetting, location);

            bool same = true;

            for (uint i = 0; i < sideField.Width; i++)
            {
                var color = sideField[i, 0].Color;
                var secondColor = sideField[i, 1].Color;
                var thirdColor = sideField[i, 2].Color;

                var throwColor = sideField.Pop(i);
                color.ShouldBe(throwColor);
                sideField[i, 0].Color.ShouldBe(secondColor);
                sideField[i, 1].Color.ShouldBe(thirdColor);

                sideField[i, 2].Color.ShouldNotBe(Color.None);

                same &= sideField[i, 2].Color == thirdColor;
            }

            same.ShouldBeFalse();
        }

        [Theory]
        [InlineData(Side.Bottom)]
        [InlineData(Side.Left)]
        [InlineData(Side.Right)]
        [InlineData(Side.Top)]
        public void ShouldSideFieldPutsSquare(Side location)
        {
            var fieldSetting = new FieldSetting() { HorzDimension = 5, VertDimension = 4, SideDimension = 3 };
            var sideField = SideField.Create(fieldSetting, location);

            for (uint i = 0; i < sideField.Width; i++)
            {
                var color = sideField[i, 0].Color;
                var secondColor = sideField[i, 1].Color;

                sideField.Push(i, Color.Red);

                sideField[i, 0].Color.ShouldBe(Color.Red);
                sideField[i, 1].Color.ShouldBe(color);

                sideField[i, 2].Color.ShouldBe(secondColor);

            }
        }

        [Fact]
        public void ShouldReturnReadonlyMatrix()
        {
            var fieldSetting = new FieldSetting() { HorzDimension = 5, VertDimension = 4, SideDimension = 3 };
            var sideField = SideField.Create(fieldSetting, Side.Bottom);


            var matrix = sideField.ToReadonlyMatrix();
            matrix.ShouldNotBeNull();

            sideField.ForEach((item, x, y) => matrix[x, y].ShouldBe(item));

        }
    }
}
