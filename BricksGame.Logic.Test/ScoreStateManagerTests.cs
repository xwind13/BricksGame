using BricksGame.Logic.Models;
using BricksGame.Logic.StateManagers;
using Shouldly;
using Xunit;

namespace BricksGame.Logic.Test
{
    public class ScoreStateManagerTests
    {

        [Fact]
        public void ShouldCreateScoreStateManager()
        {
            var setting = new FieldSetting();
            var scoreStateManager = new ScoreStateManager(new Score(setting), 2);
            scoreStateManager.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldScoreStateManagerNotChangeScoreIfNotSavedStates()
        {
            var setting = new FieldSetting();
            var score = new Score(setting);
            score.Add(444);
            var scoreStateManager = new ScoreStateManager(score, 2);

            var result = scoreStateManager.BackToPrevious();
            result.ShouldBeFalse();

            score.Value.ShouldBe(444);
        }

        [Fact]
        public void ShouldScoreStateManagerRestoreToPreviousStates()
        {
            var setting = new FieldSetting();
            var score = new Score(setting);
            var scoreStateManager = new ScoreStateManager(score, 2);

            scoreStateManager.Save();
            score.Add(2);

            scoreStateManager.Save();
            score.Add(3);

            scoreStateManager.Save();
            score.Add(4);

            score.Value.ShouldBe(9);

            var result = scoreStateManager.BackToPrevious();
            result.ShouldBeTrue();
            score.Value.ShouldBe(5);

            result = scoreStateManager.BackToPrevious();
            result.ShouldBeTrue();
            score.Value.ShouldBe(2);

            result = scoreStateManager.BackToPrevious();
            result.ShouldBeFalse();
            score.Value.ShouldBe(2);
        }
    }
}
