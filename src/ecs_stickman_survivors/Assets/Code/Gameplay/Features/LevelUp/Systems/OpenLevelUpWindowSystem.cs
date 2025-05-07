using System.Collections.Generic;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Services;
using Entitas;

namespace Code.Gameplay.Features.LevelUp.Systems
{
    public class OpenLevelUpWindowSystem : ReactiveSystem<GameEntity>
    {
        private readonly IWindowService _windowService;

        public OpenLevelUpWindowSystem(GameContext game, IWindowService windowService) : base(game)
        {
            _windowService = windowService;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
            context.CreateCollector(GameMatcher.LevelUp.Added());

        protected override bool Filter(GameEntity entity) => true;

        protected override void Execute(List<GameEntity> levelUps)
        {
            foreach (GameEntity _ in levelUps)
                _windowService.Open(WindowId.LevelUpWindow);
        }
    }
}