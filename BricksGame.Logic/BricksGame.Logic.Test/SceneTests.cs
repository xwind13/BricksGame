using Xunit;

namespace BricksGame.Logic.Test
{
    public class SceneTests
    {
        [Fact]
        public void ShouldThrowSquare()
        {
            var scene = new Scene(FieldSetting.getDefault());
            scene.ThrowSquare(Side.Top, 5);
        }
    }
}
