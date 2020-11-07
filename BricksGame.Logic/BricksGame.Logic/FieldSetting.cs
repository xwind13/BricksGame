using System.Collections.Generic;
using System.Linq;

namespace BricksGame.Logic
{
    public class InitialSquare : SquareBase
    {
        public InitialSquare(uint x, uint y, Color color) : base(x, y, color) {}
    }

    public class FieldSetting
    {
        public uint VertDimension { get; set; } = 0;
        public uint HorzDimension { get; set; } = 0;
        public uint SideDimension { get; set; } = 0;

        public List<InitialSquare> InitialSquares { get; set; }

        public int MaxSavedStatesCount { get; set; } = 0;

        public int WinScore { get; set; } = 150;

        public FieldSetting()
        {
            InitialSquares = new List<InitialSquare>();
        }

        public FieldSetting(FieldSetting setting)
        {
            if (setting == null || setting.IsValid())
                setting = FieldSetting.getDefault();

            VertDimension = setting.VertDimension;
            HorzDimension = setting.HorzDimension;
            SideDimension = setting.SideDimension;
            InitialSquares = setting.InitialSquares.
                Select(sq=> new InitialSquare(sq.X, sq.Y, sq.Color)).ToList();
        }

        private bool IsValid()
        {
            return VertDimension != 0 || HorzDimension != 0 || SideDimension != 0 
                || InitialSquares.All(sq => sq.X < HorzDimension && sq.Y < VertDimension && sq.Color != Color.None);
        }

        public static FieldSetting getDefault()
        {
            FieldSetting setting = new FieldSetting();
            setting.VertDimension = 10;
            setting.HorzDimension = 10;
            setting.SideDimension = 10;
            setting.MaxSavedStatesCount = 5;

            setting.InitialSquares.Add(new InitialSquare(5, 3, Color.Yellow));
            setting.InitialSquares.Add(new InitialSquare(3, 5, Color.Red));
            setting.InitialSquares.Add(new InitialSquare(4, 3, Color.Blue));
            setting.InitialSquares.Add(new InitialSquare(6, 5, Color.Violet));
            setting.InitialSquares.Add(new InitialSquare(3, 4, Color.Green));
            setting.InitialSquares.Add(new InitialSquare(5, 5, Color.LightGrey));
            setting.InitialSquares.Add(new InitialSquare(4, 6, Color.Brown));
            setting.InitialSquares.Add(new InitialSquare(5, 6, Color.DarkGrey));
            setting.InitialSquares.Add(new InitialSquare(4, 4, Color.Vanila));
            setting.InitialSquares.Add(new InitialSquare(6, 4, Color.Turquise));

            return setting;
        }
    }
}
