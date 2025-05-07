using System.Collections.Generic;
using Code.Gameplay.Common.Time;
using Entitas;

namespace Code.Gameplay.Features.LevelUp
{
    public class StartTimeOnLevelUpWindowProcessedSystem : ReactiveSystem<GameEntity>
    {
        private readonly ITimeService _timeService;

        public StartTimeOnLevelUpWindowProcessedSystem(GameContext game, ITimeService timeService) : base(game)
        {
            _timeService = timeService;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
            context.CreateCollector(GameMatcher.Processed.Added());

        protected override bool Filter(GameEntity entity) => entity.isLevelUp && entity.isProcessed;

        protected override void Execute(List<GameEntity> levelUps)
        {
            foreach (GameEntity _ in levelUps)
                _timeService.StartTime();
        }
    }
}