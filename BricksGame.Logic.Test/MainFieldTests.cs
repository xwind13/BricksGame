using BricksGame.Logic.Fields;
using BricksGame.Logic.Matrix;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BricksGame.Logic.Test
{
    public class MainFieldTests
    {
        public class TestMainFieldSquare : IMainFieldSquare
        {
            public MainFieldSquareState State { get; }

            public uint X { get; }

            public uint Y { get; }

            public Color Color { get; }

            public TestMainFieldSquare(uint x, uint y, MainFieldSquareState state, Color color)
            {
                X = x; Y = y; State = state; Color = color;
            }
        }

        public class TestMainField : MainField
        {
            public TestMainField(Matrix<IMainFieldSquare> m, List<InitialSquare> i, int c) : base(m, i, c) {}

            public static MainField Create(FieldSetting fieldSetting, List<TestMainFieldSquare> states)
            {
                var matrix = Create(fieldSetting);

                foreach (var state in states)
                {
                    var item = matrix.Matrix[state.X, state.Y] as MainFieldSquare;
                    item.SetState(state.State, state.Color);
                }
                
                return matrix;
            }
        }


        private FieldSetting CreateDefaultFieldSetting()
        {
            var initial = new List<InitialSquare>() { new InitialSquare(0, 0, Color.Red), new InitialSquare(9, 0, Color.Blue), new InitialSquare(8, 0, Color.Red) };
            return new FieldSetting() { HorzDimension = 10, VertDimension = 10, SideDimension = 3, InitialSquares = initial };
        }

        [Fact]
        public void ShouldCreateMainField()
        {
            var fieldSetting = CreateDefaultFieldSetting();
            var mainField = TestMainField.Create(fieldSetting, new List<TestMainFieldSquare>());

            mainField.Width.ShouldBe(fieldSetting.HorzDimension);
            mainField.Height.ShouldBe(fieldSetting.VertDimension);
        }

        [Fact]
        public void ShouldMainFieldResetSquareState()
        {
            var fieldSetting = CreateDefaultFieldSetting();
            var testValues = new List<TestMainFieldSquare>()
            {
                TS(3, 3, true, 2, Direction.Left, Color.Red),
            };

            var mainField = TestMainField.Create(fieldSetting, testValues);
            var item = mainField[3, 3].Value;
            item.State.IsActive.ShouldBeTrue();
            item.State.Obstacle.ShouldBe(2u);
            item.State.Direction.Value.ShouldBe(Direction.Left);
            item.Color.ShouldBe(Color.Red);
            mainField.ResetSquareState(3, 3);
            item.State.IsActive.ShouldBeFalse();
            item.State.Obstacle.ShouldBe(0u);
            item.State.Direction.Value.ShouldBe(Direction.None);
            item.Color.ShouldBe(Color.None);
        }

        [Fact]
        public void ShouldMainFieldResetToInitialState()
        {
            var fieldSetting = CreateDefaultFieldSetting();
            var testValues = new List<TestMainFieldSquare>()
            {
                TSA(3, 3, 2, Direction.Left, Color.Red),
                TSA(5, 3, 2, Direction.Left, Color.Red),
            };

            var mainField = TestMainField.Create(fieldSetting, testValues);
            mainField.Reset();
            mainField.Matrix.ForEach(item =>
            {
                var initialSquare = fieldSetting.InitialSquares.FirstOrDefault(initItem => initItem.X == item.X && initItem.Y == item.Y);
                if (initialSquare != null)
                {
                    item.State.IsActive.ShouldBeTrue();
                    item.State.Obstacle.ShouldBe((uint)(fieldSetting.InitialSquares.IndexOf(initialSquare) + 1));
                    item.State.Direction.Value.ShouldBe(Direction.None);
                    item.Color.ShouldBe(initialSquare.Color);
                    return;
                }

                item.State.IsActive.ShouldBeFalse();
                item.State.Obstacle.ShouldBe(0u);
                item.State.Direction.Value.ShouldBe(Direction.None);
                item.Color.ShouldBe(Color.None);
            });
        }

        [Fact]
        public void ShouldMainFieldGetSquareCombinations()
        {
            var fieldSetting = CreateDefaultFieldSetting();
            var firstCombination = new List<TestMainFieldSquare>()
            {
                TSA(2, 4, color: Color.Blue),
                TSA(2, 5, color: Color.Blue),
                TSA(2, 6, color: Color.Blue),
                TSA(3, 5, color: Color.Blue),
                TSA(3, 6, color: Color.Blue),
                TSA(3, 7, color: Color.Blue),
            };

            var secondCombination = new List<TestMainFieldSquare>()
            {
                TSA(3, 3, color: Color.Red),
                TSA(4, 3, color: Color.Red),
                TSA(5, 3, color: Color.Red),
            };

            var thirdNotCompleteCombination = new List<TestMainFieldSquare>()
            {
                TSA(4, 4, color: Color.Green),
                TSA(5, 4, color: Color.Green),
                TSA(7, 4, color: Color.Green),
            };

            var testValues = new List<TestMainFieldSquare>();
            testValues.AddRange(firstCombination);
            testValues.AddRange(secondCombination);
            testValues.AddRange(thirdNotCompleteCombination);

            var mainField = TestMainField.Create(fieldSetting, testValues);
            var combinations = mainField.GetSquareCombinations();

            combinations.Count().ShouldBe(2);
            var result1 = combinations.ElementAt(0);
            result1.Count().ShouldBe(6);
            firstCombination.ShouldAllBe(item => result1.Any(resItem => item.X == resItem.X && item.Y == resItem.Y));

            var result2 = combinations.ElementAt(1);
            result2.Count().ShouldBe(3);
            secondCombination.ShouldAllBe(item => result2.Any(resItem => item.X == resItem.X && item.Y == resItem.Y));
        }

        [Theory]
        [InlineData(Direction.Left, 4, 3)]
        [InlineData(Direction.Right, 4, 1)]
        [InlineData(Direction.Up, 2, 3)]
        [InlineData(Direction.Down, 2, 5)]
        [InlineData(Direction.Down, 5, -1)]
        [InlineData(Direction.Up, 0, -1)]
        public void ShouldFindAvalibleDestinationInDirection(Direction dir, uint lineIdx, int result)
        {
            var fieldSetting = CreateDefaultFieldSetting();
            var testValues = new List<TestMainFieldSquare>()
            {
                TSA(2, 4, color: Color.Blue),
            };

            var mainField = TestMainField.Create(fieldSetting, testValues);
            var dest = mainField.FindAvalibleDestinationInDirection(new MoveDirection(dir), lineIdx);
            dest.ShouldBe(result);
        }

        [Theory]
        [MemberData(nameof(InitValues))]
        public void ShouldReturnSquareWithDestination(List<TestMainFieldSquare> states, (TestMainFieldSquare, int) result)
        {
            var fieldSetting = CreateDefaultFieldSetting();
            var mainField = TestMainField.Create(fieldSetting, states);

            var item = mainField.FindSquareWithDestinationToMove();
            item.ShouldNotBeNull();

            var square = item.Square;

            item.Destination.ShouldBe(result.Item2);
            if (result.Item2 != -1)
            {
                square.ShouldNotBeNull();
                square.X.ShouldBe(result.Item1.X);
                square.Y.ShouldBe(result.Item1.Y);
            }
        }

        // first - same obstacle, second - same direction
        public static List<TestMainFieldSquare> GenerateHorzValues((bool, bool)[] fls, bool obstacleDestoyed)
        {
            var dir = Direction.Right;
            var clr = Color.Blue;
            var firstObs = obstacleDestoyed ? 1u : 2u;

            return new List<TestMainFieldSquare>
            {
                TS(0, 0),
                TSA(1, 2, fls[0].Item1 ? firstObs : 3u, fls[0].Item2 ? dir : Op(dir), clr),
                TSA(2, 2, fls[1].Item1 ? firstObs : 3u, fls[1].Item2 ? dir : Op(dir), clr),
                TSA(4, 2, fls[2].Item1 ? firstObs : 3u, fls[2].Item2 ? dir : Op(dir), clr),
                TSA(5, 2, fls[3].Item1 ? firstObs : 3u, fls[3].Item2 ? dir : Op(dir), clr),
            };
        }

        public static List<TestMainFieldSquare> GenerateVertValues((bool, bool)[] fls, bool obstacleDestoyed)
        {
            var dir = Direction.Up;
            var clr = Color.Blue;
            var firstObs = !obstacleDestoyed ? 2u : 1u;

            return new List<TestMainFieldSquare>
            {
                TS(0, 0),
                TSA(2, 1, fls[0].Item1 ? firstObs : 3u, fls[0].Item2 ? dir : Op(dir), clr),
                TSA(2, 2, fls[1].Item1 ? firstObs : 3u, fls[1].Item2 ? dir : Op(dir), clr),
                TSA(2, 4, fls[2].Item1 ? firstObs : 3u, fls[2].Item2 ? dir : Op(dir), clr),
                TSA(2, 5, fls[3].Item1 ? firstObs : 3u, fls[3].Item2 ? dir : Op(dir), clr),
            };
        }

        private static TestMainFieldSquare TS(uint x, uint y, bool isActive = false, uint obstacle = 0, Direction dir = Direction.None, Color color = Color.None)
        {
            return new TestMainFieldSquare(x, y, new MainFieldSquareState(isActive, obstacle, dir), color);
        }

        private static TestMainFieldSquare TSA(uint x, uint y, uint obstacle = 0, Direction dir = Direction.None, Color color = Color.None)
        {
            return TS(x, y, true, obstacle, dir, color);
        }

        private static Direction Op(Direction dir)
        {

            return new MoveDirection(dir).Opposite;
        }

        private static IEnumerable<object[]> GenerateAllInitValues()
        {
            var initValues = new List<object[]>();
            initValues.AddRange(GenerateInitValuesByAction(GenerateHorzValues));
            initValues.AddRange(GenerateInitValuesByAction(GenerateVertValues));

            initValues.Add(GetInitValueWithResult((2, 1), (2, 6), Direction.Up, 5));
            initValues.Add(GetInitValueWithResult((2, 8), (2, 3), Direction.Down, 4));
            initValues.Add(GetInitValueWithResult((1, 2), (6, 2), Direction.Right, 5));
            initValues.Add(GetInitValueWithResult((8, 2), (3, 2), Direction.Left, 4));


            return initValues;
        }

        private static IEnumerable<object[]> GenerateInitValuesByAction(Func<(bool, bool)[], bool, List<TestMainFieldSquare>> f)
        {
            var first = f == GenerateHorzValues ? TS(1, 2) : TS(2, 1);
            var second = TS(2, 2);

            return new List<object[]>
            {
                
                new object[] { f(GetMovementObstacleOrderCase(1), false), (first, 9) },  // 1 1 1 1  (>) < < <                    
                new object[] { f(GetMovementObstacleOrderCase(2), false), (second, 0) }, // 1 1 1 1  > (<) > <                    
                new object[] { f(GetMovementObstacleOrderCase(3), false), (second, 0) }, // 1 1 2 1  > (<) < >                  
                new object[] { f(GetMovementObstacleOrderCase(4), false), (first, 9) },  // 1 1 2 1  (>) < < < 
                
                new object[] { f(GetMovementObstacleOrderCase(1), true), (first, 9) },   // 1 1 1 1  (>) < < <                    
                new object[] { f(GetMovementObstacleOrderCase(2), true), (first, 9) },   // 1 1 1 1  (>) < > <                    
                new object[] { f(GetMovementObstacleOrderCase(3), true), (second, 0) },  // 1 1 2 1  > (<) < >                  
                new object[] { f(GetMovementObstacleOrderCase(4), true), (first, 9) },   // 1 1 2 1  (>) < < < 
            };
        }

        private static object[] GetInitValueWithResult((uint x, uint y) p1, (uint x, uint y) p2, Direction dir, int result)
        {
            return new object[] {

                new List<TestMainFieldSquare>
                {
                    TSA(p1.x, p1.y, 2, dir, Color.Blue),
                    TSA(p2.x, p2.y, 2, color: Color.Blue),
                },
                (TS(p1.x, p1.y), result)
            };
        }

        private static (bool, bool)[] GetMovementObstacleOrderCase(int i)
        {
                
            var obs1 = true; // 2 | 1
            var obs2 = false; // 3
            var dirR = true;  // >
            var dirL = false; // <

            return i switch
            {
                1 => new (bool, bool)[] { (obs1, dirR), (obs1, dirL), (obs1, dirL), (obs1, dirL) },
                2 => new (bool, bool)[] { (obs1, dirR), (obs1, dirL), (obs1, dirR), (obs1, dirL) },
                3 => new (bool, bool)[] { (obs1, dirR), (obs1, dirL), (obs2, dirL), (obs1, dirR) },
                4 => new (bool, bool)[] { (obs1, dirR), (obs1, dirL), (obs2, dirL), (obs1, dirL) },

                _ => new (bool, bool)[0]
            };
        }

        public static IEnumerable<object[]> InitValues => GenerateAllInitValues();

    }
}
