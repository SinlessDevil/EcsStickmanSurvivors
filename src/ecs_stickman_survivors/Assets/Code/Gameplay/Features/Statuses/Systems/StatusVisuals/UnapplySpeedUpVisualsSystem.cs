using System.Collections.Generic;
using Code.Gameplay.Features.Effects.Extensions;
using Entitas;

namespace Code.Gameplay.Features.Statuses.Systems.StatusVisuals
{
    public class UnapplySpeedUpVisualsSystem : ReactiveSystem<GameEntity>
    {
        public UnapplySpeedUpVisualsSystem(GameContext gameContext) : base(gameContext)
        {
            
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
            context.CreateCollector(GameMatcher
                .AllOf(
                    GameMatcher.SpeedUp,
                    GameMatcher.Status,
                    GameMatcher.Unapplied
                ).Added());

        protected override bool Filter(GameEntity entity) => entity.isStatus && entity.isSpeedUp && entity.hasTargetId && entity.isAffected;

        protected override void Execute(List<GameEntity> statuses)
        {
            foreach (GameEntity status in statuses)
            {
                GameEntity target = status.Target();
                if (target is {hasStatusVisuals: true}) 
                    target.StatusVisuals.UnapplySpeedUp();
            }
        }
    }
}