using BricksGame.Logic.Matrix;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace BricksGame.Logic.Test
{
    public class TestValue
    {
        public int Value { get; set; }

        public TestValue(int value)
        {
            Value = value;
        }
    }

    public class MatrixTests
    {
        public MatrixTests() { }

        [Fact]
        public void ShouldCreateMatrix()
        {
            var items = new TestValue[3, 2];

            var matrix = new Matrix<TestValue>(items);

            matrix.Height.ShouldBe((uint)2);
            matrix.Width.ShouldBe((uint)3);
        }

        [Theory]
        [MemberData(nameof(InitValues))]
        public void ShouldCreateMatrixWithValues(TestValue[,] items)
        {
            var matrix = new Matrix<TestValue>(items);

            matrix.Height.ShouldBe((uint)items.GetLength(1));
            matrix.Width.ShouldBe((uint)items.GetLength(0));

            for (uint i = 0; i < matrix.Width; i++)
                for (uint j = 0; j < matrix.Height; j++)
                    matrix[i, j].ShouldBe(items[i, j]);
        }

        [Fact]
        public void ShouldFindValueInMatrix()
        {
            var matrix = new Matrix<TestValue>(new TestValue[,] { { new TestValue(1), new TestValue(2), new TestValue(3) } });

            var item = matrix.Find((item) => item.Value == 2);
            item.Value.ShouldBe(2);
        }

        [Fact]
        public void ShouldNotFindValueInMatrix()
        {
            var matrix = new Matrix<TestValue>(new TestValue[,] { { new TestValue(1), new TestValue(2), new TestValue(3) } });

            var item = matrix.Find((item) => item.Value == 5);
            item.ShouldBeNull();
        }

        [Theory]
        [MemberData(nameof(InitValues))]
        public void ShouldCreateChainedMatrix(TestValue[,] items)
        {
            var chainedMatrix = new LBChainedMatrix<TestValue>(new Matrix<TestValue>(items));

            for (uint i = 0; i < chainedMatrix.Width; i++)
                for (uint j = 0; j < chainedMatrix.Height; j++)
                    chainedMatrix[i, j].Value.ShouldBe(items[i, j]);
        }


        [Theory]
        [MemberData(nameof(InitValues))]
        public void ShouldMoveByLinksVertically__LeftBottomCS(TestValue[,] items)
        {
            var chainedMatrix = new LBChainedMatrix<TestValue>(new Matrix<TestValue>(items));

            for (uint i = 0; i < chainedMatrix.Width; i++)
            {
                uint j = 0;
                var nextLink = chainedMatrix[i, j];
                nextLink.Value.ShouldBe(items[i, j]);

                while((nextLink = nextLink.Next(Direction.Up)) != null)
                {
                    j++;
                    nextLink.Value.ShouldBe(items[i, j]);
                }

                j.ShouldBe(chainedMatrix.Height - 1);
            }
        }

        [Theory]
        [MemberData(nameof(InitValues))]
        public void ShouldMoveByLinksHorizontally__LeftBottomCS(TestValue[,] items)
        {
            var chainedMatrix = new LBChainedMatrix<TestValue>(new Matrix<TestValue>(items));

            for (uint j = 0; j < chainedMatrix.Height; j++)
            {
                uint i = 0;
                var nextLink = chainedMatrix[i, j];
                nextLink.Value.ShouldBe(items[i, j]);

                while ((nextLink = nextLink.Next(Direction.Right)) != null)
                {
                    i++;
                    nextLink.Value.ShouldBe(items[i, j]);
                }

                i.ShouldBe(chainedMatrix.Width - 1);
            }
        }

        [Theory]
        [MemberData(nameof(InitValues))]
        public void ShouldMoveByLinksOppositeVertically__LeftBottomCS(TestValue[,] items)
        {
            var chainedMatrix = new LBChainedMatrix<TestValue>(new Matrix<TestValue>(items));

            for (uint i = 0; i < chainedMatrix.Width; i++)
            {
                uint j = chainedMatrix.Height - 1;
                var nextLink = chainedMatrix[i, j];
                nextLink.Value.ShouldBe(items[i, j]);

                while ((nextLink = nextLink.Next(Direction.Down)) != null)
                {
                    j--;
                    nextLink.Value.ShouldBe(items[i, j]);
                }

                j.ShouldBe((uint)0);
            }
        }

        [Theory]
        [MemberData(nameof(InitValues))]
        public void ShouldMoveByLinksOppositeHorizontally__LeftBottomCS(TestValue[,] items)
        {
            var chainedMatrix = new LBChainedMatrix<TestValue>(new Matrix<TestValue>(items));

            for (uint j = 0; j < chainedMatrix.Height; j++)
            {
                uint i = chainedMatrix.Width - 1;
                var nextLink = chainedMatrix[i, j];
                nextLink.Value.ShouldBe(items[i, j]);

                while ((nextLink = nextLink.Next(Direction.Left)) != null)
                {
                    i--;
                    nextLink.Value.ShouldBe(items[i, j]);
                }

                i.ShouldBe((uint)0);
            }
        }

        [Fact]
        public void ShouldThrowExIfMoveUnknownDirection()
        {
            var items = new TestValue[,] { { new TestValue(1), new TestValue(2), new TestValue(3) } };
            var chainedMatrix = new LBChainedMatrix<TestValue>(new Matrix<TestValue>(items));

            var item = chainedMatrix[0, 0];

            Assert.Throws<KeyNotFoundException>(() => item.Next(Direction.None));
        }

        [Fact]
        public void ShouldThrowExIfRangeOut()
        {
            var items = new TestValue[,] { { new TestValue(1), new TestValue(2), new TestValue(3) } };
            var chainedMatrix = new LBChainedMatrix<TestValue>(new Matrix<TestValue>(items));

            Assert.Throws<IndexOutOfRangeException>(() => chainedMatrix[50, 50]);
        }

        [Fact]
        public void ShouldChangeLinkedValue()
        {
            var items = new TestValue[,] { { new TestValue(1), new TestValue(2), new TestValue(3) } };
            var chainedMatrix = new LBChainedMatrix<TestValue>(new Matrix<TestValue>(items));

            int newValue = 59;

            chainedMatrix.ForEach((linkItem) => linkItem.Value.Value = newValue);

            for (uint i = 0; i < chainedMatrix.Width; i++)
                for (uint j = 0; j < chainedMatrix.Height; j++)
                {
                    chainedMatrix[i, j].Value.Value.ShouldBe(newValue);
                    chainedMatrix[i, j].Value.ShouldBe(items[i, j]);
                }
        }

        public static IEnumerable<object[]> InitValues =>
            new List<object[]>
            {
                new object[] { new TestValue[,] { } },

                new object[] { new TestValue[,] { { new TestValue(1), new TestValue(2), new TestValue(3) } } },

                new object[] { new TestValue[,] 
                { 
                    { new TestValue(1), new TestValue(2), new TestValue(3) } ,
                    { new TestValue(1), new TestValue(2), new TestValue(3) } ,
                    { new TestValue(1), new TestValue(2), new TestValue(3) }
                } },
            };
    }
}
