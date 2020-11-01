using BricksGame.Logic.Matrix;
using BricksGame.Logic.StateManagers;
using Shouldly;
using Xunit;

namespace BricksGame.Logic.Test
{
    public class TestStateValue : IStateCopyable<TestStateValue>
    {
        public int Value { get; set; }

        public TestStateValue(int value)
        {
            Value = value;
        }

        public void CopyState(TestStateValue other)
        {
            Value = other.Value;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class MatrixStateManagerTests
    {
        private TestStateValue[,] GetTestMatrixValues()
        {
            var matrixValues = new TestStateValue[3, 2];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    matrixValues[i, j] = new TestStateValue(i);
                }
            }

            return matrixValues;
        }


        [Fact]
        public void ShouldCreateMatrixStateManager()
        {
            var matrixStateManager = new MatrixStateManager<TestStateValue>(new Matrix<TestStateValue>(GetTestMatrixValues()), 2);
            matrixStateManager.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldMatrixStateManagerNotChangeMatrixValuesIfNotSavedStates()
        {
            var matrix = new Matrix<TestStateValue>(GetTestMatrixValues());
            var matrixStateManager = new MatrixStateManager<TestStateValue>(matrix, 2);

            var result = matrixStateManager.BackToPrevious();
            result.ShouldBeFalse();

            matrix.ForEach((item, x, y) => item.Value.ShouldBe((int)x));
        }

        [Fact]
        public void ShouldMatrixStateManagerRestoreToPreviousStates()
        {
            var matrix = new Matrix<TestStateValue>(GetTestMatrixValues());
            var matrixStateManager = new MatrixStateManager<TestStateValue>(matrix, 2);

            matrixStateManager.Save();
            matrix.ForEach((item, x, y) => item.Value = (int)(2 * x));

            matrixStateManager.Save();
            matrix.ForEach((item, x, y) => item.Value = (int)(3 * x));

            matrixStateManager.Save();
            matrix.ForEach((item, x, y) => item.Value = (int)(4 * x));
            
            matrix.ForEach((item, x, y) => item.Value.ShouldBe((int)(4 * x)));

            var result = matrixStateManager.BackToPrevious();
            result.ShouldBeTrue();
            matrix.ForEach((item, x, y) => item.Value.ShouldBe((int)(3 * x)));

            result = matrixStateManager.BackToPrevious();
            result.ShouldBeTrue();
            matrix.ForEach((item, x, y) => item.Value.ShouldBe((int)(2 * x)));

            result = matrixStateManager.BackToPrevious();
            result.ShouldBeFalse();
            matrix.ForEach((item, x, y) => item.Value.ShouldBe((int)(2 * x)));
        }
    }
}
